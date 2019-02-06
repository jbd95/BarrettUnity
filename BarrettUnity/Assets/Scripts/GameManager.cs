using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public Spawner[] spawners;
    public Transform spawnZone;
    public float spawnRate; //Interval
    public float spawnRadius = 15f;
    public int numWaves = 10;
    public int TurnCount = 10;
    public ROS_Websocket RosServer;

    public GameController controller;
    public Camera mainCamera;

    int wave = 1;

    int successfulBlocks = 0;
    int unsuccessfulBlocks = 0;
    int projectilesFiredCount = 0;
    double accuracy = 0;
    int ProjectileID = 0;

    public Text DisplaySuccessRate;
    public Text DisplayAccuracy;
    public Text DisplayRLScore;

    public float XMaxDistance = 0;
    public float YMaxDistance = 0;
    public float VelocityMultiplier = 5;

    public Pattern PatternType = Pattern.Line;
    public List<int> PatternParameters = new List<int>();

    public bool IsGameAlive = false;

    GameObject shield;

    public System.Diagnostics.Stopwatch GameTime;

    public List<ProjectileInfo> Projectiles;

    public float TimeInterval = 1f;

    public RLFunctions RL = new RLFunctions();

    public Pair GridCenter = new Pair(0, 0);

    void Start()
    {
        GameTime = new System.Diagnostics.Stopwatch();
        shield = GameObject.FindGameObjectWithTag("User");
        Projectiles = new List<ProjectileInfo>();
        Debug.Log(RL.GenerateRewardTable(10,10));
    }

    /*public void Begin()
    {
        //EndRound();
        IsGameAlive = true;
        StopCoroutine(SendBucketCoordinates());
        StartCoroutine(SendBucketCoordinates());
        if (RequestedDestinations.Count > 0)
        {
            StartCoroutine(RequestedSpawn());
            return;
        }
        switch (PatternType)
        {
            case Pattern.Line:
                if (PatternParameters.Count > 1)
                {
                    StartCoroutine(LineSpawn(PatternParameters[0], PatternParameters[1]));
                }
                else
                {
                    StartCoroutine(LineSpawn(1, 1));
                }
                break;
            case Pattern.Sine:
                if (PatternParameters.Count > 0)
                {
                    StartCoroutine(SineSpawn(PatternParameters[0]));
                }
                else
                {
                    StartCoroutine(SineSpawn(10));
                }
                break;
            case Pattern.Cosine:
                if (PatternParameters.Count > 0)
                {
                    StartCoroutine(CosineSpawn(PatternParameters[0]));
                }
                else
                {
                    StartCoroutine(CosineSpawn(9));
                }
                break;
            case Pattern.SemiCircle:
                if (PatternParameters.Count > 0)
                {
                    StartCoroutine(SemiCircleSpawn(PatternParameters[0]));
                }
                else
                {
                    StartCoroutine(SemiCircleSpawn(5));
                }
                break;
        }
    }*/

    public void BeginEpisode(ActionTable<int> decision_table)
    {
        IsGameAlive = true;
        StopCoroutine(SendBucketCoordinates());
        StartCoroutine(SendBucketCoordinates());

        StartCoroutine(ExecuteActionTable(decision_table));
    }

    public void BeginEpisode(List<string> RequestedDestinations)
    {
        IsGameAlive = true;
        StopCoroutine(SendBucketCoordinates());
        StartCoroutine(SendBucketCoordinates());

        StopCoroutine("ExecuteActionList");
        StartCoroutine(ExecuteActionList(RequestedDestinations));
    }

    IEnumerator ExecuteActionList(List<string> RequestedDestinations)
    {
        ResetStatistics();
        Projectiles.Clear();
        int count = 0;
        numWaves = RequestedDestinations.Count;
        TurnCount = RequestedDestinations.Count;

        while (count < TurnCount)
        {
            if (IsProjectileVisible() || controller.User_Position != GridCenter)
            {
                if (!IsProjectileVisible())
                {
                    controller.Arrow.Show();
                }
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            controller.Arrow.Hide();
            yield return new WaitForSeconds(TimeInterval / 2);
            if (RequestedDestinations.Count > 0)
            {
                Pair destination = new Pair(RequestedDestinations[0]);
                Transform temp = Instantiate(spawners[0].obj, spawnZone).transform;
                temp = controller.GetBoxCoordinates(temp, destination.x, destination.y);
                controller.GetFieldAtBoxCoordinates(destination).IncrementVisitedValue();
                RequestedDestinations.Remove(RequestedDestinations[0]);
                count++;
            }
            yield return new WaitForSeconds(TimeInterval);
        }
    }

    IEnumerator ExecuteActionTable(ActionTable<int> decision_table)
    {
        ResetStatistics();
        Projectiles.Clear();
        int count = 0;
        numWaves = TurnCount;

        while (count < TurnCount)
        {
            //if (IsGameGoing() || !(controller.User_Position.IsValid()))

            if (IsProjectileVisible() || controller.User_Position != GridCenter)
            {
                if(!IsProjectileVisible())
                {
                    controller.SetGridCenterVisibility(true);
                }
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            controller.SetGridCenterVisibility(false);
            yield return new WaitForSeconds(TimeInterval / 2);
            Pair destination = decision_table.GetNextPosition(controller.User_Position);
            Transform temp = Instantiate(spawners[0].obj, spawnZone).transform;
            temp = controller.GetBoxCoordinates(temp, destination.x, destination.y);
            count++;
            yield return new WaitForSeconds(TimeInterval);
        }
    }

    IEnumerator SendBucketCoordinates()
    {
        GameTime.Start();
        while (IsGameAlive || IsGameGoing())
        {
            foreach (ProjectileInfo current in Projectiles)
            {
                if (!current.IsFinished)
                {
                    double elapsed_time = (double)GameTime.ElapsedMilliseconds / 1000;

                    current.Position = current.Projectile.Position;
                    if (current.IsCaught == Caught.Yes)
                    {
                        //RosServer.SendScore(GetSuccessfulHitAccuracy() + "");
                        RosServer.SendBucketCoordinates(current, elapsed_time, current.Position);
                    }
                    else
                    {
                        RosServer.SendBucketCoordinates(current, elapsed_time, controller.User_Position);
                    }
                    if (current.IsCaught != Caught.Undetermined)
                    {
                        current.IsFinished = true;
                        /*Debug.Log("Reward is: " + RL.Reward(Projectiles[Projectiles.Count - 1], this));
                        RosServer.SendRLValue("Reward:" + RL.Reward(Projectiles[Projectiles.Count - 1], this));
                        Debug.Log("Value is: " + RL.Value(Projectiles, this));
                        RosServer.SendRLValue("Value:" + RL.Value(Projectiles, this));*/
                    }
                }
            }
            yield return new WaitForSeconds(0.3f);
        }
        GameTime.Stop();
        //Projectiles = new List<ProjectileInfo>();
        RosServer.SendRLValue("episode finished");
    }

    private void SendRLRoundResults()
    {
        foreach(ProjectileInfo current in Projectiles)
        {
            RosServer.SendScore("RL Result:" + current.StartPosition.ToString() + ":" + RL.Reward(current, this));
        }
        RosServer.SendScore("RL Score:" + RL.Score(Projectiles, this));
        RosServer.SendRLValue("Value:" + RL.Score(Projectiles, this));
    }

    private bool IsGameGoing()
    {
        foreach (ProjectileInfo current in Projectiles)
        {
            if (!current.IsFinished)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsProjectileVisible()
    {
        foreach(ProjectileInfo current in Projectiles)
        {
            if(current.IsCaught == Caught.Undetermined)
            {
                return true;
            }
        }
        return false;
    }

    public ProjectileInfo GetVisibleProjectile()
    {
        foreach (ProjectileInfo current in Projectiles)
        {
            if (current.IsCaught == Caught.Undetermined)
            {
                return current;
            }
        }
        return null;
    }

    public class ProjectileInfo
    {
        public double TimeLaunched;
        public double TimeLanded;
        public Caught IsCaught;
        public Projectile Projectile;
        public int ID;
        public bool IsFinished;
        public Pair Position;
        public Pair StartPosition;
        public Pair UserStartPosition;
        public string LastSubmitted;

        public ProjectileInfo(double _time, GameObject _gameobject, Pair _startposition, Pair _userposition)
        {
            TimeLaunched = _time;
            Projectile = _gameobject.GetComponent<Projectile>();
            IsCaught = Caught.Undetermined;
            IsFinished = false;
            LastSubmitted = "";
            StartPosition = _startposition;
            UserStartPosition = _userposition;
        }

        public override string ToString()
        {
            return "ID: " + ID + "   CAUGHT: " + IsCaught.ToString() + "   POS: " + Position + "   User Start Position: " + UserStartPosition;
        }
        public bool IsCompleted()
        {
            return IsCaught != Caught.Undetermined;
        }
    }

    public enum Caught
    {
        Undetermined = -1,
        No = 0,
        Yes = 1
    }

    /*IEnumerator RequestedSpawn()
    {
        int i = 0;
        numWaves = RequestedDestinations.Count;
        while (true)
        {
            if (i >= RequestedDestinations.Count)
            {
                break;
            }

            string[] coordinates = RequestedDestinations[i].Split('(')[1].Split(')')[0].Split(',');
            int x = Convert.ToInt32(coordinates[0]);
            int y = Convert.ToInt32(coordinates[1]);
            Transform temp = Instantiate(spawners[0].obj, spawnZone).transform;
            temp = controller.GetBoxCoordinates(temp, x, y);
            temp.GetComponent<Projectile>().VelocityPercentage = VelocityMultiplier;
            i++;
            yield return new WaitForSeconds(TimeInterval);
        }

        RequestedDestinations.Clear();
    }*/

    void EndRound()
    {
        IsGameAlive = false;
        SendRLRoundResults();
        RosServer.SendScore("Episode Finished");
        Projectiles.Clear();
        //RosServer.SendRLValue("RLvalue: " + RL.Value(Projectiles));
    }

    public void SendtoBox(int x, int y)
    {
        Transform temp = Instantiate(spawners[0].obj, spawnZone).transform;
        temp = controller.GetBoxCoordinates(temp, x, y);
    }

    IEnumerator ContinuousSpawn()
    {
        while (true)
        {
            foreach (Spawner spawner in spawners)
            {

                // for (int i = 0; i < Mathf.RoundToInt(spawner.spawnRate.Evaluate(wave)); i++) {
                for (int i = 0; i < spawnRate; i++)
                {
                    Transform temp = Instantiate(spawner.obj, spawnZone).transform;
                    temp.localPosition = Vector3.right * UnityEngine.Random.Range(-spawnRadius, spawnRadius);
                }
            }
            if (wave < numWaves)
            {
                wave++;
            }
            else
            {
                wave = 1;
                //EndGame();
                break;
            }
            yield return new WaitForSeconds(3f);
        }
    }

    IEnumerator LineSpawn(int xslope, int yslope)
    {
        float xmax = (controller.XValues.Count - 1) / (float)xslope;
        float ymax = (controller.YValues.Count - 1) / (float)yslope;

        if (xmax > ymax)
        {
            xmax = ymax;
        }

        if (xmax != (int)xmax)
        {
            xmax = (int)xmax + 1;
        }

        numWaves = (int)xmax;

        for (int x = 0; x < xmax; x++)
        {
            Transform temp = Instantiate(spawners[0].obj, spawnZone).transform;
            temp = controller.GetBoxCoordinates(temp, (x * xslope), (x * yslope));
            yield return new WaitForSeconds(TimeInterval);
        }
    }

    IEnumerator SineSpawn(int amplitude)
    {
        if (amplitude > (controller.YValues.Count - 1))
        {
            throw new System.Exception("The Amplitude cannot be bigger than the amount of intervals");
        }

        numWaves = controller.XValues.Count - 1;

        for (int x = 0; x < controller.XValues.Count - 1; x++)
        {
            Transform temp = Instantiate(spawners[0].obj, spawnZone).transform;
            temp = controller.GetBoxCoordinates(temp, x, Mathf.Abs((int)(amplitude * Mathf.Sin(Mathf.Deg2Rad * x * (180 / (controller.XValues.Count - 2))))));
            yield return new WaitForSeconds(TimeInterval);
        }
    }

    IEnumerator CosineSpawn(int amplitude)
    {
        if (amplitude > (controller.YValues.Count - 2))
        {
            throw new System.Exception("The Amplitude cannot be bigger than the amount of intervals");
        }

        numWaves = controller.XValues.Count - 1;

        for (int x = 0; x < controller.XValues.Count - 1; x++)
        {
            Transform temp = Instantiate(spawners[0].obj, spawnZone).transform;


            if ((x * 180 / (controller.XValues.Count - 2)) <= 90)
            {
                temp = controller.GetBoxCoordinates(temp, x, Mathf.Abs((int)(amplitude * Mathf.Cos(Mathf.Deg2Rad * x * (180 / (controller.XValues.Count - 2))))));
            }
            else
            {
                temp = controller.GetBoxCoordinates(temp, x, Mathf.Abs((int)(amplitude * Mathf.Cos(Mathf.Deg2Rad * (180 / (controller.XValues.Count - 2)) * (controller.XValues.Count - 2 - x)))));
            }
            yield return new WaitForSeconds(TimeInterval);
        }
    }

    IEnumerator SemiCircleSpawn(int radius)
    {
        if (radius > (controller.XValues.Count - 1) || radius > (controller.YValues.Count - 1))
        {
            throw new System.Exception("The radius cannot be bigger than the amount of intervals");
        }

        numWaves = controller.XValues.Count - 1;

        for (int x = (-1) * radius; x <= (radius * 2); x++)
        {
            Transform temp = Instantiate(spawners[0].obj, spawnZone).transform;
            temp = controller.GetBoxCoordinates(temp, radius + x, 2 * (int)(Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(x, 2))));
            yield return new WaitForSeconds(TimeInterval);
        }
    }

    public enum Pattern
    {
        Line,
        Sine,
        Cosine,
        SemiCircle,
        None
    }

    void EndGame()
    {
        wave = 1;

    }

    [System.Serializable]
    public class Spawner
    {
        public GameObject obj;
        public AnimationCurve spawnRate;
    }

    public void ResizeShield(float value)
    {
        shield.transform.localScale = new Vector3(value, value, value);
    }

    public void SuccessfulProjectileBlock(double time, GameObject projectile)
    {
        ProjectileLanded(time, projectile, Caught.Yes);
        successfulBlocks++;
        DisplaySuccessRate.text = successfulBlocks + "/" + projectilesFiredCount;
        DisplayAccuracy.text = "" + ((int)GetSuccessfulHitAccuracy()) + "%";
        DisplayRLScore.text = "" + ((int)RL.Score(Projectiles, this));

        if (numWaves == (successfulBlocks + unsuccessfulBlocks))
        {
            EndRound();
        }
    }

    public void UnsuccessfulProjectileBlock(double time, GameObject projectile)
    {
        ProjectileLanded(time, projectile, Caught.No);
        unsuccessfulBlocks++;
        DisplaySuccessRate.text = successfulBlocks + "/" + projectilesFiredCount;
        DisplayAccuracy.text = "" + ((int)GetSuccessfulHitAccuracy()) + "%";
        DisplayRLScore.text = "" + ((int)RL.Score(Projectiles, this));

        if (numWaves == (successfulBlocks + unsuccessfulBlocks))
        {
            EndRound();
        }
    }

    public void ProjectileLaunched(double time, GameObject projectile, Pair startpos)
    {
        ProjectileInfo info = new ProjectileInfo(time, projectile, startpos, controller.User_Position);
        info.ID = ProjectileID;
        info.Position = info.StartPosition;
        ProjectileID++;
        info.Projectile.ID = info.ID;
        Projectiles.Add(info);
        projectilesFiredCount++;
        DisplaySuccessRate.text = successfulBlocks + "/" + projectilesFiredCount;
    }

    public void ProjectileLanded(double time, GameObject projectile, Caught caught)
    {
        foreach (ProjectileInfo current in Projectiles)
        {
            if (projectile.GetComponent<Projectile>().ID == current.ID)
            {
                current.IsCaught = caught;
                current.TimeLanded = time;
            }
        }
    }

    public void TrackAccuracy(Vector3 hitPoint)
    {
        double distance = Mathf.Sqrt(Mathf.Pow(hitPoint.z, 2) + Mathf.Pow((Mathf.Abs(hitPoint.y)), 2));

        if (distance >= 1)
        {
            distance = 0.0;
        }
        else
        {

            distance = distance * 100;
            distance = 100 - distance;
        }

        if ((int)distance > 50)
        {
            distance = 100;
        }

        accuracy += distance;

        /* Debug.Log(accuracy / successfulBlocks);
         RosServer.GetComponent<ROS_Websocket>().SendScore((accuracy / successfulBlocks) + "");*/

    }

    public double GetSuccessfulHitAccuracy()
    {
        if (projectilesFiredCount == 0)
        {
            return 0;
        }
        return (successfulBlocks / (double)projectilesFiredCount) * 100;
    }
    public double GetTotalHitAccuracy()
    {
        if (projectilesFiredCount == 0)
        {
            return 0;
        }
        return accuracy / projectilesFiredCount;
    }

    Vector3 VectorAbs(Vector3 input)
    {
        input.x = Mathf.Abs(input.x);
        input.y = Mathf.Abs(input.y);
        input.z = Mathf.Abs(input.z);

        return input;
    }

    public void ResetStatistics()
    {
        ResetLabels();
        successfulBlocks = 0;
        unsuccessfulBlocks = 0;
        projectilesFiredCount = 0;
        accuracy = 0;
        ProjectileID = 0;
    }

    public void ResetLabels()
    {
        DisplaySuccessRate.text = "0/0";
        DisplayAccuracy.text = "0%";
    }
}
