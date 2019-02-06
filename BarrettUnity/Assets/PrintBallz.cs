using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PrintBallz : MonoBehaviour
{


    public Text my_text;
    public float z;

    void Start()
    {
        my_text = GameObject.Find("BallzText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

        z = GameObject.Find("Ballz").GetComponent<Slider>().value;
        my_text.text = z.ToString();


    }
}