using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SendToRobot : MonoBehaviour {

    public RotateBody _RotateBody;
    public RotateJoint2 _RotateJoint2;
    public RotateJoint3 _RotateJoint3;

    public float[] finalSolution = new float[3];

    // Update is called once per frame
    void variables () {

        finalSolution[0] = _RotateBody.GiveRotateA();
        finalSolution[1] = _RotateJoint2.GiveRotateB();
        finalSolution[2] = _RotateJoint3.GiveRotateC();

        print(finalSolution[0]);
        print(finalSolution[1]);
        print(finalSolution[2]);

    }

}
