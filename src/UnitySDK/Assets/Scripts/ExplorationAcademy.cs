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
            area.targetDistance = resetParameters["target_distance"];
            area.minReward = resetParameters["min_reward"];
            area.winReward = resetParameters["win_reward"];
            area.collisionPenalty = resetParameters["collision_penalty"];
            area.timePenalty = resetParameters["time_penalty"];
            area.ResetArea();

        }
    }

}