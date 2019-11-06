using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System;

public class ExplorationAgent : Agent
{
    [Header("Exploration Agent Settings")]
    public float maxSpeed = 25f;
    public float turnSpeed = 300;
    public float moveSpeed = 2f;
    public bool showRays = true;
    public float[] rayAngles = { 20f, 30f, 40f, 50f, 60f, 70f, 80f, 90f, 100f, 110f, 120f, 130f, 140f, 150f };
    public float rayDistance;
    public bool useVectorObs = true;

    private Vector3[] movement;

    private ExplorationArea exArea;
    private Rigidbody body;
    private RayPerception3D rayPerception;

    private bool reachedGoal;
    private bool resetting;

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        body = GetComponent<Rigidbody>();
        exArea = transform.parent.GetComponent<ExplorationArea>();
        rayPerception = GetComponent<RayPerception3D>();

        movement = new Vector3[3];
        reachedGoal = false;
        resetting = false;
    }

    public override void AgentReset()
    {
        reachedGoal = false;
        resetting = false;
    }

    public override void CollectObservations()
    {
        if (useVectorObs)
        {
            string[] detectableObjects = { "LevelBoundaries", "Obstacle", "Goal" };

            // Add obstacles and goal observations
            AddVectorObs(rayPerception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));

            // Agent velocity
            Vector3 localVelocity = transform.InverseTransformDirection(body.velocity);
            AddVectorObs(localVelocity.x);
            AddVectorObs(localVelocity.z);
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        getMovement(vectorAction);

        if (GetCumulativeReward() < exArea.minReward && !resetting)
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
            AddReward(exArea.timePenalty);
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

        float[] actionHist = {forwardAxis == 2 ? -1 : forwardAxis,
                              rightAxis == 2 ? -1 : rightAxis,
                              horRotationAxis == 2 ? -1 : horRotationAxis};

        Monitor.Log("Vector Action", actionHist);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            reachedGoal = true;
            AddReward(exArea.winReward);
            exArea.UpdateScore(GetCumulativeReward());
        } else if(collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("LevelBoundaries"))
        {
            exArea.OnObstacleCollision();
            float penality = (float)(-Math.Exp(exArea.collisionPenalty * exArea.obstacleCollisions) + 1);
            AddReward(penality);
        }
    }

    void FixedUpdate()
    {
        var direction = movement[0] + movement[1];
        body.AddForce(direction * moveSpeed, ForceMode.VelocityChange);
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
                Debug.DrawRay(transform.position, endPosition, Color.gray, 0.01f, depthTest:true);
            }

        }
    }
}