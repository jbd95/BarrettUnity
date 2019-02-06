using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FatigueDetection : MonoBehaviour
{

    Vector3 LastPosition;
    float LastTime;
    float CurrentTime;
    GameManager manager;
    List<float> CalculatedVelocities = new List<float>();
    ComputerPlayer2 CP;
    FatigueTable Table;
    GameManager.ProjectileInfo TrackedProjectile = null;
    public Text DisplayFatigue;

    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        LastPosition = transform.position;
        LastTime = -1;
        CP = GetComponent<ComputerPlayer2>();
        Table = new FatigueTable(manager.controller.xIntervalCount, manager.controller.yIntervalCount);
    }

    public void FixedUpdate()
    {
        /*if (TrackedProjectile == null)
        {
            TrackedProjectile = manager.GetVisibleProjectile();
        }
        if (TrackedProjectile != null)
        {
            if (IsPlayerMoving(LastPosition) && manager.IsProjectileVisible())
            {
                CalculatedVelocities.Add(Vector3.Distance(LastPosition, transform.position) / (Time.fixedDeltaTime));
            }
            else if (!manager.IsProjectileVisible() && CalculatedVelocities.Count > 0)
            {
                Table[TrackedProjectile.StartPosition].Add(AverageCalculatedVelocities());
                TrackedProjectile = null;
                DisplayFatigue.text = (int)Table.CalculateFatiguePercentage() + "%";
             
            }
            LastPosition = transform.position;
        }*/
        
    }

    private float AverageCalculatedVelocities()
    {
        if (CalculatedVelocities.Count == 0)
        {
            return 0;
        }
        float average = 0;
        foreach (float currentvel in CalculatedVelocities)
        {
            average += currentvel;
        }
        average /= CalculatedVelocities.Count;
        Debug.Log("Estimated: " + average + "  Actual: " + CP.Velocity);
        CalculatedVelocities.Clear();
        return average;
    }

    private bool IsPlayerMoving(Vector3 LastPosition)
    {
        return LastPosition != transform.position;
    }
}

public class FatigueTable
{
    public List<float> this[int r, int c]
    {
        get
        {
            return Table[r, c];
        }
        set
        {
            Table[r, c] = value;
        }
    }
    public List<float> this[Pair loc]
    {
        get
        {
            return Table[loc.x, loc.y];
        }
        set
        {
            Table[loc.x, loc.y] = value;
        }
    }
    public List<float>[,] Table;
    int Rows;
    int Columns;
    public FatigueTable(int _rows, int _columns)
    {
        Rows = _rows;
        Columns = _columns;
        Table = new List<float>[Rows, Columns];

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                this[i, j] = new List<float>();
            }
        }
    }
    public void AddValue(int r, int c, float value)
    {
        this[r, c].Add(value);
    }
    public void AddValue(Pair location, float value)
    {
        AddValue(location.x, location.y, value);
    }

    /*public double AddValue(Pair location, float value)
    {

    }*/

    /*public double CalculateFatiguePercentage()
    {
        Matrix velocities = new Matrix(Rows, Columns, _zeros: true);
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (this[i, j].Count > 0)
                {
                    double sum = 0;
                    foreach (float velocity in this[i, j])
                    {
                        sum += velocity;
                    }
                    sum /= this[i, j].Count;
                    velocities[i, j] = Mathf.Abs((float)((this[i, j][this[i, j].Count - 1] - sum) / sum));
                }
            }
        }

        int non_zero_count = 0;
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (velocities[i, j] > 0)
                {
                    non_zero_count++;
                }
            }
        }

        double final = 0;
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                final += velocities[i, j];
            }
        }
        final /= non_zero_count;
        return final * 100;
    }*/

    /*public double CalculateFatiguePercentage()
    {
        Matrix velocities = new Matrix(Rows, Columns, _zeros: true);

        for(int i = 0; i < Rows; i++)
        {
            for(int j = 0; j < Columns; j++)
            {
                if(this[i, j].Count > 1)
                {
                    double temp = this[i, j][this[i, j].Count - 1];
                    velocities[i, j] = Mathf.Abs((float)((temp - this[i, j][0]) / temp));
                }
            }
        }

        int nonzero_count = 0;
        for(int i = 0; i < Rows; i++)
        {
            for(int j = 0; j < Columns; j++)
            {
                if(velocities[i, j] > 0)
                {
                    nonzero_count++;
                }
            }
        }
    }*/
}
