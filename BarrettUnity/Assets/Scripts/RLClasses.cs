using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;

public class RLFunctions
{
    public double Alpha = 0.9;
    public int TimePenalty = -1;
    public int MissedValue = -30;

    public double Reward(GameManager.ProjectileInfo info, GameManager manager)
    {
        if (info.IsCaught == GameManager.Caught.Yes)
        {
            return -((manager.controller.xIntervalCount + manager.controller.yIntervalCount - 2) - ManhattanDistance(manager.GridCenter, info.StartPosition));
        }
        else
        {
            return MissedValue;
        }
    }

    public double RewardWithTime(GameManager.ProjectileInfo info, GameManager manager)
    {
        if(info.IsCaught == GameManager.Caught.Yes)
        {
            return -((manager.controller.xIntervalCount + manager.controller.yIntervalCount - 2) - ManhattanDistance(manager.GridCenter, info.StartPosition));
        }
        else
        {
            return MissedValue;
        }
    }

    private float ProjectileLifetime(GameManager manager, Pair startPosition)
    {
        Vector3 transformedDestination = manager.controller.UIContainer.transform.TransformPoint(new Vector3(manager.controller.xStartValue, manager.controller.yStartValue, 0));
        float ydistance = Mathf.Abs(manager.controller.GetBoxCoordinatesInWorldFrame(startPosition.x, startPosition.y).y - manager.mainCamera.ScreenToWorldPoint(new Vector3(transformedDestination.x, transformedDestination.y, 1.1f)).y);
        return ydistance / -Projectile.velocity;
    }

    public double Reward(GameManager manager, Pair startPosition, bool wasCaught)
    {
        if (wasCaught)
        {
            return -((manager.controller.xIntervalCount + manager.controller.yIntervalCount - 2) - ManhattanDistance(manager.GridCenter, startPosition));
            //return -(((manager.controller.xIntervalCount * 1.5) + manager.controller.yIntervalCount - 2) - ManhattanDistance(manager.GridCenter, startPosition));
        }
        else
        {
            return MissedValue;
        }
    }
    public double Reward(Pair location, GameManager manager)
    {
        return -((manager.controller.xIntervalCount + manager.controller.yIntervalCount - 2) - ManhattanDistance(manager.GridCenter, location));
    }

    //Rows and Columns should be actual number of rows and columns
    private int RewardGridLocation(Pair start_pos, int rows, int columns)
    {
        if (start_pos.x < (rows / 2))
        {
            //grid either 2 or 3
            if (start_pos.y < (columns / 2))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            //grid either 0 or 1
            if (start_pos.y < (columns / 2))
            {
                return 3;
            }
            else
            {
                return 2;
            }
        }
    }

    public double Score(List<GameManager.ProjectileInfo> projectiles, GameManager manager)
    {
        double score = 0;

        for (int i = 0; (i < projectiles.Count) && (projectiles[i].IsCompleted()); i++)
        {
            score += Reward(projectiles[i], manager);
        }

        return score;
    }

    public double Value(List<GameManager.ProjectileInfo> projectiles, GameManager manager)
    {
        double value = 0;

        for (int x = 0; (x < projectiles.Count) && (projectiles[x].IsCaught != GameManager.Caught.Undetermined); x++)
        {
            value += (Math.Pow(Alpha, x) * Reward(projectiles[x], manager));
        }

        return value;
    }

    /*private int ManhattanDistance(Pair user, Pair projectile)
    {
        //return (Math.Abs(user.x - projectile.x) - xmax) + (Math.Abs(user.y - projectile.y) - ymax);
        return (Math.Abs(user.x - projectile.x)) + (Math.Abs(user.y - projectile.y));
    }*/

    public string GenerateRewardTable(int rows, int columns)
    {
        string returnable = "";

        for (int j = columns - 1; j >= 0; j--)
        {
            for (int i = 0; i < rows; i++)
            {
                returnable += Reward(new Pair(i, j), GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>()).ToString() + " | ";
            }
            returnable += "\n";
        }
        return returnable;
    }

    private int ManhattanDistance(Pair user, Pair projectile)
    {
        return (Math.Abs(user.x - projectile.x) + Math.Abs(user.y - projectile.y));
    }

    private int TimeReward(GameManager.ProjectileInfo info)
    {
        return (TimePenalty * (int)((info.TimeLanded - info.TimeLaunched) / 1000));
    }

