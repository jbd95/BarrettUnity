using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PrintBally : MonoBehaviour
{


    public Text my_text;
    public float y;

    void Start()
    {
        my_text = GameObject.Find("BallyText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

        y = GameObject.Find("Bally").GetComponent<Slider>().value;
        my_text.text = y.ToString();


    }
}
