using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System;

public class Login : MonoBehaviour
{

    public GameObject LoginPanel;
    public GameObject ExistingUserScreen;
    public GameObject NewUserScreen;
    public GameObject LoadingScreen;
    public Dropdown UserSelection;
    public InputField NewUserName;
    public Text DuplicateNameErrorText;
    public Button ExistingUserContinue;
    public Button NewUserContinue;
    public Toggle IsPCPlayer;
    public Dropdown PCPlayerSpeed;
    Vector3 NewContinueStart;
    bool AnimationRunning = false;

    string SaveLocation = "";
    string UserListFileName = "Users.txt";
    string AddUserText = "Add User";

    User LoadedUser = null;

    // Use this for initialization
    void Start()
    {
        LoginPanel.SetActive(true);
        ExistingUserScreen.SetActive(true);
        NewUserScreen.SetActive(false);
        LoadingScreen.SetActive(false);

        if (File.Exists(SaveLocation + UserListFileName))
        {
            UserSelection.ClearOptions();
            foreach (string current in CheckForFileValidity(File.ReadAllLines(SaveLocation + UserListFileName)))
            {
                UserSelection.options.Add(new Dropdown.OptionData(current.Trim()));
            }
            UserSelection.options.Add(new Dropdown.OptionData(AddUserText));
            UserSelection.value = 0;
            UserSelection.captionText.text = UserSelection.options[UserSelection.value].text;
        }
        else
        {
            File.Create(SaveLocation + UserListFileName);
        }
        NewContinueStart = NewUserContinue.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetUserList().Contains(NewUserName.text))
        {
            DuplicateNameErrorText.gameObject.SetActive(false);
            if (NewUserName.text.Trim() != "")
            {
                if (!AnimationRunning && NewUserContinue.transform.position == NewContinueStart)
                {
                    AnimationRunning = true;
                    StartCoroutine(AnimateContinueButton(NewUserContinue, NewContinueStart));
                }
            }
            else
            {
                StopCoroutine("AnimateContinueButton");
                NewUserContinue.transform.position = NewContinueStart;
                AnimationRunning = false;
            }
        }
        else
        {
            StopCoroutine("AnimateContinueButton");
            NewUserContinue.transform.position = NewContinueStart;
            DuplicateNameErrorText.gameObject.SetActive(true);
            AnimationRunning = false;
        }
    }


    public void ExistingUserContinueClick()
    {
        string selected = UserSelection.options[UserSelection.value].text;
        if (selected == AddUserText)
        {
            AddUserMenu();
        }
        else
        {
            User loadedUser = new User(File.ReadAllText(SaveLocation + selected + ".txt"));
            Debug.Log("User Loaded: " + loadedUser.ToString());
            LoadGame(loadedUser);
        }
    }
    public void NewUserContinueClick()
    {
        if (!GetUserList().Contains(NewUserName.text) && NewUserName.text.Trim() != "")
        {
            DuplicateNameErrorText.gameObject.SetActive(false);
            LoadGame(AddUser(NewUserName.text));
        }
        else
        {
            DuplicateNameErrorText.gameObject.SetActive(true);
        }

    }

    public void AddUserMenu()
    {
        NewUserScreen.SetActive(true);
        ExistingUserScreen.SetActive(false);
    }

    public void IsPCPlayerValueChanged()
    {
        PCPlayerSpeed.gameObject.SetActive(IsPCPlayer.isOn);
    }

    public User AddUser(string name)
    {
        StreamWriter namewriter = new StreamWriter(SaveLocation + UserListFileName, true);
        namewriter.WriteLine(name);
        namewriter.Close();
        StreamWriter writer = new StreamWriter(SaveLocation + name + ".txt");
        User newUser = new User(name, SaveLocation + name + ".txt");
        newUser.ISPC = IsPCPlayer.isOn;
        if (newUser.ISPC)
        {
            newUser.PCSpeed = PCPlayerSpeed.options[PCPlayerSpeed.value].text.Trim();
        }
        writer.Write(newUser.ToJson());
        writer.Close();
        return newUser;
    }

    public void LoadGame(User user)
    {
        /*LoadedUser = user;
        NewUserScreen.SetActive(false);
        ExistingUserScreen.SetActive(false);
        LoadingScreen.SetActive(true);
        StartCoroutine(LoadingAnimation());
        SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Main"));
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;*/

        LoadedUser = user;
        NewUserScreen.SetActive(false);
        ExistingUserScreen.SetActive(false);
        LoadingScreen.SetActive(true);
        StartCoroutine(LoadingAnimation());
        SceneManager.LoadSceneAsync("Main_2.0", LoadSceneMode.Single);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Main_2.0"));
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>().UpdateUser(LoadedUser.Name);
        //GameObject.FindGameObjectWithTag("Controller").GetComponent<GameController>().UpdateUser();
    }

    public List<string> GetUserList()
    {
        return CheckForFileValidity(File.ReadAllLines(SaveLocation + UserListFileName));
    }

    public IEnumerator LoadingAnimation()
    {
        Text loadingtext = LoadingScreen.GetComponentInChildren<Text>();
        while (LoadingScreen.activeSelf)
        {
            loadingtext.text += ".";
            yield return new WaitForSeconds(0.1f);
        }
    }

    private List<string> CheckForFileValidity(string[] usernames)
    {
        List<string> returnable = new List<string>();
        foreach (string current in usernames)
        {
            if (File.Exists(SaveLocation + current + ".txt"))
            {
                returnable.Add(current);
            }
        }
        return returnable;
    }

    public void CallAnimateContinueButton()
    {

    }

    private IEnumerator AnimateContinueButton(Button button, Vector3 start)
    {
        Vector3 endPosition = button.transform.position;
        endPosition.y -= 40;
        Vector3 currentPosition = start;
        button.transform.position = currentPosition;

        while (currentPosition.y != endPosition.y)
        {
            yield return new WaitForSeconds(0.01f);
            currentPosition.y -= 2;
            button.transform.position = currentPosition;
        }
        AnimationRunning = false;
    }
}


