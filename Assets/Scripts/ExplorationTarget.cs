using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationTarget : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Protagonist"))
        {
            
        }
        else if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("LevelBoundaries"))
        {
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
