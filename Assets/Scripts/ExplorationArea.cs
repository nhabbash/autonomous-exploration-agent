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
    public Bounds areaBounds;

    private GameObject goal;
    private List<GameObject> spawnedObstacles;
    private List<Tuple<Vector3, float>> occupiedPositions;

    private Renderer groundRenderer;
    private Material groundMaterial;

    private int collisionLayerMask;

    private void Start()
    {
        groundRenderer = ground.GetComponent<Renderer>();
        groundMaterial = groundRenderer.material;
        // Get the ground's bounds
        areaBounds = ground.GetComponent<Collider>().bounds;

        collisionLayerMask = ~LayerMask.GetMask("Ground");
    }
    public void SuccessResetArea()
    {
        // Color map
        StartCoroutine(this.SwapGroundMaterial(success: true));
        this.ResetArea();
    }

    public void FailResetArea()
    {
        // Color map
        StartCoroutine(this.SwapGroundMaterial(success: false));
        ResetAgent();
        this.ResetArea();
    }

    public override void ResetArea()
    {
        occupiedPositions = new List<Tuple<Vector3, float>>();
        ResetGoal();
        ResetObstacles();
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

    public void UpdateScore(float reward)
    {
        rewardText.text = reward.ToString("0.00");
    }

    private void ResetAgent()
    {
        SpawnObjects(expAgent, spawnRange);
    }
  
    public void ResetGoal()
    {
        if (goal == null)
        {
            goal = Instantiate(goalPrefab, transform);
        }
        SpawnObjects(goal, spawnRange);
    }

    private void ResetObstacles()
    {
        if (spawnedObstacles == null)
        {
            spawnedObstacles = new List<GameObject>();
            for (int i = 0; i < numObstacles; i++)
            {
                GameObject obstacle = Instantiate(obstaclePrefab, transform);
                spawnedObstacles.Add(obstacle);
            }
        }
        foreach (GameObject obstacle in spawnedObstacles.ToArray())
        {
            SpawnObjects(obstacle, spawnRange);
        }
    }

    private void SpawnObjects(GameObject target, float range)
    {
        Collider targetCollider = target.GetComponent<Collider>();
        var spawnPos = Vector3.zero;
        var foundLocation = false;
        int tries = 20;

        var orientation = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f));
        var colliderRadius = Mathf.Max(targetCollider.bounds.extents.x, targetCollider.bounds.extents.y, targetCollider.bounds.extents.z);

        do
        {
            var randomPosX = UnityEngine.Random.Range(-areaBounds.extents.x * range,
                areaBounds.extents.x * range);

            var randomPosZ = UnityEngine.Random.Range(-areaBounds.extents.z * range,
                areaBounds.extents.z * range);
            spawnPos = ground.transform.position + new Vector3(randomPosX, targetCollider.bounds.extents.y, randomPosZ);

            //foundLocation = !Physics.CheckBox(spawnPos, targetCollider.bounds.extents, orientation, collisionLayerMask);
            foundLocation = !Physics.CheckSphere(spawnPos, colliderRadius, collisionLayerMask);
            tries--;
        }
        while (!foundLocation || tries > 0);

        if (foundLocation)
        {
            //Debug
            /*
            Collider[] hitColliders = Physics.OverlapBox(spawnPos, targetCollider.bounds.extents, orientation, collisionLayerMask);
            Debug.Log(hitColliders.Length);
            if (Physics.CheckBox(spawnPos, targetCollider.bounds.extents, orientation, collisionLayerMask))
            {
                var targetRenderer = target.GetComponent<Renderer>();
                var targetMaterial = targetRenderer.material;
                targetMaterial = failureMaterial;
            }
            
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = spawnPos;
            sphere.transform.localScale = target.transform.localScale;
            SphereCollider sphereCollider = sphere.GetComponent<SphereCollider>();
            sphereCollider.radius = colliderRadius;
            */

            target.transform.position = spawnPos;
            target.transform.rotation = orientation;
        }
        
    }

}
