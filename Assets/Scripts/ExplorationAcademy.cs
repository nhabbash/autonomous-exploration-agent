using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationAcademy : Academy {

    private ExplorationArea[] areas;
    public override void InitializeAcademy()
    {
        Monitor.SetActive(true);
    }
    public override void AcademyReset()
    {
        if (areas == null)
        {
            areas = GameObject.FindObjectsOfType<ExplorationArea>();
        }

        foreach (ExplorationArea area in areas)
        {
            area.numObstacles = (int)resetParameters["num_obstacles"];
            area.spawnRange = resetParameters["spawn_range"];
            area.collisionRadius = resetParameters["collision_radius"];
            area.pathMinimumDistance = resetParameters["path_minimum_distance"];
            area.ResetArea();
        }
    }

}