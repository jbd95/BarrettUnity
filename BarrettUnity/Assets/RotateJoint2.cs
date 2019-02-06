using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RotateJoint2 : MonoBehaviour
{

    public float RotationB;
    public float RotationD;

    public void RotateB()
    {
        RotationB = GameObject.Find("Joint2Slider").GetComponent<Slider>().value;
        transform.localEulerAngles = new Vector3(0, 0, RotationB); 
    }

    public void CallRotateB()
    {

        transform.localEulerAngles = new Vector3(0, 0, RotationB);
        transform.RotateAround(transform.position, transform.up, RotationD);
    }

    public float GiveRotateB()
    {
        return RotationB;
    }

}