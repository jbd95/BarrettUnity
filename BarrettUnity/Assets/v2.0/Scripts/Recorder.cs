using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Recorder : MonoBehaviour {
    Vector3 position;
    public Forward_Kinematics FE;
    List<string> Time_Stamp = new List<string>();
    protected static T[,] ResizeArray1<T>(ref T[,] original, int rows, int cols)
    {
        var newArray = new T[rows, cols];
        int minRows = Math.Min(rows, original.GetLength(0));
        int minCols = Math.Min(cols, original.GetLength(1));
        int a=0;
        int j=0;
        for (a = 0; a < minRows; a++)
            for (j = 0; j < minCols; j++)
                newArray[a, j] = original[a, j];
        return newArray;
    }

    public class PosList<T>
    {
        T[,] items = new T[5,3];
        int count;
        public void Add(T Px, T Py, T Pz)
        {
            if (count == items.GetLength(0))
            {
                items = ResizeArray1(ref items,(items.GetLength(0)+5), 3);
            }
            items[count, 0] = Px;
            items[count, 1] = Py;
            items[count++, 2] = Pz;
        }
        public T this[int index1, int index2]
        {
            get
            {
                if (index1 >= count || index1 < 0)
                    throw new IndexOutOfRangeException();
                return items[index1, index2];
            }
        }

        public T[,] ToArray()
        {
            T[,] ret = new T[count, 3];
            ret = items.Clone() as T[,];
            return ret;
        }

        public int[] ArrLen()
        {
            int[] ret = new int[] { count, 3 };
            return ret;
        }
    }

    PosList<float> P = new PosList<float>();

    // Use this for initialization
    void Start () {
        //position = transform.position;
        position = FE.EEPosition;
        //Vector3 position2 = transform.position;
        float P1 = position[0];
        float P2 = position[1];
        float P3 = position[2];
        P.Add(P1,P2,P3);
        Time_Stamp.Add(DateTime.Now.ToString("HH:mm:ss:fffff"));
        //Debug.Log("Object Pos: "+position2+" Act Pos: "+ position);
    }
	
	// Update is called once per frame
	void Update () {
        //position= transform.position;
        position = FE.EEPosition;
        //Vector3 position2 = transform.position;
        float P1 = position[0];
        float P2 = position[1];
        float P3 = position[2];
        P.Add(P1, P2, P3);
        Time_Stamp.Add(DateTime.Now.ToString("HH:mm:ss:fffff"));
        //Debug.Log("Object Pos: " + position2 + " Act Pos: " + position);
    }

    public void SaveDat(string Fname, List<string> ScoreDat)
    {
        int[] Alen = P.ArrLen();

        float[,] ADat = P.ToArray();

        using (var w = File.AppendText(Fname + "trajectory.csv"))
        {
            for (int a = 0; a < Alen[0]; a++)
            {
                string line = (Time_Stamp[a].ToString() + "," + ADat[a, 0].ToString("F9") + "," + ADat[a, 1].ToString("F9") + "," + ADat[a, 2].ToString("F9"));
                w.WriteLine(line);
                w.Flush();
            }

        }

        using (var k = File.AppendText(Fname + "score.csv"))
        {
            foreach (var SDat in ScoreDat)
            {
                k.WriteLine(SDat);
                k.Flush();
            }

        }
    }
}
