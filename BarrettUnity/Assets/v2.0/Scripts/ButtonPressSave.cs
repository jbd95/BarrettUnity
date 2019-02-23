using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPressSave : MonoBehaviour {
    public Button SaveButton;
    public Recorder other;
    
    // Use this for initialization
    void Start () {
        SaveButton.onClick.AddListener(SaveDat);
        
    }
	// Update is called once per frame
	void SaveDat () {
        List<string> DummyDat = new List<string>();
        for (int i = 0; i < 5; i++)
            DummyDat.Add(i + ",This is a Test," + (i + 1));
        other.SaveDat(@"C:\Users\Heracleia\Desktop\Fatigue_Cognitive\Test\",DummyDat);
	}
}
