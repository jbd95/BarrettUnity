using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RotateJoint3 : MonoBehaviour {

    public float RotationC;

    public void RotateC()
    {
        RotationC = GameObject.Find("Joint3Slider").GetComponent<Slider>().value;
        transform.localEulerAngles = new Vector3(0, 0, RotationC);
    }

    public void CallRotateC()
    {

        transform.localEulerAngles = new Vector3(0, 0, RotationC);
    }

    public float GiveRotateC()
    {
        return RotationC;
    }

}
