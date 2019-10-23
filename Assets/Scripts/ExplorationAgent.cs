using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class ExplorationAgent : Agent
{
    [Header("Exploration Agent Settings")]
    public float maxSpeed = 25f;
    public float turnSpeed = 300;
    public float moveSpeed = 2f;
    public bool showRays = true;
    public float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };
    public float rayDistance = 20f;

    private Vector3[] movement;

    private ExplorationAcademy exAcademy;
    private ExplorationArea exArea;
    private Rigidbody body;
    private RayPerception3D rayPerception;

    private bool reachedGoal;
    private bool resetting;

    private const float MIN_REWARD = -1f;
    private const float WIN_REWARD = 1f;


    public override void InitializeAgent()
    {
        base.InitializeAgent();
        body = GetComponent<Rigidbody>();
        exAcademy = FindObjectOfType<ExplorationAcademy>();
        exArea = transform.parent.GetComponent<ExplorationArea>();
        rayPerception = GetComponent<RayPerception3D>();

        movement = new Vector3[3];
        reachedGoal = false;
        resetting = false;
    }

    public override void AgentReset()
    {
        body.velocity = Vector3.zero;
        transform.position = new Vector3(Random.Range(-exArea.spawnRange, exArea.spawnRange),
            2f, Random.Range(-exArea.spawnRange, exArea.spawnRange))
            + exArea.transform.position;
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

        reachedGoal = false;
        resetting = false;
    }

    public override void CollectObservations()
    {
        string[] detectableObjects = { "LevelBoundaries", "Obstacle", "Goal" };

        // Add obstacles and goal observations
        AddVectorObs(rayPerception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));

        // Agent velocity
        Vector3 localVelocity = transform.InverseTransformDirection(body.velocity);
        AddVectorObs(localVelocity.x);
        AddVectorObs(localVelocity.z);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        getMovement(vectorAction);

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

    private void getMovement(float[] actions)
    {
        var fwDirection = Vector3.zero;
        var rDirection = Vector3.zero;
        var rotation = Vector3.zero;

        var forwardAxis = (int)actions[0];
        var rightAxis = (int)actions[1];
        var horRotationAxis = (int)actions[2];

        switch (forwardAxis)
        {
            case 1:
                fwDirection = transform.forward;
                break;
            case 2:
                fwDirection = -transform.forward;
                break;
        }

        switch (rightAxis)
        {
            case 1:
                rDirection = -transform.right;
                break;
            case 2:
                rDirection = transform.right;
                break;
        }

        switch (horRotationAxis)
        {
            case 1:
                rotation = -transform.up;
                break;
            case 2:
                rotation = transform.up;
                break;
        }

        movement[0] = fwDirection;
        movement[1] = rDirection;
        movement[2] = rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            reachedGoal = true;
            AddReward(WIN_REWARD);
            exArea.UpdateScore(GetCumulativeReward());
            exArea.ResetGoal();
        } else if(collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("LevelBoundaries"))
        {
            AddReward(-.1f);
        }
    }

    void FixedUpdate()
    {
        var direction = movement[0] + movement[1];
        body.AddForce( direction * moveSpeed, ForceMode.VelocityChange);
        transform.Rotate(movement[2], Time.fixedDeltaTime * turnSpeed);

        if (body.velocity.sqrMagnitude > maxSpeed)
        {
            body.velocity *= 0.95f;
        }

    }

    void Update()
    {
        if (showRays)
        {
            foreach (var angle in rayAngles)
            {
                var endPosition = transform.TransformDirection(
                    RayPerception3D.PolarToCartesian(rayDistance, angle));
                Debug.DrawRay(transform.position, endPosition, Color.cyan, 0.01f, depthTest:true);
            }

        }
    }
}