using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Manager : MonoBehaviour
{
    public GameObject UIContainer;
    public Camera MainCamera;
    public float TimeLeft;
    public Text TimerText;
    public Text UserText;
    public GameObject GameOverText;
    [SerializeField] private Recorder DataSaver;
    [SerializeField] private ArrowAlignment Arrow;
    [SerializeField] private Forward_Kinematics fk;
    [SerializeField] private GameMode CurrentMode;
    [SerializeField] private float Radius;
    [SerializeField] private int TargetCount;
    [SerializeField] private Vector3 StartingPos;
    [SerializeField] private Text Instructions;
    [SerializeField] private string CurrentGoal;
    [SerializeField] private GameObject ParentForTargets;
    [SerializeField] private List<GameObject> Targets;
    [SerializeField] private int RoundScore;
    [SerializeField] private List<int> ScoreHistory;
    [SerializeField] private bool IsRoundActive = false;
    [SerializeField] private bool IsGameActive = false;
    [SerializeField] private int InstructionCount;

    private RoundData CurrentRoundData;
    private List<string> RoundDataHistory;
    private GameObject Player;
    public string CurrentUser;

    public int RoundNumber = 1;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Arrow.Hide();
        TimeLeft = TimeLeft * 60;
        ShowTime();
        RoundDataHistory = new List<string>();
        CurrentRoundData = new RoundData();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGameActive)
        {
            //Arrow.Align();
            if (IsRoundActive)
            {


                Arrow.Hide();

                bool roundOver = true;
                for (int i = 0; i < ParentForTargets.transform.childCount; i++)
                {
                    if (ParentForTargets.transform.GetChild(i).name == CurrentGoal)
                    {
                        roundOver = false;
                    }
                }
                if (roundOver)
                {
                    EndRound();
                }
            }
            else
            {
                if (!AtHomePosition() && !ProjectilesVisible())
                {
                    Arrow.Show();
                }
                else
                {
                    Arrow.Hide();
                    StartRound();
                }
            }
            if (TimeLeft <= 0)
            {
                EndGame();
            }
            TimeLeft -= Time.deltaTime;
            ShowTime();
        }
    }


    private void ShowTime()
    {
        TimerText.text = "Time Left: " + (int)(TimeLeft / 60) + ":" + ((int)(TimeLeft % 60)).ToString("00");
    }

    private bool AtHomePosition()
    {
        //return Mathf.Abs(Vector3.Distance(Abs(Player.transform.position), Abs(Arrow.HomePosition))) < 0.6;
        return Mathf.Abs(Vector3.Distance(Abs(Player.transform.position), Abs(Arrow.HomePosition))) < 0.8;
    }

    private Vector3 Abs(Vector3 input)
    {
        input.x = Mathf.Abs(input.x);
        input.y = Mathf.Abs(input.y);
        input.z = Mathf.Abs(input.z);
        return input;
    }

    private bool ProjectilesVisible()
    {
        return ParentForTargets.transform.childCount != 0;
    }

    public void StartRound()
    {
        CurrentRoundData = new RoundData();
        IsRoundActive = true;
        RoundScore = 0;
        if (CurrentMode == GameMode.Simple)
        {
            List<Vector3> positions = GetTargetLocations(Radius, TargetCount, StartingPos);
            List<GameObject> gameObjects = GetRandomObjects(Targets, TargetCount);

            InstantiateObjects(gameObjects, positions);
            GiveInstructions(gameObjects);
        }
        else if (CurrentMode == GameMode.Stroop)
        {
            List<Vector3> positions = GetTargetLocations(Radius, TargetCount, StartingPos);
            List<GameObject> gameObjects = GetRandomObjects(Targets, TargetCount);

            InstantiateObjects(gameObjects, positions);
            GiveStroopInstructions(gameObjects);
        }
        else
        {
            List<Vector3> positions = GetTargetLocations(Radius, TargetCount, StartingPos);
            List<GameObject> gameObjects = GetRandomObjects(Targets, TargetCount);

            InstantiateObjects(gameObjects, positions);
            GiveSwitchedInstructions(gameObjects);
        }
        CurrentRoundData.Mode = CurrentMode;
        CurrentRoundData.TimeInstantiated = DateTime.Now.ToString("HH:mm:ss:fffff");
        CurrentRoundData.RoundNumber = RoundNumber.ToString();
    }

    public void StartGame()
    {
        IsGameActive = true;
    }

    public void EndGame()
    {
        IsGameActive = false;
        Arrow.Hide();
        EndRound();
        GameOverText.gameObject.SetActive(true);
        SavePerformanceData();
    }

    public void EndRound()
    {
        ScoreHistory.Add(RoundScore);
        Debug.Log("Storing Round Data: " + CurrentRoundData.ToString());
        RoundDataHistory.Add(CurrentRoundData.ToString());
        for (int i = 0; i < ParentForTargets.transform.childCount; i++)
        {
            Destroy(ParentForTargets.transform.GetChild(i).gameObject);
        }
        IsRoundActive = false;
        RoundNumber++;
    }

    public void TargetHit(string hit)
    {
        CurrentRoundData.TimePicked = DateTime.Now.ToString("HH:mm:ss:fffff");
        CurrentRoundData.ColorSelected = hit.ToLower();
        CurrentRoundData.RightColor = CurrentGoal.ToLower();
        if (hit.ToLower() == CurrentGoal.ToLower())
        {
            Debug.Log("You hit the right one");
            CurrentRoundData.Score = 1.ToString();
            RoundScore++;
        }
        else
        {
            CurrentRoundData.Score = 0.ToString();
            RoundScore--;
        }
        CurrentRoundData.CurrentTime = DateTime.Now.ToString("HH:mm:ss:fffff");
        EndRound();
    }

    private void GiveInstructions(List<GameObject> targets)
    {
        var goalObj = DecideColorGoal(targets);
        CurrentGoal = goalObj.name;
        Instructions.text = "Catch the " + "<color=#" + ColorToHTML(StringToColor(goalObj.GetComponent<Target>().ColorName)) + ">" + goalObj.name + "</color>" + " targets";
    }
    private void GiveStroopInstructions(List<GameObject> targets)
    {
        var goalObject = DecideColorGoal(targets);
        CurrentGoal = goalObject.name;

        GameObject falseGoal = targets[0];
        int index = 0;
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].name != CurrentGoal)
            {
                falseGoal = targets[i];
                index = i;
                break;
            }
        }
        //Instructions.text = "Catch the " + "<color=#" + ColorToHTML(StringToColor(goalObject.GetComponent<Target>().ColorName)) + ">" + falseGoal.name + "</color>" + " targets";
        List<string> instructions = new List<string>();
        instructions.Add("<color=#" + ColorToHTML(StringToColor(goalObject.GetComponent<Target>().ColorName)) + ">" + falseGoal.name + "</color> ");

        for (int i = 0; i < InstructionCount - 1; i++)
        {
            instructions.Add("<color=#" + ColorToHTML(StringToColor(targets[i].GetComponent<Target>().ColorName)) + ">" + targets[i].name + "</color> ");
        }
        instructions.Shuffle();

        Instructions.text = "";
        for (int i = 0; i < instructions.Count; i++)
        {
            Instructions.text += instructions[i];
        }
        CurrentRoundData.TextColor = goalObject.GetComponent<Target>().ColorName.ToLower();
        CurrentRoundData.TextName = falseGoal.name.ToLower();
    }



    private void GiveSwitchedInstructions(List<GameObject> targets)
    {
        var goalObject = DecideColorGoal(targets);
        CurrentGoal = goalObject.name;

        GameObject falseGoal = Targets[0];
        foreach (GameObject current in Targets)
        {
            if (current.name != CurrentGoal)
            {
                falseGoal = current;
                break;
            }
        }
        Instructions.text = "Catch the " + "<color=#" + ColorToHTML(StringToColor(falseGoal.GetComponent<Target>().ColorName)) + ">" + falseGoal.name + "</color>" + " targets";
        CurrentRoundData.TextColor = goalObject.GetComponent<Target>().ColorName.ToLower();
        CurrentRoundData.TextName = falseGoal.name.ToLower();
    }
    private GameObject DecideColorGoal(List<GameObject> available)
    {
        return available[UnityEngine.Random.Range(0, available.Count)];
    }

    private List<Vector3> GetTargetLocations(float radius, int targetCount, Vector3 startingPos)
    {
        List<Vector3> resultingPos = new List<Vector3>();
        float angle = ((2 * Mathf.PI) / targetCount);
        float randomDistribution = UnityEngine.Random.Range(0, 2 * Mathf.PI);
        for (int i = 1; i <= targetCount; i++)
        {
            resultingPos.Add(new Vector3(startingPos.x + (radius * Mathf.Cos(randomDistribution + i * angle)), startingPos.y + (radius * Mathf.Sin(randomDistribution + i * angle)), 0));
        }
        return resultingPos;
    }
    private List<GameObject> GetRandomObjects(List<GameObject> available, int targetCount)
    {
        List<GameObject> resultingObjs = new List<GameObject>();
        for (int i = 0; i < targetCount; i++)
        {
            resultingObjs.Add(available[UnityEngine.Random.Range(0, available.Count)]);
        }

        for (int i = 1; i < targetCount; i++)
        {
            if (resultingObjs[i].name != resultingObjs[0].name)
            {
                return resultingObjs;
            }
        }

        return GetRandomObjects(available, targetCount);
    }
    private void InstantiateObjects(List<GameObject> objs, List<Vector3> positions)
    {
        for (int i = 0; i < objs.Count; i++)
        {
            var newObj = GameObject.Instantiate(objs[i]);
            newObj.transform.position = positions[i];
            newObj.transform.SetParent(ParentForTargets.transform);
            newObj.name = objs[i].name;
        }
    }

    private string ColorFromBerry(string input)
    {
        return input.Split(' ')[0].Trim().ToLower();
    }

    private Color StringToColor(string input)
    {
        Color parsedColor;
        if (ColorUtility.TryParseHtmlString(input.ToLower(), out parsedColor))
        {
            return parsedColor;
        }
        return new Color(0, 0, 0);
    }
    private string ColorToHTML(Color input)
    {
        return ColorUtility.ToHtmlStringRGB(input);
    }

    private void SavePerformanceData()
    {
        Debug.Log("Saving data for all rounds");
        DataSaver.SaveDat(@"C:\Users\Heracleia\Desktop\Fatigue_Cognitive\Data\" + CurrentUser + "_" + CurrentMode.ToString() + "_" + DateTime.Now.ToString("HH-mm-ss-fffff") + "_", RoundDataHistory);
    }

    public void SetHome()
    {
        fk.SetHome();
        Arrow.SetHome(Player.transform.position);
    }

    #region Public Modifiers
    public void OnGameModeChanged(int value)
    {
        CurrentMode = (GameMode)value;
    }
    public void OnRadiusChanged(float value)
    {
        Radius = value;
    }
    public void OnTargetCountChanged(float value)
    {
        TargetCount = (int)value;
    }
    public void UpdateUser(string name)
    {
        CurrentUser = name;
        UserText.text = "User: " + name;
    }
    #endregion

}
public enum GameMode
{
    Simple = 0,
    Stroop = 1,
    Switched = 2
}

class RoundData
{
    public GameMode Mode;
    public string CurrentTime;
    public string RoundNumber;
    public string RightColor;
    public string ColorSelected;
    public string TimeInstantiated;
    public string TimePicked;
    public string Score;
    public string TextColor;
    public string TextName;

    public override string ToString()
    {
        if (Mode == GameMode.Simple)
        {
            return CurrentTime + "," + RoundNumber + "," + RightColor + "," + ColorSelected + "," + TimeInstantiated + "," + TimePicked + "," + Score;
        }
        else
        {
            return CurrentTime + "," + RoundNumber + "," + RightColor + "," + ColorSelected + "," + TimeInstantiated + "," + TimePicked + "," + Score + "," + TextColor + "," + TextName;
        }
    }
}

public static class Extensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, (n + 1));
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
