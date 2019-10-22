using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationTarget : MonoBehaviour
{
    private ExplorationArea exArea;
    void Start()
    {
        exArea = transform.parent.GetComponent<ExplorationArea>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Protagonist"))
        {
            
        }
        else if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("LevelBoundaries"))
        {
            if (exArea)
            {
                exArea.ResetGoal();
            }
        }
    }

    void Update()
    {
        
    }
}
