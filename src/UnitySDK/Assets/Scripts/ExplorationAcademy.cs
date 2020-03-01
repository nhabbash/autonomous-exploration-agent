using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class ExplorationAcademy : Academy {

    private ExplorationArea[] areas;

    [Header("Settings")]
    public bool performanceRun;
    public float duration;
    public string experimentName;
    public string logFile;
    

    [HideInInspector]
    public int totalTargetsHits;
    [HideInInspector]
    public int totalCollisions;
    [HideInInspector]
    public Stopwatch stopwatch;

    public override void InitializeAcademy()
    {
        Monitor.SetActive(true);

        // Performance metric
        if (performanceRun)
        {
            totalTargetsHits = 0;
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }
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

    private void Update()
    {
        if (performanceRun)
        {
            TimeSpan timeSpan = stopwatch.Elapsed;

            if (timeSpan.TotalMinutes > duration)
            {
                float agents = areas.Length;
                float tpm = (totalTargetsHits / agents) / duration;
                float cpm = (totalCollisions / agents) / duration;

                DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory());
                string path = d.Parent.Parent.FullName;
                bool exists = File.Exists(path + logFile);
                StreamWriter writer = new StreamWriter(path + logFile, true);
                if (!exists)
                    writer.WriteLine("experiment;targets;obstacles;duration;agents;tpm;cpm");
                writer.WriteLine(experimentName + ";" + totalTargetsHits.ToString() + ";" + totalCollisions.ToString() + ";" + duration.ToString() + ";" +
                                 agents.ToString() + ";" + tpm + ";" + cpm);
                writer.Close();
                // Stops the editor
                //UnityEditor.EditorApplication.isPlaying = false;
                UnityEngine.Debug.Log("Performance metrics per " + duration + " minutes");
                UnityEngine.Debug.Log("Total targets: " + totalTargetsHits.ToString());
                UnityEngine.Debug.Log("Total obstacles: " + totalCollisions.ToString());
                UnityEngine.Debug.Log("TPM: " + tpm);
                UnityEngine.Debug.Log("CPM: " + cpm);
            }
        }

    }

    public void changeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    void OnGUI()
    {
        Monitor.Log("Total obstacle collisions", totalCollisions.ToString());
        // Monitor.Log("Reward"; reward / 5f);
        Monitor.Log("Total target hits", totalTargetsHits.ToString());
        if (performanceRun)
        {
            Monitor.Log("Time", stopwatch.Elapsed.TotalSeconds.ToString());
        }
    }

}