    public double RequestSimulationData(GameManager manager, int x, int y)
    {
        if(x >= manager.controller.xIntervalCount && y >= manager.controller.yIntervalCount)
        {
            throw new Exception("Invalid Grid Coordinates");
        }
        if (x < 0 || y < 0)
        {
            throw new Exception("Invalid Grid Coordinates");
        }
        Grid<bool> ActionResults = manager.controller.AI.GeneratePerformanceTable(Projectile.velocity);
        Debug.Log(ActionResults.ToString());
        return Reward(manager, new Pair(x, y), ActionResults[x, y]);
    }
}

/*public class ActionTable : IEnumerable
{
    public int Rows;
    public int Columns;

    public Grid this[int r, int c]
    {
        get
        {
            return Data[r, c];
        }
        set
        {
            Data[r, c] = value;
        }
    }

    public Grid this[Pair loc]
    {
        get
        {
            return Data[loc.x, loc.y];
        }
        set
        {
            Data[loc.x, loc.y] = value;
        }
    }

    Grid[,] Data;

    public ActionTable(int _rows, int _columns, bool _random = false, string _initializer = "", char _gridDelimiter = '|', char _valueDelimiter = ' ')
    {
        Rows = _rows;
        Columns = _columns;
        Data = new Grid[Rows, Columns];

        if (_random)
        {
            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    Data[x, y] = new Grid(Rows, Columns);
                    Data[x, y].SetRandomGrid();
                }
            }
        }
        else if (_initializer != "")
        {
            string[] results = _initializer.Split(_gridDelimiter);

            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    Data[x, y] = new Grid(Rows, Columns);
                }
            }

            int rcounter = 0, ccounter = 0;

            for (int x = 0; x < results.Length; x++)
            {
                Data[rcounter, ccounter].SetGridFromString(results[x], _valueDelimiter);
                ccounter++;

                if (ccounter == Columns)
                {
                    ccounter = 0;
                    rcounter++;

                    if (rcounter == Rows)
                    {
                        return;
                    }
                }
            }
        }
        else
        {
            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    Data[x, y] = new Grid(Rows, Columns);
                }
            }
        }
    }

    public override string ToString()
    {
        string returnable = "";

        for (int x = 0; x < Rows; x++)
        {
            for (int y = 0; y < Columns; y++)
            {
                returnable += "Grid at (" + x + "," + y + ")\n";
                returnable += Data[x, y].ToString() + "\n";
            }
        }
        return returnable;
    }

    public Pair GetNextPosition(Pair starting_position)
    {
        if (starting_position.IsValid(Rows, Columns))
        {
            return Data[starting_position.x, starting_position.y].GetTrueValue();
        }
        return new Pair(-1, -1);
    }

    public void GenerateTableFromDistance(double alpha, GameManager.Caught caught)
    {
        ActionTable position_table = new ActionTable(Rows, Columns, _random: true);

        for (int i = 0; i < position_table.Rows; i++)
        {
            for (int j = 0; j < position_table.Columns; j++)
            {
                for (int x = 0; x < position_table[i, j].Rows; x++)
                {
                    for (int y = 0; y < position_table[i, j].Columns; y++)
                    {
                        if (caught == GameManager.Caught.Yes)
                        {
                            position_table[i, j][x, y] = alpha * ManhattanDistance(new Pair(i, j), ConvertArraytoGraphCoordinates(new Pair(x, y)));
                        }
                        else
                        {

                        }
                    }
                }
            }
        }
        Debug.Log(position_table.ToString());
    }
    private Pair ConvertArraytoGraphCoordinates(Pair given)
    {
        return new Pair(given.y, Rows - 1 - given.x);
    }

    private int ManhattanDistance(Pair user, Pair projectile)
    {
        return Math.Abs(user.x - projectile.x) + Math.Abs(user.y - projectile.y);
    }

    public IEnumerator GetEnumerator()
    {
        return Data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}*/

public class ActionTable<T> : IEnumerable
{
    public int Rows;
    public int Columns;
    T ZeroValue;
    T OneValue;

    public Grid<T> this[int r, int c]
    {
        get
        {
            return Data[r, c];
        }
        set
        {
            Data[r, c] = value;
        }
    }

    public Grid<T> this[Pair loc]
    {
        get
        {
            return Data[loc.x, loc.y];
        }
        set
        {
            Data[loc.x, loc.y] = value;
        }
    }

    Grid<T>[,] Data;

