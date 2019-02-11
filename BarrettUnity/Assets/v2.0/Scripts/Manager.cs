using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (IsRoundActive)
        {
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
    }

    public void StartRound()
    {
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
    }

    public void EndRound()
    {
        ScoreHistory.Add(RoundScore);
        for (int i = 0; i < ParentForTargets.transform.childCount; i++)
        {
            Destroy(ParentForTargets.transform.GetChild(i).gameObject);
        }
        IsRoundActive = false;
    }

    public void TargetHit(string hit)
    {
        Debug.Log("you hit " + hit + " target");

        if (hit.ToLower() == CurrentGoal.ToLower())
        {
            Debug.Log("You hit the right one");
            RoundScore++;
        }
        else
        {
            RoundScore--;
        }
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

        GameObject falseGoal = Targets[0];
        foreach (GameObject current in Targets)
        {
            if (current.name != CurrentGoal)
            {
                falseGoal = current;
                break;
            }
        }
        Instructions.text = "Catch the " + "<color=#" + ColorToHTML(StringToColor(goalObject.GetComponent<Target>().ColorName)) + ">" + falseGoal.name + "</color>" + " targets";
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
    }
    private GameObject DecideColorGoal(List<GameObject> available)
    {
        return available[Random.Range(0, available.Count)];
    }

    private List<Vector3> GetTargetLocations(float radius, int targetCount, Vector3 startingPos)
    {
        List<Vector3> resultingPos = new List<Vector3>();
        float angle = (2 * Mathf.PI) / targetCount;

        for (int i = 1; i <= targetCount; i++)
        {
            resultingPos.Add(new Vector3(startingPos.x + (radius * Mathf.Cos(i * angle)), startingPos.y + (radius * Mathf.Sin(i * angle)), 0));
        }
        return resultingPos;
    }
    private List<GameObject> GetRandomObjects(List<GameObject> available, int targetCount)
    {
        List<GameObject> resultingObjs = new List<GameObject>();
        for (int i = 0; i < targetCount; i++)
        {
            resultingObjs.Add(available[Random.Range(0, available.Count)]);
        }
        return resultingObjs;
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
    #endregion

}
public enum GameMode
{
    Simple = 0,
    Stroop = 1,
    Switched = 2
}
