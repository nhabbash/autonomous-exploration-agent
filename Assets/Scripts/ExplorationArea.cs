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

        collisionLayerMask = ~LayerMask.GetMask("LevelBoundaries");
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
        Collider[] hitColliders;
        Quaternion orientation;
        Vector3 randomLocalPosition;
        Rigidbody rb = target.GetComponent<Rigidbody>();
        do
        {
            orientation = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f));
            randomLocalPosition = new Vector3(UnityEngine.Random.Range(-range, range), 0f, UnityEngine.Random.Range(-range, range));
            randomLocalPosition.Scale(transform.localScale);

            hitColliders = Physics.OverlapSphere(randomLocalPosition, Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z));
        }
        while (hitColliders.Length > 1);
        if(rb != null)
        {
            //rb.velocity = Vector3.zero;
            //rb.angularVelocity = Vector3.zero;
        }
        target.transform.rotation = orientation;
        target.transform.localPosition = randomLocalPosition;
    }


    private bool CheckIfPositionIsOpen(Vector3 testPosition, float testRadius)
    {
        foreach (Tuple<Vector3, float> occupied in occupiedPositions)
        {
            Vector3 occupiedPosition = occupied.Item1;
            float occupiedRadius = occupied.Item2;
            if (Vector3.Distance(testPosition, occupiedPosition) - occupiedRadius <= testRadius)
            {
                return false;
            }
        }

        return true;
    }
}
