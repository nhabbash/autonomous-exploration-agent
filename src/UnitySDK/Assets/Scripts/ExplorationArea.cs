using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MLAgents;
using System.IO;

public class ExplorationArea : Area
{
    [Header("Settings")]
    public bool is3D;
    public bool isStructured;

    [Header("Exploration Area Objects")]
    public GameObject expAgent;
    public GameObject ground;
    public GameObject area3D;
    public Material successMaterial;
    public Material failureMaterial;
    public TextMeshPro rewardText;
    public TextMeshPro obstacleCollisionsText;

    [Header("Prefabs")]
    public GameObject goalPrefab;
    public GameObject obstaclePrefab;

    [Header("Debug")]
    public bool drawCollisionRadius = false;
    public bool drawTargetDistance = false;
    public bool drawAgentRays = false;
    
    [HideInInspector]
    public int numObstacles;
    [HideInInspector]
    public float obstacleSpeed;
    [HideInInspector]
    public float spawnRange;
    [HideInInspector]
    public float collisionRadius;
    [HideInInspector]
    public float targetDistance;
    [HideInInspector]
    public float targetSize;
    [HideInInspector]
    public float rayLength;
    [HideInInspector]
    public float minReward;
    [HideInInspector]
    public float winReward;
    [HideInInspector]
    public float collisionPenalty;
    [HideInInspector]
    public float timePenalty;

    [HideInInspector]
    public Bounds areaBounds;
    [HideInInspector]
    private ExplorationAcademy exAcademy;
    
    private GameObject goal;
    private List<GameObject> spawnedObstacles;
    private List<Tuple<Vector3, float>> occupiedPositions;
    private int spawnTries = 30;

    private Renderer areaRenderer;
    private GameObject areaObject;
    private Material areaMaterial;

    public delegate bool CustomCheckFunction(Vector3 pos);
    public int obstacleCollisions
    {
        get { return Int32.Parse(obstacleCollisionsText.text); }
    }

    private void Start()
    {
        exAcademy = FindObjectOfType<ExplorationAcademy>();

        if (!is3D)
        {
            areaRenderer = ground.GetComponent<Renderer>();
            areaMaterial = areaRenderer.material;
            areaBounds = ground.GetComponent<Collider>().bounds;
        } else
        {
            areaRenderer = ground.GetComponent<Renderer>();
            areaMaterial = areaRenderer.material;
            areaBounds = new Bounds(area3D.transform.position, area3D.transform.localScale);
        }
        occupiedPositions = new List<Tuple<Vector3, float>>();
    }

    public void UpdateScore(float reward)
    {
        rewardText.text = reward.ToString("0.00");
    }

    public void OnObstacleCollision()
    {
        obstacleCollisionsText.text = (obstacleCollisions + 1).ToString();
        exAcademy.totalCollisions++;
    }

    public override void ResetArea()
    {   
        obstacleCollisionsText.text = "0";
        if(occupiedPositions != null) {
            occupiedPositions.Clear();
        }
        ResetAgent();
        ResetGoal();
        ResetObstacles();
    }

    public void ResetAgent()
    {
        if(!isStructured)
        {
            SpawnObjectsDist(expAgent, spawnRange);
        }
        else
        {
            expAgent.transform.position = new Vector3(-15.5f, 0.5f, 11.8f);
        }
    }

    private void ResetGoal()
    {
        if (!isStructured)
        {
            if (goal == null)
            {
                goal = Instantiate(goalPrefab, transform);
                goal.SetActive(true);
            }

            SpawnObjectsDist(goal, spawnRange, (Vector3 spawnPos) =>
            {
                float cor = this.getColliderOccupationRadius(this.expAgent.GetComponent<Collider>(), this.expAgent);
                float dist = Vector3.Distance(spawnPos, this.expAgent.transform.position);
                return Mathf.Abs(dist - targetDistance) <= 5;
            });
        }
        
    }

