using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoveBall : MonoBehaviour
{

    public float x;
    public float y;
    public float z;

    // Update is called once per frame
    public void Move()
    {
        x = -GameObject.Find("Ballx").GetComponent<Slider>().value;
        y = GameObject.Find("Ballz").GetComponent<Slider>().value;
        z = -GameObject.Find("Bally").GetComponent<Slider>().value;


        transform.localPosition = new Vector3(x, y, z);

    }
}
