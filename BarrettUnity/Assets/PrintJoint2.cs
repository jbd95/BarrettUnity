using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PrintJoint2: MonoBehaviour
{


    public Text my_text;
    public float Rotation;

    void Start()
    {
        my_text = GameObject.Find("Joint2Text").GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {

        Rotation = GameObject.Find("Joint2Slider").GetComponent<Slider>().value;
        my_text.text = Rotation.ToString();


    }
}

