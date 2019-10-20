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
    private bool resetting;

    private const float MIN_REWARD = -1f;
    private const float WIN_REWARD = 1f;



    void Start()
    {
        body = GetComponent<Rigidbody>();
        exAcademy = FindObjectOfType<ExplorationAcademy>();
        exArea = transform.parent.GetComponent<ExplorationArea>();
        rayPerception = GetComponent<RayPerception3D>();
        reachedGoal = false;
        resetting = false;
    }

    public override void AgentReset()
    {
        body.velocity = Vector3.zero;
        reachedGoal = false;
        resetting = false;
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

        if (GetCumulativeReward() < MIN_REWARD && !resetting)
        {
            Done();
            exArea.FailResetArea();
            resetting = true;

        }
        else if (reachedGoal && !resetting)
        {
            Done();
            exArea.SuccessResetArea();
            resetting = true;

        }
        else
        {
            AddReward(-.001f);
            exArea.UpdateScore(GetCumulativeReward());
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            reachedGoal = true;
            AddReward(WIN_REWARD);
            exArea.UpdateScore(GetCumulativeReward());
        } else if(collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("LevelBoundaries"))
        {
            AddReward(-.1f);
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