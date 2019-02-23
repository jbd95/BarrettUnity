using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public GameManager manager;
    int ProjectileCount = 10;
    float XMaxDistance = 0.5f;
    float YMaxDistance = 0.25f;
    float VelocityMultiplier = 5;
    float SpawnRadius = 15;
    int SpawnRate = 1;
    public List<float> XValues;
    public List<float> YValues;

    public List<Button> xGridObjects;
    public List<Button> yGridObjects;
    public List<ActionCount> Fields;

    public int xIntervalCount = 10;
    public float xStartValue = -390.5f;
    public float xEndValue = 300f;

    public int yIntervalCount = 10;
    public float yStartValue = -390.5f;
    public float yEndValue = 300f;

    public GameObject TextFieldsTemplate;
    public GameObject UIContainer;
    public GameObject Bucket;
    public GridResize GridResizer;
    public Toggle GridVisibility;
    public Toggle FieldVisibility;
    public Button GridCenterMarker;
    public Button StartResizeButton;
    public Button FinishResizeButton;
    ColorBlock GridCenterMarkerVisibleColor;
    ColorBlock GridCenterMarkerInvisibleColor;

    public Pair User_Position;

    float xinterval, yinterval;

    // TODO public int NumberofWaves = 1;
    public Slider XDistanceSlider;
    public Slider YDistanceSlider;
    public Slider VelocitySlider;
    public Slider SpawnRadiusSlider;
    public Slider SpawnRateSlider;
    public Text ProjectileCountText;
    public Text UserNameText;
    public Dropdown PatternChooser;

    public ArrowAlignment Arrow;
    public ComputerPlayer2 AI;

    public Forward_Kinematics fk;
    public User CurrentUser = null;

    bool ShowGrid = true;


    // Use this for initialization
    void Start()
    {
        XValues = new List<float>();
        xGridObjects = new List<Button>();
        yGridObjects = new List<Button>();
        Fields = new List<ActionCount>();
        UpdateGameManagerValues();
        User_Position = new Pair(-1, -1);
        GridCenterMarkerVisibleColor = GridCenterMarker.colors;
        GridCenterMarkerInvisibleColor = GridCenterMarker.colors;
        GridCenterMarkerInvisibleColor.normalColor = new Color(GridCenterMarkerVisibleColor.normalColor.r, GridCenterMarkerVisibleColor.normalColor.g, GridCenterMarkerVisibleColor.normalColor.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        User_Position = BucketBoxPosition(Bucket.transform.position);
        //Arrow.Align();
    }

    void OnApplicationQuit()
    {
        SaveFieldValues();
    }

    public void Initialize()
    {
        UpdateSliderValues();
        manager.ResetStatistics();

        RemoveGrid();
        DivideIntervals();

        manager.GameTime = new System.Diagnostics.Stopwatch();
        manager.RosServer.SendGridInformation(XValues.Count - 1, YValues.Count - 1);
        manager.RosServer.SendClearGraph(XValues.Count -1, YValues.Count - 1);

        /*test
        ActionTable table = new ActionTable(10, 10, _random: true);
        Debug.Log(table.ToString());
        manager.BeginEpisode(table);*/
    }

    public void UpdateSliderValues()
    {
        manager.XMaxDistance = XDistanceSlider.value;
        manager.YMaxDistance = YDistanceSlider.value;
        if (ProjectileCountText.text != string.Empty)
        {
            manager.numWaves = Convert.ToInt32(ProjectileCountText.text);
        }
        manager.VelocityMultiplier = VelocitySlider.value;
        manager.spawnRadius = SpawnRadiusSlider.value;
        manager.spawnRate = SpawnRateSlider.value;
        manager.PatternType = (GameManager.Pattern)Enum.Parse(typeof(GameManager.Pattern), PatternChooser.options[PatternChooser.value].text);
    }

    public void StartGridResize()
    {
        GridResizer.gameObject.SetActive(true);
        if(XValues.Count != 0)
        {
            GridResizer.Resize(XValues[0], YValues[0], XValues[XValues.Count -1], YValues[YValues.Count - 1]);
        }
        StartResizeButton.gameObject.SetActive(false);
        FinishResizeButton.gameObject.SetActive(true);
    }

    public void FinishGridResize()
    {
        RemoveGrid();
        DivideIntervals();
        GridResizer.gameObject.SetActive(false);
        FinishResizeButton.gameObject.SetActive(false);
        StartResizeButton.gameObject.SetActive(true);
    }

    public void UpdateUser()
    {
        if(CurrentUser != null)
        {
            UserNameText.text = "User: " + CurrentUser.Name;
            AI.IsActive = CurrentUser.ISPC;
            AI.SetVelocity(CurrentUser.PCSpeed);
        }
    }

    public void ReceivedSpeedChange(float speed)
    {
        manager.VelocityMultiplier = speed;
        VelocitySlider.value = speed;
    }

    public void UpdateStartValues()
    {
        xStartValue = UIContainer.transform.InverseTransformPoint((GridResizer.Left.transform.position.x < GridResizer.Right.transform.position.x) ? GridResizer.Left.transform.position : GridResizer.Right.transform.position).x;
        xEndValue = UIContainer.transform.InverseTransformPoint((GridResizer.Right.transform.position.x > GridResizer.Left.transform.position.x) ? GridResizer.Right.transform.position : GridResizer.Left.transform.position).x;
        yStartValue = UIContainer.transform.InverseTransformPoint((GridResizer.Bottom.transform.position.y < GridResizer.Top.transform.position.y) ? GridResizer.Bottom.transform.position : GridResizer.Top.transform.position).y;
        yEndValue = UIContainer.transform.InverseTransformPoint((GridResizer.Top.transform.position.y > GridResizer.Bottom.transform.position.y) ? GridResizer.Top.transform.position : GridResizer.Bottom.transform.position).y;
    }

    void UpdateGameManagerValues()
    {
        manager.XMaxDistance = XMaxDistance;
        manager.YMaxDistance = YMaxDistance;
        manager.VelocityMultiplier = VelocityMultiplier;
        manager.spawnRadius = SpawnRadius;
        manager.numWaves = ProjectileCount;
        manager.spawnRate = SpawnRate;
        SetGridVisibility(GridVisibility.isOn);
    }

    public Vector3 GetBoxCoordinates(int x, int y)
    {
        float xpos = xStartValue + (x * xinterval) + (xinterval / 2);
        float ypos = yStartValue + (y * yinterval) + (yinterval / 2);

        Vector3 transformedDestination = UIContainer.transform.TransformPoint(new Vector3(xpos, ypos, 0));

        return new Vector3(transformedDestination.x, transformedDestination.y, 0);
    }

    public Vector3 GetBoxCoordinatesRelativeToUIContainer(int x, int y)
    {
        float xpos = xStartValue + (x * xinterval) + (xinterval / 2);
        float ypos = yStartValue + (y * yinterval) + (yinterval / 2);

        return new Vector3(xpos, ypos, 0);
    }

    public Vector3 GetBoxCoordinatesInWorldFrame(int x, int y)
    {
        float xpos = xStartValue + (x * xinterval) + (xinterval / 2);
        float ypos = yStartValue + (y * yinterval) + (yinterval / 2);

        Vector3 transformedDestination = UIContainer.transform.TransformPoint(new Vector3(xpos, ypos, 0));
        Vector3 finalPoint = manager.mainCamera.ScreenToWorldPoint(new Vector3(transformedDestination.x, transformedDestination.y, 1.1f));

        return finalPoint;
    }

    public Transform GetBoxCoordinates(Transform projectile, int x, int y)
    {
        if (projectile.GetComponent<Projectile>() == null)
        {
            return projectile;
        }

        float xpos = xStartValue + (x * xinterval) + (xinterval / 2);
        float ypos = yStartValue + (y * yinterval) + (yinterval / 2);

        Vector3 transformedDestination = UIContainer.transform.TransformPoint(new Vector3(xpos, ypos, 0));
        Vector3 finalPoint = manager.mainCamera.ScreenToWorldPoint(new Vector3(transformedDestination.x, transformedDestination.y, 1.1f));

        projectile.gameObject.GetComponent<Projectile>().InitialPos = new Vector2(finalPoint.z, finalPoint.y);
        projectile.gameObject.GetComponent<Projectile>().StartPos = new Pair(x, y);

        return projectile;
    }

    public Vector2 GetTopLeftBoxCoordinatesRelativeToUIContainer(int x, int y)
    {
        return new Vector2((xStartValue + (x * xinterval)), (yStartValue + ((y+1) * yinterval)));
    }

    public void SetProjectilePosition(Vector3 position)
    {
        position = UIContainer.transform.InverseTransformPoint(position);

        for (int x = 0; x < xIntervalCount; x++)
        {
            for (int y = 0; y < yIntervalCount; y++)
            {
                if (MouseIntersects(position, xStartValue + (x * xinterval), xStartValue + ((x + 1) * xinterval), yStartValue + (y * yinterval), yStartValue + ((y + 1) * yinterval)))
                {
                    manager.SendtoBox(x, y);
                    return;
                }
            }
        }
    }

    public void ToggleFieldVisibility(bool othercase = true)
    {
        if (othercase)
        {
            foreach (ActionCount current in Fields)
            {
                current.SetVisible(FieldVisibility.isOn);
            }
        }
        else
        {
            foreach (ActionCount current in Fields)
            {
                current.SetVisible(false);
            }
        }
    }

    public ActionCount GetFieldAtBoxCoordinates(Pair location)
    {
        foreach(ActionCount current in Fields)
        {
            if(current.Location == location)
            {
                return current;
            }
        }
        return null;
    }

    private void CreateFields()
    {
        for(int i = 0; i < xIntervalCount; i++)
        {
            for(int j = 0; j < yIntervalCount; j++)
            {
                GameObject created = Instantiate(TextFieldsTemplate);
                created.transform.SetParent(UIContainer.gameObject.transform);
                created.transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, xinterval);
                created.transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, yinterval);
                created.transform.localPosition = GetBoxCoordinatesRelativeToUIContainer(i, j);
                created.gameObject.SetActive(true);
                created.GetComponent<ActionCount>().SetVisible(FieldVisibility.isOn);
                created.GetComponent<ActionCount>().Location = new Pair(i, j);

                if (CurrentUser == null || CurrentUser.Name == "")
                {
                    created.GetComponent<ActionCount>().SetVisitedValue(0);
                    created.GetComponent<ActionCount>().SetTrainedValue(0);
                }
                else if (CurrentUser != null && CurrentUser.VisitedLocations.Count > 0 && CurrentUser.TrainedLocations.Count > 0)
                {
                    created.GetComponent<ActionCount>().SetVisitedValue(CurrentUser.GetVisitedLocation(new Pair(i, j)));
                    created.GetComponent<ActionCount>().SetTrainedValue(CurrentUser.GetTrainedLocation(new Pair(i, j)));
                }

                Fields.Add(created.GetComponent<ActionCount>());
            }
        }
    }

    private void DeleteFields()
    {
        SaveFieldValues();
        foreach(ActionCount current in Fields)
        {
            GameObject.Destroy(current.gameObject);
        }
        Fields.Clear();
    }

    private void SaveFieldValues()
    {
        if (CurrentUser != null && Fields.Count > 0)
        {
            CurrentUser.VisitedLocations.Clear();
            CurrentUser.TrainedLocations.Clear();
            foreach (var current in Fields)
            {
                CurrentUser.VisitedLocations.Add(current.Location, current.GetVisitedValue());
                CurrentUser.TrainedLocations.Add(current.Location, current.GetTrainedValue());
            }
            CurrentUser.Save();
        }
    }

    bool MouseIntersects(Vector3 position, float xStart, float xEnd, float yStart, float yEnd)
    {
        //Debug.Log(position);
        if (position.x > xStart && position.x < xEnd)
        {
            if (position.y > yStart && position.y < yEnd)
            {
                return true;
            }
        }
        return false;
    }

    public Pair BucketBoxPosition(Vector3 user_pos)
    {
        user_pos = UIContainer.transform.InverseTransformPoint(manager.mainCamera.WorldToScreenPoint(user_pos));

        //Debug.Log(User_Position);

        for (int x = 0; x < xIntervalCount; x++)
        {
            for (int y = 0; y < yIntervalCount; y++)
            {
                if (ZIntersects(user_pos, xStartValue + (x * xinterval), xStartValue + ((x + 1) * xinterval), yStartValue + (y * yinterval), yStartValue + ((y + 1) * yinterval)))
                {
                    return new Pair(x, y);
                }
            }
        }
        //the bucket is not in a box
        return new Pair(-1, -1);
    }

    bool ZIntersects(Vector3 position, float xStart, float xEnd, float yStart, float yEnd)
    {
        if (position.x > xStart && position.x < xEnd)
        {
            if (position.y >= yStart && position.y <= yEnd)
            {
                return true;
            }
        }
        return false;
    }



    public void ToggleGrid()
    {
        if (ShowGrid != GridVisibility.isOn)
        {
            if(GridVisibility.isOn)
            {
                foreach (Button current in xGridObjects)
                {
                    current.transform.gameObject.SetActive(true);
                }

                foreach (Button current in yGridObjects)
                {
                    current.transform.gameObject.SetActive(true);
                }
                ToggleFieldVisibility(true);
                ShowGrid = true;
            }
            else
            {
                foreach (Button current in xGridObjects)
                {
                    current.transform.gameObject.SetActive(false);
                }

                foreach (Button current in yGridObjects)
                {
                    current.transform.gameObject.SetActive(false);
                }
                ToggleFieldVisibility(false);
                ShowGrid = false;
            }
            
        }
    }
    public void SetGridVisibility(bool visibility)
    {
        GridVisibility.isOn = visibility;
    }

    public void SetGridCenterVisibility(bool visibility)
    {
        if(GridCenterMarker.colors.normalColor.a == 0 && visibility)
        {
            GridCenterMarker.colors = GridCenterMarkerVisibleColor;
        }
        else if(GridCenterMarker.colors.normalColor.a != 0 && !visibility)
        {
            GridCenterMarker.colors = GridCenterMarkerInvisibleColor;
        }
    }

    void DivideIntervals()
    {
        float xdifference = Mathf.Abs(xEndValue - xStartValue);
        xinterval = xdifference / (xIntervalCount);

        for (int x = 0; x <= xIntervalCount; x++)
        {
            XValues.Add(xStartValue + (x * xinterval));
        }

        float ydifference = Mathf.Abs(yEndValue - yStartValue);
        yinterval = ydifference / (yIntervalCount);

        for (int y = 0; y <= yIntervalCount; y++)
        {
            YValues.Add(yStartValue + (y * yinterval));
        }

        //Arrow.StartPosition = GetBoxCoordinatesRelativeToUIContainer(manager.GridCenter.x, manager.GridCenter.y);

        SetupGrid();
    }

    void SetupGrid()
    {
        DeleteFields();
        GridResizer.Left.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (yEndValue - yStartValue));
        GridResizer.Bottom.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (xEndValue - xStartValue));

        for (int x = 0; x <= xIntervalCount; x++)
        {
            Button temp = Instantiate(GridResizer.Left);
            temp.transform.SetParent(UIContainer.gameObject.transform);
            temp.transform.localPosition = new Vector3(XValues[x], (YValues[0] - ((yStartValue - yEndValue) / 2)), GridResizer.Left.transform.position.z);
            Destroy(temp.gameObject.GetComponent<xGridTemplateMove>());
            xGridObjects.Add(temp);

            if (ShowGrid)
            {
                temp.gameObject.SetActive(true);
            }
            else
            {
                temp.gameObject.SetActive(false);
            }
        }

        for (int y = 0; y <= yIntervalCount; y++)
        {
            Button temp = Instantiate(GridResizer.Bottom);
            temp.transform.SetParent(UIContainer.gameObject.transform);
            temp.transform.localPosition = new Vector3((XValues[0] - ((xStartValue - xEndValue) / 2)), YValues[y], GridResizer.Bottom.transform.position.z);
            Destroy(temp.gameObject.GetComponent<yGridTemplateMove>());
            yGridObjects.Add(temp);

            if (ShowGrid)
            {
                temp.gameObject.SetActive(true);
            }
            else
            {
                temp.gameObject.SetActive(false);
            }
        }

        CreateFields();
        
       /* GridCenterMarker.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (yinterval));
        GridCenterMarker.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (xinterval));
        GridCenterMarker.transform.position = GetBoxCoordinates(manager.GridCenter.x, manager.GridCenter.y);
        GridCenterMarker.gameObject.SetActive(true);
        SetGridCenterVisibility(false);*/
    }

    void RemoveGrid()
    {
        for (int x = 0; x < xGridObjects.Count; x++)
        {
            if (xGridObjects[x] != null)
            {
                GameObject.Destroy(xGridObjects[x].gameObject);
                xGridObjects[x] = null;
            }
        }

        for (int y = 0; y < yGridObjects.Count; y++)
        {
            if (yGridObjects[y] != null)
            {
                GameObject.Destroy(yGridObjects[y].gameObject);
                yGridObjects[y] = null;
            }
        }

        xGridObjects.Clear();
        XValues.Clear();
        yGridObjects.Clear();
        YValues.Clear();
    }
}
