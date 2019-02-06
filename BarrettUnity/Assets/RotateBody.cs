using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RotateBody : MonoBehaviour {

    public float RotationA;


    public void RotateA()
    {
        RotationA = GameObject.Find("Joint1Slider").GetComponent<Slider>().value;
        transform.localEulerAngles = new Vector3(0, RotationA, 0);
    }

    public void CallRotateA()
    {


            transform.localEulerAngles = new Vector3(0, RotationA, 0);

    }

    public float GiveRotateA()
    {
        return RotationA;
    }

}