[Serializable]
public class User
{
    public string Name;
    public string FileLocation;
    public bool ISPC;
    public string PCSpeed;
    public Dictionary<Pair, int> VisitedLocations;
    public Dictionary<Pair, int> TrainedLocations;

    public User(string _name, string _fileloc)
    {
        Name = _name;
        FileLocation = _fileloc;
        VisitedLocations = new Dictionary<Pair, int>();
        TrainedLocations = new Dictionary<Pair, int>();
        VisitedLocations.Add(new Pair(0, 0), 5);
        TrainedLocations.Add(new Pair(1, 1), 10);
        PCSpeed = "";
        ISPC = false;
    }

    public User(string _json)
    {
        List<string> usable = new List<string>();
        foreach (string current in _json.Split('{'))
        {
            if (current.Trim() != "")
            {
                usable.Add("{" + current);
            }
        }

        VariableFromJson(usable[0]);
        VariableFromJson(usable[1]);
        VariableFromJson(usable[2]);
        VariableFromJson(usable[3]);
        DictionaryFromJson(usable[4]);
        DictionaryFromJson(usable[5]);
    }

    public override string ToString()
    {
        return "Name: " + Name + "\nFileLocation: " + FileLocation;
    }
    public string GetName()
    {
        return Name;
    }
    private string DictionaryToJson(Dictionary<Pair, int> dictionary, string name)
    {
        string returnable = "{|" + name + "|\n";
        foreach (var current in dictionary)
        {
            returnable += ("<" + current.Key.ToString() + ">=(" + current.Value.ToString() + ")\n");
        }
        returnable += "}\n";
        return returnable;
    }
    private void DictionaryFromJson(string json)
    {
        Dictionary<Pair, int> dictionary = new Dictionary<Pair, int>();
        json = json.Replace('{', ' ');
        json = json.Replace('}', ' ');
        string dictionaryName = "";
        string[] newlinedelimited = json.Split('\n');

        foreach (string current in newlinedelimited)
        {
            if (current.Trim() == "")
            {
                continue;
            }
            if (current.Contains("|"))
            {
                dictionaryName = current.Split('|')[1];
            }
            else
            {
                dictionary.Add(Pair.FromString(current.Split('<')[1].Split('>')[0]), int.Parse(current.Split('=')[1].Split('(')[1].Split(')')[0]));
            }
        }

        switch (dictionaryName)
        {
            case "VisitedLocations":
                VisitedLocations = dictionary;
                break;

            case "TrainedLocations":
                TrainedLocations = dictionary;
                break;
        }
    }
    private string VariableToJson(string _name, object value)
    {
        return "{|" + _name + "|=" + value.ToString() + "}\n";
    }
    private void VariableFromJson(string json)
    {
        string varname = json.Split('|')[1];
        string value = json.Split('=')[1].Split('}')[0];

        switch (varname)
        {
            case "Name":
                Name = value;
                break;
            case "FileLocation":
                FileLocation = value;
                break;
            case "ISPC":
                ISPC = bool.Parse(value);
                break;
            case "PCSpeed":
                PCSpeed = value;
                break;
        }
    }
    public string ToJson()
    {
        string json = "";
        json += VariableToJson("Name", Name);
        json += VariableToJson("FileLocation", FileLocation);
        json += VariableToJson("ISPC", ISPC);
        json += VariableToJson("PCSpeed", PCSpeed);
        json += DictionaryToJson(VisitedLocations, "VisitedLocations");
        json += DictionaryToJson(TrainedLocations, "TrainedLocations");
        Debug.Log(json);
        return json;
    }

    public int GetVisitedLocation(Pair location)
    {
        foreach (var current in VisitedLocations)
        {
            if (current.Key == location)
            {
                return current.Value;
            }
        }
        return 0;
    }

    public int GetTrainedLocation(Pair location)
    {
        foreach (var current in TrainedLocations)
        {
            if (current.Key == location)
            {
                return current.Value;
            }
        }
        return 0;
    }

    public void Save()
    {
        File.WriteAllText(FileLocation, ToJson());
    }
}
