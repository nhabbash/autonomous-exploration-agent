using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MLAgents;

public class ExplorationArea : Area
{
    [Header("Exploration Area Objects")]
    public GameObject expAgent;
    public GameObject ground;
    public Material successMaterial;
    public Material failureMaterial;
    public TextMeshPro rewardText;

    [Header("Prefabs")]
    public GameObject goalPrefab;
    public GameObject obstaclePrefab;

    [HideInInspector]
    public int numObstacles;
    [HideInInspector]
    public float obstacleSpeed;
    [HideInInspector]
    public float spawnRange;
    [HideInInspector]
    public float collisionRadius;
    [HideInInspector]
    public float pathMinimumDistance;

    [HideInInspector]
    public Bounds areaBounds;

    private GameObject goal;
    private List<GameObject> spawnedObstacles;
    private List<Tuple<Vector3, float>> occupiedPositions;

    private Renderer groundRenderer;
    private Material groundMaterial;
    public int collisionLayerMask;

    public delegate bool CustomCheckFunction(Vector3 pos);

    private void Start()
    {
        groundRenderer = ground.GetComponent<Renderer>();
        groundMaterial = groundRenderer.material;
        areaBounds = ground.GetComponent<Collider>().bounds;
        collisionLayerMask = ~LayerMask.GetMask("Ground");
        occupiedPositions = new List<Tuple<Vector3, float>>();
    }

    public void UpdateScore(float reward)
    {
        rewardText.text = reward.ToString("0.00");
    }
    public override void ResetArea()
    {
        occupiedPositions.Clear();
        ResetAgent();
        ResetGoal();
        ResetObstacles();
    }

    public void ResetAgent()
    {
        SpawnObjectsDist(expAgent, spawnRange);
    }

    private void ResetGoal()
    {
        if (goal == null)
        {
            goal = Instantiate(goalPrefab, transform);
            goal.SetActive(true);
        }
        SpawnObjectsDist(goal, spawnRange, (Vector3 spawnPos) =>
            {
                float cor = this.getColliderOccupationRadius(this.expAgent.GetComponent<Collider>(), this.expAgent);
                return Vector3.Distance(spawnPos, this.expAgent.transform.position) - cor - collisionRadius <= pathMinimumDistance;
            });
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

        foreach (GameObject obstacle in spawnedObstacles.ToArray())
        {
            SpawnObjectsDist(obstacle, spawnRange);
        }
    }

    private void SpawnObjectsDist(GameObject target, float range, int tries = 10) => SpawnObjectsDist(target, range, (Vector3 pos) => true, tries);

    private void SpawnObjectsDist(GameObject target, float range, CustomCheckFunction customCondToCheckLocation, int tries = 10)
    {
        Collider targetCollider = target.GetComponent<Collider>();
        var spawnPos = Vector3.zero;
        bool foundLocation = false;

        for(; !foundLocation && tries > 0; tries--)
        {
            var randomPosX = 0.0f;
            var randomPosZ = 0.0f;

            if (target.name.Contains("Target"))
            {
                randomPosX = Mathf.Clamp(
                    UnityEngine.Random.Range(
                    (expAgent.transform.position.x - pathMinimumDistance),
                    (expAgent.transform.position.x + pathMinimumDistance)),
                    (-areaBounds.extents.x + targetCollider.bounds.extents.x),
                    (areaBounds.extents.x - targetCollider.bounds.extents.x)
                    );
                randomPosZ = Mathf.Clamp(
                    UnityEngine.Random.Range(
                    (expAgent.transform.position.z - pathMinimumDistance),
                    (expAgent.transform.position.z + pathMinimumDistance)),
                    (-areaBounds.extents.z + targetCollider.bounds.extents.z),
                    (areaBounds.extents.z - targetCollider.bounds.extents.z)
                    );
            }else{ 
                randomPosX = UnityEngine.Random.Range(
                    (-areaBounds.extents.x + targetCollider.bounds.extents.x),
                    (areaBounds.extents.x - targetCollider.bounds.extents.x)) * range;
                randomPosZ = UnityEngine.Random.Range(
                    (-areaBounds.extents.z + targetCollider.bounds.extents.z),
                    (areaBounds.extents.z - targetCollider.bounds.extents.z)) * range;
            }

            spawnPos = ground.transform.position + new Vector3(randomPosX, targetCollider.bounds.extents.y, randomPosZ);
            foundLocation = this.checkLocation(spawnPos) && customCondToCheckLocation(spawnPos);
        }

        if (foundLocation)
        {
            var orientation = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f));

            target.transform.position = spawnPos;
            target.transform.rotation = orientation;
            occupiedPositions.Add(new Tuple<Vector3, float>(target.transform.position, getColliderOccupationRadius(targetCollider, target)));
        }
        else
        {
            target.SetActive(false);
            Debug.LogWarning("Couldn't spawn object: " + target.name + ", deactivating it for the current episode");
        }

    }

    private float getColliderOccupationRadius(Collider targetCollider, GameObject target)
    {
        Vector3 colliderBounds = targetCollider.bounds.extents;
        colliderBounds.Scale(target.transform.localScale);
        return (float)Mathf.Max(colliderBounds.x, colliderBounds.y, colliderBounds.z);
    }

    private bool checkLocation(Vector3 spawnPos) => !occupiedPositions
        .Exists(occupied =>
            Vector3.Distance(spawnPos, occupied.Item1) - occupied.Item2 - collisionRadius <= 0
        );

    public void SuccessResetArea()
    {
        // Color map
        StartCoroutine(this.SwapGroundMaterial(success: true));
        ResetArea();
    }

    public void FailResetArea()
    {
        // Color map
        StartCoroutine(this.SwapGroundMaterial(success: false));
        ResetAgent();
        ResetArea();
    }

    public IEnumerator SwapGroundMaterial(bool success)
    {
        if (success)
        {
            groundRenderer.material = successMaterial;
        }
        else
        {
            groundRenderer.material = failureMaterial;
        }
        yield return new WaitForSeconds(0.5f);
        groundRenderer.material = groundMaterial;
    }

     /*private void OnDrawGizmos()
     {
         foreach (GameObject obstacle in spawnedObstacles.ToArray())
         {
             Gizmos.DrawWireSphere(obstacle.transform.position, collisionRadius);
         }
    }*/
}
