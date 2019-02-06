using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionCount : MonoBehaviour
{
    public Pair Location = new Pair(-1, -1);
    public Text VisitedText;
    public Text TrainedText;
    int VisitedCount = 0;
    int TrainedCount = 0;
    // Use this for initialization
    void Start()
    {
        UpdateValues();
    }

    private void UpdateValues()
    {
        VisitedText.text = "" + VisitedCount;
        TrainedText.text = "" + TrainedCount;
    }
    public int GetVisitedValue()
    {
        return VisitedCount;
    }
    public int GetTrainedValue()
    {
        return TrainedCount;
    }
    public void SetVisitedValue(int _value)
    {
        VisitedCount = _value;
        UpdateValues();
    }
    public void SetTrainedValue(int _value)
    {
        TrainedCount = _value;
        UpdateValues();
    }
    public void IncrementVisitedValue()
    {
        VisitedCount++;
        UpdateValues();
    }
    public void IncrementTrainedValue()
    {
        TrainedCount++;
        UpdateValues();
    }

    public void SetVisible(bool _visible)
    {
        VisitedText.gameObject.SetActive(_visible);
        TrainedText.gameObject.SetActive(_visible);
    }

}
