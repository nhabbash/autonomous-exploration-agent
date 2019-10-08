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

    public override void ResetArea()
    {
        occupiedPositions = new List<Tuple<Vector3, float>>();
        ResetAgent();
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
  
    private void ResetGoal()
    {
        if (goal != null)
        {
            Destroy(goal);
        }

        goal = Instantiate(goalPrefab, transform);
        SpawnObjects(goal, spawnRange);
    }

    private void ResetObstacles()
    {
        if (spawnedObstacles != null)
        { 
            foreach (GameObject obstacle in spawnedObstacles.ToArray())
            {
                Destroy(obstacle);
            }
        }

        spawnedObstacles = new List<GameObject>();

            for (int i = 0; i<numObstacles; i++)
            {
                GameObject obstacle = Instantiate(obstaclePrefab, transform);
                SpawnObjects(obstacle, spawnRange);
                spawnedObstacles.Add(obstacle);
            }
    }

    private void SpawnObjects(GameObject target, float range)
    {
        Collider c = target.GetComponent<Collider>();
        Debug.Log(c.enabled);
        target.transform.rotation = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f));

        Vector3 randomLocalPosition = new Vector3(UnityEngine.Random.Range(-range, range), 0.1f, UnityEngine.Random.Range(-range, range));
        randomLocalPosition.Scale(transform.localScale);

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