    public ActionTable(int _rows, int _columns, T _zero, T _one, bool _random = false, string _initializer = "", char _gridDelimiter = '|', char _valueDelimiter = ' ')
    {
        Rows = _rows;
        Columns = _columns;
        Data = new Grid<T>[Rows, Columns];

        ZeroValue = _zero;
        OneValue = _one;

        if (_random)
        {
            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    Data[x, y] = new Grid<T>(Rows, Columns, ZeroValue, OneValue);
                    Data[x, y].SetRandomGrid();
                }
            }
        }
        else if (_initializer != "")
        {
            string[] results = _initializer.Split(_gridDelimiter);

            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    Data[x, y] = new Grid<T>(Rows, Columns, ZeroValue, OneValue);
                }
            }

            int rcounter = 0, ccounter = 0;

            for (int x = 0; x < results.Length; x++)
            {
                Data[rcounter, ccounter].SetGridFromString(results[x], _valueDelimiter);
                ccounter++;

                if (ccounter == Columns)
                {
                    ccounter = 0;
                    rcounter++;

                    if (rcounter == Rows)
                    {
                        return;
                    }
                }
            }
        }
        else
        {
            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    Data[x, y] = new Grid<T>(Rows, Columns, ZeroValue, OneValue);
                }
            }
        }
    }

    public override string ToString()
    {
        string returnable = "";

        for (int x = 0; x < Rows; x++)
        {
            for (int y = 0; y < Columns; y++)
            {
                returnable += "Grid at (" + x + "," + y + ")\n";
                returnable += Data[x, y].ToString() + "\n";
            }
        }
        return returnable;
    }

    public Pair GetNextPosition(Pair starting_position)
    {
        if (starting_position.IsValid(Rows, Columns))
        {
            return Data[starting_position.x, starting_position.y].GetTrueValue();
        }
        return new Pair(-1, -1);
    }

    public void GenerateTableFromDistance(double alpha, GameManager.Caught caught)
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                for (int x = 0; x < this[i, j].Rows; x++)
                {
                    for (int y = 0; y < this[i, j].Columns; y++)
                    {
                        if (caught == GameManager.Caught.Yes)
                        {
                            this[i, j][x, y] = (T)Convert.ChangeType(alpha * ManhattanDistance(new Pair(i, j), ConvertArraytoGraphCoordinates(new Pair(x, y))), typeof(T));
                        }
                        else
                        {
                            this[i, j][x, y] = (T)Convert.ChangeType(alpha * ManhattanDistance(new Pair(i, j), ConvertArraytoGraphCoordinates(new Pair(x, y))), typeof(T));
                        }
                    }
                }
            }
        }
    }
    private Pair ConvertArraytoGraphCoordinates(Pair given)
    {
        return new Pair(given.y, Rows - 1 - given.x);
    }

    private int ManhattanDistance(Pair user, Pair projectile)
    {
        return Math.Abs(user.x - projectile.x) + Math.Abs(user.y - projectile.y);
    }

    public IEnumerator GetEnumerator()
    {
        return Data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}

/*
public class Grid : IEnumerable
{
    public int Rows;
    public int Columns;

    public int this[int r, int c]
    {
        get
        {
            return Data[r, c];
        }
        set
        {
            Data[r, c] = value;
        }
    }

    public int this[Pair loc]
    {
        get
        {
            return Data[loc.x, loc.y];
        }
        set
        {
            Data[loc.x, loc.y] = value;
        }
    }

    int[,] Data;

    public Grid(int _rows, int _columns)
    {
        Rows = _rows;
        Columns = _columns;
        Data = new int[Rows, Columns];
    }
    public void Clear()
    {
        Data = new int[Rows, Columns];
    }
    public Pair GetTrueValue()
    {
        for (int x = 0; x < Rows; x++)
        {
            for (int y = 0; y < Columns; y++)
            {
                if (Data[x, y] == 1)
                {
                    return new Pair(x, y);
                }
            }
        }
        return new Pair(-1, -1);
    }
    public void SetGridFromString(string values, char separator)
    {
        string[] result = values.Split(separator);
        int rcounter = 0, ccounter = 0;

        for (int x = 0; x < result.Length; x++)
        {
            if (result[x].Trim() != "")
            {
                Data[rcounter, ccounter] = int.Parse(result[x]);
                ccounter++;
                if (ccounter == Columns)
                {
                    ccounter = 0;
                    rcounter++;

                    if (rcounter == Rows)
                    {
                        return;
                    }
                }
            }
        }
    }
    public void SetRandomGrid()
    {
        Zeros();
        Data[UnityEngine.Random.Range(0, Rows - 1), UnityEngine.Random.Range(0, Columns - 1)] = 1;
    }
    private void Zeros()
    {
        for (int x = 0; x < Rows; x++)
        {
            for (int y = 0; y < Columns; y++)
            {
                Data[x, y] = 0;
            }
        }
    }
    public override string ToString()
    {
        string returnable = "";
        for (int x = 0; x < Rows; x++)
        {
            for (int y = 0; y < Columns; y++)
            {
                returnable += Data[x, y] + " ";
            }
            returnable += "\n";
        }
        return returnable;
    }

    public IEnumerator GetEnumerator()
    {
        return Data.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}
*/
public class Grid<T> : IEnumerable
{
    public int Rows;
    public int Columns;
    T ZeroValue;
    T OneValue;