    private void ResetObstacles()
    {
        if (spawnedObstacles == null)
        {
            spawnedObstacles = new List<GameObject>();
            for (int i = 0; i < numObstacles; i++)
            {
                GameObject obstacle = Instantiate(obstaclePrefab, transform);
                obstacle.SetActive(true);
                obstacle.name += "(" + i + ")";
                spawnedObstacles.Add(obstacle);
            }
        }

        if(numObstacles > spawnedObstacles.Count)
        {
            for (int i = spawnedObstacles.Count; i < numObstacles; i++)
            {
                GameObject obstacle = Instantiate(obstaclePrefab, transform);
                obstacle.SetActive(true);
                obstacle.name += "(" + i + ")";
                spawnedObstacles.Add(obstacle);
            }
        }

        foreach (GameObject obstacle in spawnedObstacles.ToArray())
        {
            obstacle.SetActive(true);
            SpawnObjectsDist(obstacle, spawnRange);
        }
    }

    private void SpawnObjectsDist(GameObject target, float range) => SpawnObjectsDist(target, range, (Vector3 pos) => true);

    private void SpawnObjectsDist(GameObject target, float range, CustomCheckFunction customCondToCheckLocation)
    {
        Collider targetCollider = target.GetComponent<Collider>();
        var spawnPos = Vector3.zero;
        bool foundLocation = false;

        var tries = spawnTries;

        for(; !foundLocation && tries > 0; tries--)
        {
            var randomPosX = 0.0f;
            var randomPosY = 0.0f;
            var randomPosZ = 0.0f;
            var bounds = areaBounds.extents;
            if(is3D)
            {
                if (target.name.Contains("Target"))
                {
                    // Find a random spot on the circumference at targetDistance
                    float angle = UnityEngine.Random.Range(0, Mathf.PI * 2);
                    var pos = new Vector3(Mathf.Sin(angle) * targetDistance, Mathf.Cos(angle) * targetDistance);

                    randomPosX = Mathf.Clamp(pos.x,
                                            (-bounds.x + targetCollider.bounds.extents.x),
                                            (bounds.x - targetCollider.bounds.extents.x));

                    randomPosY = Mathf.Clamp(pos.y,
                                            (-bounds.y + targetCollider.bounds.extents.y),
                                            (bounds.y - targetCollider.bounds.extents.y));

                    randomPosZ = Mathf.Clamp(pos.z,
                                            (-bounds.z + targetCollider.bounds.extents.z),
                                            (bounds.z - targetCollider.bounds.extents.z));
                }
                else
                {
                    randomPosX = UnityEngine.Random.Range(
                        (-bounds.x + targetCollider.bounds.extents.x),
                        (bounds.x - targetCollider.bounds.extents.x)) * range;
                    randomPosY = UnityEngine.Random.Range(
                        (-bounds.y + targetCollider.bounds.extents.y),
                        (bounds.y - targetCollider.bounds.extents.y)) * range;
                    randomPosZ = UnityEngine.Random.Range(
                        (-bounds.z + targetCollider.bounds.extents.z),
                        (bounds.z - targetCollider.bounds.extents.z)) * range;
                }

                spawnPos = new Vector3(randomPosX, randomPosY, randomPosZ) + areaBounds.center;
                foundLocation = this.checkLocation(spawnPos) && customCondToCheckLocation(spawnPos);
            } else
            {
                if (target.name.Contains("Target"))
                {
                    // Find a random spot on the circumference at targetDistance
                    float angle = UnityEngine.Random.Range(0, Mathf.PI * 2);
                    var pos = new Vector2(Mathf.Sin(angle) * targetDistance, Mathf.Cos(angle) * targetDistance);

                    randomPosX = Mathf.Clamp(pos.x,
                                            (-bounds.x + targetCollider.bounds.extents.x),
                                            (bounds.x - targetCollider.bounds.extents.x));

                    randomPosZ = Mathf.Clamp(pos.y,
                                            (-bounds.z + targetCollider.bounds.extents.z),
                                            (bounds.z - targetCollider.bounds.extents.z));

                }
                else
                {
                    randomPosX = UnityEngine.Random.Range(
                        (-bounds.x + targetCollider.bounds.extents.x),
                        (bounds.x - targetCollider.bounds.extents.x)) * range;
                    randomPosZ = UnityEngine.Random.Range(
                        (-bounds.z + targetCollider.bounds.extents.z),
                        (bounds.z - targetCollider.bounds.extents.z)) * range;
                }

                spawnPos = new Vector3(randomPosX, targetCollider.bounds.extents.y, randomPosZ) + areaBounds.center;
                foundLocation = this.checkLocation(spawnPos) && customCondToCheckLocation(spawnPos);
            }

        }

        if (foundLocation && is3D)
        {
            var orientation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), 0f));
            target.transform.position = spawnPos;
            target.transform.rotation = orientation;
            occupiedPositions.Add(new Tuple<Vector3, float>(target.transform.position, getColliderOccupationRadius(targetCollider, target)));
        }
        else if (foundLocation && !is3D)
        {
            var orientation = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f));

            target.transform.position = spawnPos;
            target.transform.rotation = orientation;
            occupiedPositions.Add(new Tuple<Vector3, float>(target.transform.position, getColliderOccupationRadius(targetCollider, target)));
        }
        else
        {
            if (target.name.Contains("Target") || target.name.Contains("Agent"))
            {
                UnityEngine.Debug.LogWarning("Couldn't spawn object: " + target.name + ", resetting area");
                ResetArea();
            }
            else
            {
                target.SetActive(false);
                UnityEngine.Debug.LogWarning("Couldn't spawn object: " + target.name + ", deactivating it for the current episode");
            }

        }

    }

    private float getColliderOccupationRadius(Collider targetCollider, GameObject target)
    {
        Vector3 colliderBounds = targetCollider.bounds.extents;
        colliderBounds.Scale(target.transform.localScale);
        return (float)Mathf.Max(colliderBounds.x, colliderBounds.y, colliderBounds.z);
    }

    private bool checkLocation(Vector3 spawnPos) => 
        !occupiedPositions.Exists(occupied =>
            Vector3.Distance(spawnPos, occupied.Item1) - occupied.Item2 - collisionRadius <= 0
        );

    public void SuccessResetArea()
    {
        // Color map
        StartCoroutine(this.SwapareaMaterial(success: true));
        // Increase hits
        exAcademy.totalTargetsHits++;
        ResetArea();
    }

    public void FailResetArea()
    {
        // Color map
        StartCoroutine(this.SwapareaMaterial(success: false));
        ResetAgent();
        ResetArea();
    }

    public IEnumerator SwapareaMaterial(bool success)
    {
        if (success)
        {
            areaRenderer.material = successMaterial;
        }
        else
        {
            areaRenderer.material = failureMaterial;
        }
        yield return new WaitForSeconds(0.5f);
        areaRenderer.material = areaMaterial;
    }

     private void OnDrawGizmos()
     {
        if (drawCollisionRadius)
        {
            if (spawnedObstacles != null)
            {
                foreach (GameObject obstacle in spawnedObstacles.ToArray())
                {
                    Gizmos.DrawWireSphere(obstacle.transform.position, collisionRadius);
                }
            }

            if (goal != null)
            {
                Gizmos.DrawWireSphere(goal.transform.position, collisionRadius);
            }

            if (expAgent != null)
            {
                Gizmos.DrawWireSphere(expAgent.transform.position, collisionRadius);
            }
        }

        if (drawTargetDistance)
        {
            if(goal != null && expAgent != null)
            {
                Gizmos.DrawLine(expAgent.transform.position, goal.transform.position);
            }
            
        }
    }

}
