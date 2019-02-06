using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PrintBallx : MonoBehaviour
{


    public Text my_text;
    public float x;

    void Start()
    {
        my_text = GameObject.Find("BallxText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

        x = GameObject.Find("Ballx").GetComponent<Slider>().value;
        my_text.text = x.ToString();


    }
}