    public T this[int r, int c]
    {
        get
        {
            return Data[r, c];
        }
        set
        {
            Data[r, c] = value;
        }
    }

    public T this[Pair loc]
    {
        get
        {
            return Data[loc.x, loc.y];
        }
        set
        {
            Data[loc.x, loc.y] = value;
        }
    }

    T[,] Data;

    public Grid(int _rows, int _columns, T _zero, T _one)
    {
        Rows = _rows;
        Columns = _columns;
        Data = new T[Rows, Columns];
        ZeroValue = _zero;
        OneValue = _one;
    }
    public void Clear()
    {
        Data = new T[Rows, Columns];
    }
    public Pair GetTrueValue()
    {
        for (int x = 0; x < Rows; x++)
        {
            for (int y = 0; y < Columns; y++)
            {
                if (Data[x, y].Equals(OneValue))
                {
                    return new Pair(x, y);
                }
            }
        }
        return new Pair(-1, -1);
    }
    public void SetGridFromString(string values, char separator)
    {
        string[] result = values.Split(separator);
        int rcounter = 0, ccounter = 0;

        for (int x = 0; x < result.Length; x++)
        {
            if (result[x].Trim() != "")
            {
                Data[rcounter, ccounter] = (T)Convert.ChangeType(result[x], typeof(T));
                ccounter++;
                if (ccounter == Columns)
                {
                    ccounter = 0;
                    rcounter++;

                    if (rcounter == Rows)
                    {
                        return;
                    }
                }
            }
        }
    }
    public void SetRandomGrid()
    {
        Zeros();
        Data[UnityEngine.Random.Range(0, Rows - 1), UnityEngine.Random.Range(0, Columns - 1)] = OneValue;
    }

    private void Zeros()
    {
        for (int x = 0; x < Rows; x++)
        {
            for (int y = 0; y < Columns; y++)
            {
                Data[x, y] = ZeroValue;
            }
        }
    }
    public override string ToString()
    {
        string returnable = "";
        for (int x = 0; x < Rows; x++)
        {
            for (int y = 0; y < Columns; y++)
            {
                returnable += Data[x, y] + " ";
            }
            returnable += "\n";
        }
        return returnable;
    }

    public IEnumerator GetEnumerator()
    {
        return Data.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}

public class Pair
{
    public int x;
    public int y;

    public Pair(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public Pair(string _initializer, char first_separator = '(', char num_separator = ',', char second_separator = ')')
    {
        string[] numbers = _initializer.Split(first_separator)[1].Split(second_separator)[0].Split(num_separator);
        x = int.Parse(numbers[0]);
        y = int.Parse(numbers[1]);
    }

    public bool IsValid()
    {
        if (x >= 0 && y >= 0)
        {
            return true;
        }
        return false;
    }
    public bool IsValid(int rows, int columns)
    {
        if (x >= 0 && x < rows && y >= 0 && y < rows)
        {
            return true;
        }
        return false;
    }

    public static Pair FromString(string input)
    {
        return new Pair(int.Parse(input.Split(',')[0].Split('(')[1]), int.Parse(input.Split(',')[1].Split(')')[0]));
    }

    public override string ToString()
    {
        return "(" + x + "," + y + ")";
    }

    public static explicit operator Vector2(Pair pair)
    {
        return new Vector2(pair.x, pair.y);
    }

    public static bool operator <(Pair first, Pair second)
    {
        if (first.x < second.x)
        {
            if (first.y < second.y)
            {
                return true;
            }
        }
        return false;
    }

    public static bool operator >(Pair first, Pair second)
    {
        if (first.x > second.x)
        {
            if (first.y > second.y)
            {
                return true;
            }
        }
        return false;
    }

    public static bool operator ==(Pair first, Pair second)
    {
        if (first.x == second.x && first.y == second.y)
        {
            return true;
        }
        return false;
    }

    public static bool operator !=(Pair first, Pair second)
    {
        if (first.x != second.x || first.y != second.y)
        {
            return true;
        }
        return false;
    }
}
