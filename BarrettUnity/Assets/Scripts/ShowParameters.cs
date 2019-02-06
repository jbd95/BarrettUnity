using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowParameters : MonoBehaviour {

    public Dropdown OptionChooser;
    public GameManager manager;
    public InputField inputTemplate;

    List<InputField> fields = new List<InputField>();

    // Use this for initialization
    void Start () {
		
	}
	
	public void SelectionMade()
    {
        GameManager.Pattern selected = (GameManager.Pattern)Enum.Parse(typeof(GameManager.Pattern), OptionChooser.options[OptionChooser.value].text);
        DeleteFields();
        manager.PatternParameters.Clear();
        switch (selected)
        {
            case GameManager.Pattern.Line:
                fields = ShowInputAreas(2);
                break;
            case GameManager.Pattern.Sine:
                fields = ShowInputAreas(1);
                break;
            case GameManager.Pattern.Cosine:
                fields = ShowInputAreas(1);
                break;
            case GameManager.Pattern.SemiCircle:
                fields = ShowInputAreas(1);
                break;
        }
    }

    List<InputField> ShowInputAreas(int count)
    {
        int padding = 15;
        float yStartPos = transform.localPosition.y + padding;
        List<InputField> fields = new List<InputField>();

        for(int i = 0; i < count; i++)
        {
            InputField temp = GameObject.Instantiate(inputTemplate);
            temp.transform.SetParent(this.transform.parent);
            temp.transform.localPosition = new Vector3(transform.localPosition.x, yStartPos, 0);
            temp.text = "1";
            yStartPos += padding + temp.preferredHeight;
            temp.onEndEdit.AddListener(delegate { UpdateValues(); });
            temp.gameObject.SetActive(true);
            fields.Add(temp);
        }

        return fields;
    }

    void DeleteFields()
    {
        foreach(InputField current in fields)
        {
            current.gameObject.SetActive(false);
            Destroy(current.gameObject);
        }
        fields.Clear();
    }

    void UpdateValues()
    {
        manager.PatternParameters.Clear();
        foreach (InputField current in fields)
        {
            manager.PatternParameters.Add(Convert.ToInt32(current.text));
        }
    }
}
