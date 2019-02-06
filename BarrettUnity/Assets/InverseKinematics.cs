using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text;

public class InverseKinematics : MonoBehaviour
{

        public float x;
        public float y;
        public float z;

        public float l1 = 0.55f;
        public float l2 = 0.35f;

        public float z3 = 0.045f;
        public float z4 = -0.045f;

        public float theta1;
        public float[] theta4 = new float[2];
        public float[] theta2 = new float[4];
        public float[] finalSolution = new float[3];

        public RotateBody _RotateBody;
        public RotateJoint2 _RotateJoint2;
        public RotateJoint3 _RotateJoint3; 

        public int flag;

        void Update()
        {


        //Get the coordinates of the Sphere
        x = -transform.position.x;
        y = -transform.position.z;
        z = transform.position.y;

        if (y < 0.01)
        {

            //Finding Theta 1
            theta1 = -Mathf.Atan2(y, x);

            //Finding Theta 4
            float N = (Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2) - Mathf.Pow(z3, 2) - Mathf.Pow(z4, 2) - Mathf.Pow(l1, 2) - Mathf.Pow(l2, 2)) / 2;
            float R = z3 * l2 - z4 * l1;
            float T = l1 * l2 + z3 * z4;
            float a = (N + T) / 2;
            float b = -R;
            float c = (N - T) / 2;

            //First solution of theta4
            theta4[0] = 2 * Mathf.Atan((-b + Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / (2 * a));

            //Second solution of theta4
            theta4[1] = 2 * Mathf.Atan((-b - Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / (2 * a));

            //Finding Theta 2

            // Theta 2 solution for the fist solution of theta4
            float B = 0;
            float M = Mathf.Cos(theta4[0]) * l2 - z4 * Mathf.Sin(theta4[0]) + l1;
            float P = Mathf.Sin(theta4[0]) * l2 + z4 * Mathf.Cos(theta4[0]) + z3;

            theta2[0] = Mathf.Atan(((Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) - Mathf.Pow(B, 2)) * M - z * P)) / (Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) - Mathf.Pow(B, 2)) * P + z * M));
            theta2[1] = Mathf.Atan(((-Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) - Mathf.Pow(B, 2)) * M - z * P)) / (-Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) - Mathf.Pow(B, 2)) * P + z * M));

            // Theta 2 solution for the second solution of theta4

            M = Mathf.Cos(theta4[1]) * l2 - z4 * Mathf.Sin(theta4[1]) + l1;
            P = Mathf.Sin(theta4[1]) * l2 + z4 * Mathf.Cos(theta4[1]) + z3;

            theta2[2] = Mathf.Atan(((Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) - Mathf.Pow(B, 2)) * M - z * P)) / (Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) - Mathf.Pow(B, 2)) * P + z * M));
            theta2[3] = Mathf.Atan(((-Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) - Mathf.Pow(B, 2)) * M - z * P)) / (-Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) - Mathf.Pow(B, 2)) * P + z * M));

            //Find final solution
            for (int i = 0; i < 5; i++)
            {

                for (int j = 0; i < 2; i++)
                {
                    if (theta2[i] >= 0 && theta4[j] >= 0)
                    {
                        finalSolution[1] = theta2[i];
                        finalSolution[2] = theta4[j];
                    }
                }
            }

            if (x > 0 && x < 3.5)
            {
                finalSolution[2] = theta4[0];

            }

            if (z > 0 && z < 3.6)
            {
                finalSolution[1] = theta2[0];
            }

            finalSolution[0] = theta1;

            //Call the joint motion scripts
            //_RotateBody.RotationA = finalSolution[0] * Mathf.Rad2Deg;
            //_RotateBody.CallRotateA();

            //_RotateJoint2.RotationB = finalSolution[1] * Mathf.Rad2Deg;
            //_RotateJoint2.CallRotateB();

            //_RotateJoint3.RotationC = finalSolution[2] * Mathf.Rad2Deg;
            //_RotateJoint3.CallRotateC();

        }   

    }


    void connectMatlab(int flag)
    {

        System.Net.Sockets.TcpClient clientSocket;
        clientSocket = new System.Net.Sockets.TcpClient();
        clientSocket.Connect("192.168.1.2", 12345);
        NetworkStream networkStream = clientSocket.GetStream();
        string dataFromClient;

        if (flag == 0)
        {

            if (float.IsNaN(finalSolution[0]))
            {
                GameObject Body = GameObject.Find("Barret:Base");
                finalSolution[0] = Body.transform.eulerAngles.y*Mathf.Deg2Rad;
            }

            if (float.IsNaN(finalSolution[1]))
            {
                GameObject Joint2 = GameObject.Find("Barret:Join2");
                finalSolution[1] = Joint2.transform.eulerAngles.z * Mathf.Deg2Rad;

            }

            if (float.IsNaN(finalSolution[2]))
            {
                GameObject Joint2 = GameObject.Find("Barret:Join2");
                GameObject Joint3 = GameObject.Find("Barret:Joint3");
                finalSolution[2] = (Joint3.transform.eulerAngles.z- Joint2.transform.eulerAngles.z) * Mathf.Deg2Rad;

            }

            print(finalSolution[0]);
            print(finalSolution[1]);
            print(finalSolution[2]);

            dataFromClient = finalSolution[0].ToString() + "/" + finalSolution[1].ToString() + "/" + finalSolution[2].ToString();

        }
        else
        {
            dataFromClient = "OK";
        }

        Debug.Log(dataFromClient);
        Byte[] sendBytes = Encoding.ASCII.GetBytes(dataFromClient);
        networkStream.Write(sendBytes, 0, sendBytes.Length);
        //networkStream.Flush();
        //Console.WriteLine(" >> " + dataFromClient);

    }


    void OnGUI()
    {
        //GUI.Button(new Rect(250, 10, 150, 50), "Send2Robot");
        if (Input.GetKeyDown("space"))
        {
            flag = 0;
            connectMatlab(flag);

        }
        //GUI.Button(new Rect(550, 10, 150, 50), "Execute")
        if (Input.GetKeyDown(KeyCode.LeftControl) == true)
        {
            flag = 1;
            connectMatlab(flag);
             
        }
    }

}