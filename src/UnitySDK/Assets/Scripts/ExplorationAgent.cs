﻿using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System;

public class ExplorationAgent : Agent
{
    [Header("Exploration Agent Settings")]
    public float maxSpeed = 25f;
    public float turnSpeed = 300;
    public float moveSpeed = 2f;
    public float[] rayAngles = { 20f, 30f, 40f, 50f, 60f, 70f, 80f, 90f, 100f, 110f, 120f, 130f, 140f, 150f };
    public float rayDistance;
    public bool useVectorObs = true;

    private Vector3[] movement;
    private Vector3 translation;
    private Vector3 rotation;

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
        translation = new Vector3();
        rotation = new Vector3();
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
            if(exArea.is3D)
            {
                AddVectorObs(rayPerception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 5f));
                AddVectorObs(rayPerception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, -5f));
            }

            // Agent velocity
            Vector3 localVelocity = transform.InverseTransformDirection(body.velocity).normalized;

            // Normalization
            AddVectorObs(localVelocity.x);
            if(exArea.is3D)
            {
                AddVectorObs(localVelocity.y);

            }
            AddVectorObs(localVelocity.z);
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (exArea.is3D)
        {
            get3DMovement(vectorAction);
        } else
        {
            get2DMovement(vectorAction);
        }

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

    private void get2DMovement(float[] actions)
    {
        var fwDirection = Vector3.zero;
        var rDirection = Vector3.zero;
        var yawRot = Vector3.zero;

        var forwardAxis = (int)actions[0];
        var rightAxis = (int)actions[1];
        var yawRotationAxis = (int)actions[2];

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

        switch (yawRotationAxis)
        {
            case 1:
                yawRot = -transform.up;
                break;
            case 2:
                yawRot = transform.up;
                break;
        }

        translation = fwDirection + rDirection;
        rotation = yawRot;

        float[] actionHist = {forwardAxis == 2 ? -1 : forwardAxis,
                              rightAxis == 2 ? -1 : rightAxis,
                              yawRotationAxis == 2 ? -1 : yawRotationAxis};

        Monitor.Log("Vector Action", actionHist);
    }

    private void get3DMovement(float[] actions)
    {
        var fwDirection = Vector3.zero;
        var rDirection = Vector3.zero;
        var uDirection = Vector3.zero;
        var yawRot = 0.0f;
        var pitchRot = 0.0f;

        var forwardAxis = (int)actions[0];
        var rightAxis = (int)actions[1];
        var upAxis = (int)actions[2];
        var yawRotationAxis = (int)actions[3];
        var pitchRotationAxis = (int)actions[4];

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

        switch (upAxis)
        {
            case 1:
                uDirection = -transform.up;
                break;
            case 2:
                uDirection = transform.up;
                break;
        }

        switch (yawRotationAxis)
        {
            case 1:
                yawRot = -transform.rotation.eulerAngles.y;
                break;
            case 2:
                yawRot = transform.transform.rotation.eulerAngles.y;
                break;
        }

        switch (pitchRotationAxis)
        {
            case 1:
                pitchRot = -transform.rotation.eulerAngles.x;
                break;
            case 2:
                pitchRot = transform.transform.rotation.eulerAngles.x;
                break;
        }

        translation = fwDirection + rDirection + uDirection;
        rotation = new Vector3(pitchRot, yawRot, 0.0f);

        float[] actionHist = {forwardAxis == 2 ? -1 : forwardAxis,
                              rightAxis == 2 ? -1 : rightAxis,
                              upAxis == 2 ? -1 : upAxis,
                              yawRotationAxis == 2 ? -1 : yawRotationAxis,
                              pitchRotationAxis == 2 ? -1 : pitchRotationAxis,
        };

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
            float penality = -0.1f; //(float)(-Math.Exp(exArea.collisionPenalty * exArea.obstacleCollisions) + 1);
            AddReward(penality);
        }
    }

    void FixedUpdate()
    {
        body.AddForce(translation * moveSpeed, ForceMode.VelocityChange);
        transform.Rotate(rotation, Time.fixedDeltaTime * turnSpeed);
        
        if (body.velocity.sqrMagnitude > maxSpeed)
        {
            body.velocity *= 0.95f;
        }

    }

    void Update()
    {
        if (exArea.drawAgentRays)
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