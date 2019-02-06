using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PrintJoint1 : MonoBehaviour {


    public Text my_text;
    public float Rotation;

    void Start()
    {
        my_text = GameObject.Find("Joint1Text").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update () {

        Rotation = GameObject.Find("Joint1Slider").GetComponent<Slider>().value;
        my_text.text = Rotation.ToString();


    }
}
