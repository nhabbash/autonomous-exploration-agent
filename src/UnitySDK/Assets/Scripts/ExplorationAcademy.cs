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

    [Header("Debug")]
    public bool drawDebug = false;
    public bool drawCollisionRadius = false;
    public bool drawTargetDistance = false;
    public bool drawAgentRays = false;

    [HideInInspector]
    public int totalTargetsHits;
    [HideInInspector]
    public int totalCollisions;
    [HideInInspector]
    public Stopwatch stopwatch;
    [HideInInspector]
    public Camera[] cams;

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
    public void CustomAcademyReset(string parameters)
    {
        if (areas == null)
        {
            areas = GameObject.FindObjectsOfType<ExplorationArea>();
        }

        string[] paramss = parameters.Split('-');
        foreach (ExplorationArea area in areas)
        {
            area.numObstacles = int.Parse(paramss[0]);
            area.spawnRange = float.Parse(paramss[1]);
            area.collisionRadius = float.Parse(paramss[2]);
            area.targetDistance = float.Parse(paramss[3]);
            area.minReward = resetParameters["min_reward"];
            area.winReward = resetParameters["win_reward"];
            area.collisionPenalty = float.Parse(paramss[4]);
            area.timePenalty = resetParameters["time_penalty"];
            area.ResetArea();
        }
    }

    public void changeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void activateDraw(string activateString)
    {
        bool activate = activateString.Equals("true");
        this.drawDebug = activate;
    }

    public void switchCam()
    {
        var cams = this.cams;
        for (var i = 0; i < cams.Length; i++)
        {
            cams[i].enabled = !cams[i].enabled;
        }
    }

    private void Start()
    {

        this.cams = Camera.allCameras;
        cams[0].enabled = true;
        cams[1].enabled = false;
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

        // Draws only when there's 1 area to avoid getting too computationally intensive
        if (drawDebug && areas.Length == 1)
        {
            var agent = areas[0].expAgent.GetComponent<ExplorationAgent>();
            var is3D = areas[0].is3D ? true : false;
            float[] offsets3D = { 28.29f, 15.32f, -28.29f, -15.32f };
            int i = 0;

            if (is3D)
            {
                for (i = 0; i < agent.rayAngles.Length; i++)
                {
                    for (int j = 1; j <= offsets3D.Length; j++)
                    {
                        var coord = RayPerception3D.PolarToCartesian(agent.rayDistance, agent.rayAngles[i]);
                        coord.y = offsets3D[j-1];
                        var endPosition = agent.transform.TransformPoint(coord);
                        var direction = agent.transform.TransformDirection(coord);

                        agent.rayRenderer[i*j].SetPosition(0, agent.transform.position);
                        if (Physics.Raycast(agent.transform.position, direction, out RaycastHit hit))
                            if (hit.collider && Vector3.Distance(hit.point, agent.transform.position) <= agent.rayDistance)
                                agent.rayRenderer[i * j].SetPosition(1, hit.point);
                            else
                                agent.rayRenderer[i * j].SetPosition(1, endPosition);
                    }
                }
            }
            else
            {
                foreach (var angle in agent.rayAngles)
                {
                    var coord = RayPerception3D.PolarToCartesian(agent.rayDistance, angle);
                    var endPosition = agent.transform.TransformPoint(coord);
                    var direction = agent.transform.TransformDirection(coord);

                    agent.rayRenderer[i].SetPosition(0, agent.transform.position);
                    if (Physics.Raycast(agent.transform.position, direction, out RaycastHit hit))
                        if (hit.collider && Vector3.Distance(hit.point, agent.transform.position) <= agent.rayDistance)
                            agent.rayRenderer[i].SetPosition(1, hit.point);
                        else
                            agent.rayRenderer[i].SetPosition(1, endPosition);
                    i++;
                }


            }

        }
    }

    /*void OnGUI()
    {
        Monitor.Log("Total obstacle collisions", totalCollisions.ToString());
        // Monitor.Log("Reward"; reward / 5f);
        Monitor.Log("Total target hits", totalTargetsHits.ToString());
        if (performanceRun)
        {
            Monitor.Log("Time", stopwatch.Elapsed.TotalSeconds.ToString());
        }
    }*/

}