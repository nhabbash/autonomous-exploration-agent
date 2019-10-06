using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class ExplorationAgent : Agent
{
    [Header("Exploration Agent Settings")]
    public float maxSpeed = 8f;
    public float speed = 1f;
    public bool accelerate = true;

    private ExplorationAcademy exAcademy;
    private ExplorationArea exArea;
    private Rigidbody body;
    private RayPerception3D rayPerception;
    private Vector3 signals = Vector3.zero;

    private bool reachedGoal;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        exAcademy = FindObjectOfType<ExplorationAcademy>();
        exArea = transform.parent.GetComponent<ExplorationArea>();
        rayPerception = GetComponent<RayPerception3D>();
        reachedGoal = false;
    }

    public override void AgentReset()
    {
        body.velocity = Vector3.zero;
        reachedGoal = false;
    }

    public override void CollectObservations()
    {
        float rayDistance = 2f;
        float[] rayAngles = { 90f };
        string[] detectableObjects = { "LevelBoundaries", "Obstacle", "Goal" };
        string[] detectableGoal = { "Goal" };

        // Add obstacles and goal observations
        AddVectorObs(rayPerception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        //AddVectorObs(rayPerception.Perceive(rayDistance, rayAngles, detectableGoal, 0f, 0f));

        // Agent velocity
        Vector3 localVelocity = transform.InverseTransformDirection(body.velocity);
        AddVectorObs(localVelocity.x);
        AddVectorObs(localVelocity.z);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        signals = controlSignal;

        // Reward, determine state

        if (GetCumulativeReward() < -5f)
        {
            // Reward is too negative, give up
            Done();

            // Color area
            StartCoroutine(exArea.SwapGroundMaterial(success: false));

            // Reset
            exArea.ResetArea();

        }
        else if (reachedGoal)
        {
            Done();

            // Color area
            StartCoroutine(exArea.SwapGroundMaterial(success: true));

            // Reset
            exArea.ResetArea();

        }else
        {
            // Encourage more movement with a small penalty
            AddReward(-.001f);
            exArea.UpdateScore(GetCumulativeReward());
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            reachedGoal = true;
            AddReward(1f);
            exArea.UpdateScore(GetCumulativeReward());
            Destroy(collision.gameObject);
        }else if(collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("LevelBoundaries"))
        {
            AddReward(-.01f);
        }
    }

    void FixedUpdate()
    {
        if (!accelerate)
            body.MovePosition(body.position + signals * speed * Time.fixedDeltaTime);
        else
        {
            Vector3 accel = signals * speed;

            if (body.velocity.magnitude > maxSpeed)
            {
                body.velocity = body.velocity.normalized * maxSpeed;
            }

            body.AddForce(accel, ForceMode.VelocityChange);

        }
    }
}