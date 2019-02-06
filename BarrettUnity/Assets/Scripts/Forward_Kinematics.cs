using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Forward_Kinematics : MonoBehaviour
{
   // double[,] result2;

    void Start()
    {
        ActionTable<double> table = new ActionTable<double>(4, 4, _zero:0, _one:1);
        table.GenerateTableFromDistance(0.5, GameManager.Caught.Yes);


        ActionTable<GameManager.Caught> testing = new ActionTable<GameManager.Caught>(4, 4, _random:true, _zero: GameManager.Caught.No, _one: GameManager.Caught.Yes);
        Debug.Log(testing.ToString());

        Matrix4x4 test = new Matrix4x4();
        Matrix4x4 test2 = new Matrix4x4();

        var test3 = test * test2;
    }

    public void Print()
    {
        //PrintMatrix(result2, 4, 4);
    }

    public void Move(double j1, double j2, double j3, double j4)
    {
        //var result = ForwardKinematics(new double[] { j1, j2, j3, j4 });
        //transform.position = new Vector3(0f, (float)result[1, 3], (float)result[2, 3]);
        //result2 = result;

        var result = ForwardKinematics(new double[] { j1, j2, j3, j4 });
        transform.position = new Vector3(0f, (float)result[1, 3], (float)result[2, 3]);
    }


    public Matrix ForwardKinematics(double[] q)
    {
        int size = q.Length;

        Matrix DH_Table = new Matrix(new double[,] { { 0, ((-1) * Math.PI / 2), 0, q[0] }, { 0, Math.PI / 2, 0, q[1] }, { 0.045, ((-1) * Math.PI / 2), 0.55, q[2] }, { -0.045, Math.PI / 2, 0, q[3] }, { 0, 0, 0.35, 0 } });

        List<Matrix> T = new List<Matrix>();

        for (int i = 0; i < size + 1; i++)
        {
            T.Add(new Matrix(4, 4, _zeros: true));
        }

        for (int i = 0; i < T.Count; i++)
        {
            T[i] = DH_Matrix(DH_Table[i, 0], DH_Table[i, 1], DH_Table[i, 2], DH_Table[i, 3]);
            T[i].RoundToZero();
        }

        Matrix rotate_z = new Matrix(new double[,] { { -1, 0, 0 }, { 0, -1, 0 }, { 0, 0, 1 } });
        Matrix rotate_x = new Matrix(new double[,] { { 1, 0, 0 }, { 0, 0, -1 }, { 0, 1, 0 } });

        Matrix rotation = rotate_z * rotate_x;
        Matrix T_e = rotation.HomogeneousTransform();

        for (int i = 0; i < T.Count; i++)
        {
            T_e = T_e * T[i];
        }

        T_e[2, 3] *= -1;

        return T_e;
    }
    Matrix DH_Matrix(double a, double alpha, double d, double theta)
    {
        Matrix matrix = new Matrix(new double[,]{ { Math.Cos(theta), (-1) * (Math.Sin(theta) * Math.Cos(alpha)), Math.Sin(theta) * Math.Sin(alpha), a * Math.Cos(theta) }, { Math.Sin(theta), Math.Cos(theta) * Math.Cos(alpha), (-1) * Math.Cos(theta) * Math.Sin(alpha), a * Math.Sin(theta) }, { 0, Math.Sin(alpha), Math.Cos(alpha), d }, { 0, 0, 0, 1 } });
        return matrix;
    }


    /*double[,] DH_Matrix(double a, double alpha, double d, double theta)
    {
        double[,] matrix = { { Math.Cos(theta), (-1) * (Math.Sin(theta) * Math.Cos(alpha)), Math.Sin(theta) * Math.Sin(alpha), a * Math.Cos(theta) }, { Math.Sin(theta), Math.Cos(theta) * Math.Cos(alpha), (-1) * Math.Cos(theta) * Math.Sin(alpha), a * Math.Sin(theta) }, { 0, Math.Sin(alpha), Math.Cos(alpha), d }, { 0, 0, 0, 1 } };
        return matrix;
    }

    public double[,] ForwardKinematics(double[] q)
    {
        int size = q.Length;

        double[,] DH_Table = { { 0, ((-1) * Math.PI / 2), 0, q[0] }, { 0, Math.PI / 2, 0, q[1] }, { 0.045, ((-1) * Math.PI / 2), 0.55, q[2] }, { -0.045, Math.PI / 2, 0, q[3] }, { 0, 0, 0.35, 0 } };

        List<double[,]> T = new List<double[,]>();

        for (int i = 0; i < size + 1; i++)
        {
            T.Add(new double[4, 4] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } });
        }

        for (int i = 0; i < T.Count; i++)
        {
            T[i] = DH_Matrix(DH_Table[i, 0], DH_Table[i, 1], DH_Table[i, 2], DH_Table[i, 3]);
            T[i] = RoundtoZero(T[i], 4, 4);
        }

        //double[,] T_e = HomogeneousTransform(MatrixMultiplication(MatrixMultiplication(ZRotation(180), XRotation(90), 3, 3, 3, 3), ZRotation(-90), 3, 3, 3, 3), 3, 3);
        double[,] T_e = HomogeneousTransform(MatrixMultiplication(ZRotation(180), XRotation(90), 3, 3, 3, 3), 3, 3);
        //double[,] T_e = HomogeneousTransform(ZRotation(180), 3, 3);
        //T_e[0, 3] = -1;
        //PrintMatrix(T_e, 4, 4);
        //double[,] T_e = new double[4, 4] { { 1,0,0,0 }, { 0,0,1,0 }, { 0,-1,0,0 }, { 0,0,0,1 } };
        //double[,] T_e = T[0];

        for (int i = 0; i < T.Count; i++)
        {
            T_e = MatrixMultiplication(T_e, T[i], 4, 4, 4, 4);
        }

        T_e[2, 3] *= -1;

        return T_e;
    }

    public double[,] MatrixMultiplication(double[,] first, double[,] second, int frow, int fcolumn, int srow, int scolumn)
    {
        double[,] result = new double[frow, scolumn];

        for (int i = 0; i < frow; i++)
        {
            for (int j = 0; j < scolumn; j++)
            {
                result[i, j] = GetMatrixValue(first, second, i, j, frow, fcolumn, srow, scolumn);
            }
        }
        return result;
    }

    public double[,] RoundtoZero(double[,] matrix, int row, int column)
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                matrix[i, j] = RoundtoZero(matrix[i, j]);
            }
        }
        return matrix;
    }

    private double RoundtoZero(double num)
    {
        if (Math.Abs(num) < (0.0000001))
        {
            return 0;
        }
        return num;
    }

    private double GetMatrixValue(double[,] first, double[,] second, int desired_row, int desired_column, int frow, int fcolumn, int srow, int scolumn)
    {
        double[] row = new double[fcolumn];

        for (int i = 0; i < frow; i++)
        {
            if (i == desired_row)
            {
                for (int j = 0; j < fcolumn; j++)
                {
                    row[j] = first[i, j];
                }
            }
        }

        double[] column = new double[srow];

        for (int j = 0; j < scolumn; j++)
        {
            if (j == desired_column)
            {
                for (int i = 0; i < srow; i++)
                {
                    column[i] = second[i, j];
                }
            }
        }

        double final = 0;
        for (int i = 0; i < srow; i++)
        {
            final += (row[i] * column[i]);
        }

        return final;
    }

    private double[,] ZRotation(double degrees)
    {
        double[,] matrix = new double[,] { { Math.Cos(degrees * Mathf.Deg2Rad), -Math.Sin(degrees * Mathf.Deg2Rad), 0}, { Math.Sin(degrees * Mathf.Deg2Rad), Math.Cos(degrees * Mathf.Deg2Rad), 0 }, { 0,0,1 } };
        return RoundtoZero(matrix, 3, 3);
    }

    private double[,] XRotation(double degrees)
    {
        double[,] matrix = new double[,] { { 1, 0, 0 }, { 0, Math.Cos(degrees * Mathf.Deg2Rad), -Math.Sin(degrees * Mathf.Deg2Rad) }, { 0, Math.Sin(degrees * Mathf.Deg2Rad), Math.Cos(degrees * Mathf.Deg2Rad) } };
        return RoundtoZero(matrix, 3, 3);
    }

    private double[,] YRotation(double degrees)
    {
        double[,] matrix = new double[,] { { Math.Cos(degrees * Mathf.Deg2Rad), 0, Math.Sin(degrees * Mathf.Deg2Rad)}, { 0, 1, 0 }, { -Math.Sin(degrees * Mathf.Deg2Rad), 0, Math.Cos(degrees * Mathf.Deg2Rad)} };
        return RoundtoZero(matrix, 3, 3);
    }

    private double[,] HomogeneousTransform(double[,] matrix, int rows, int columns)
    {
        double[,] new_matrix = new double[rows + 1, columns + 1];

        for(int x = 0; x < rows; x++)
        {
            for(int y = 0; y < columns; y++)
            {
                new_matrix[x, y] = matrix[x, y];
            }
        }
        for(int x = 0; x < columns; x++)
        {
            new_matrix[rows, x] = 0;
        }
       new_matrix[rows, columns] = 1;

        for(int x = 0; x < rows; x++)
        {
            new_matrix[x, columns] = 0;
        }
        return new_matrix;
    }


    void PrintMatrix(double[,] matrix, int rows, int columns)
    {
        for (int i = 0; i < rows; i++)
        {
            string row = "";
            for (int j = 0; j < columns; j++)
            {
                if (j != (columns - 1))
                {
                    row += matrix[i, j] + ", ";
                }
                else
                {
                    row += matrix[i, j];
                }
            }
            Debug.Log("Matrix at row " + i + ": " + row);
        }
    }*/
}

public class Matrix
{
    public int Rows;
    public int Columns;

    public double this[int r, int c]
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

    private double[,] Data;

    public Matrix(int _rows, int _columns, bool _zeros=false)
    {
        Rows = _rows;
        Columns = _columns;

        Data = new double[Rows, Columns];

        if(_zeros)
        {
            Zeros();
        }
    }

    public Matrix(double[,] _data)
    {
        Data = _data;
        Rows = Data.GetLength(0);
        Columns = Data.GetLength(1);
    }

    public void Zeros()
    {
        for(int i = 0; i < Rows; i++)
        {
            for(int j = 0; j < Columns; j++)
            {
                this[i, j] = 0;
            }
        }
    }

    public override string ToString()
    {
        string returnable = "";
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (j != (Columns - 1))
                {
                    returnable += Data[i, j] + ", ";
                }
                else
                {
                    returnable += Data[i, j];
                }
            }
            returnable += "\n";
        }
        return returnable;
    }

    public Matrix HomogeneousTransform()
    {
        Matrix new_matrix = new Matrix(Rows + 1, Columns + 1);
        for (int x = 0; x < Rows; x++)
        {
            for (int y = 0; y < Columns; y++)
            {
                new_matrix[x, y] = this[x, y];
            }
        }
        for (int x = 0; x < Columns; x++)
        {
            new_matrix[Rows, x] = 0;
        }
        new_matrix[Rows, Columns] = 1;

        for (int x = 0; x < Rows; x++)
        {
            new_matrix[x, Columns] = 0;
        }
        return new_matrix;
    }

    public static Matrix operator*(Matrix first_matrix, Matrix second_matrix)
    {
        if(first_matrix.Columns != second_matrix.Rows)
        {
            throw new Exception("Matrices Size Error");
        }

        Matrix result = new Matrix(first_matrix.Rows, second_matrix.Columns);

        for (int i = 0; i < first_matrix.Rows; i++)
        {
            for (int j = 0; j < second_matrix.Columns; j++)
            {
                double[] row = new double[first_matrix.Columns];

                for (int x = 0; x < first_matrix.Rows; x++)
                {
                    if (x == i)
                    {
                        for (int y = 0; y < first_matrix.Columns; y++)
                        {
                            row[y] = first_matrix[x, y];
                        }
                    }
                }

                double[] column = new double[second_matrix.Rows];

                for (int y = 0; y < second_matrix.Columns; y++)
                {
                    if (y == j)
                    {
                        for (int x = 0; x < second_matrix.Rows; x++)
                        {
                            column[x] = second_matrix[x, y];
                        }
                    }
                }

                result[i, j] = 0;
                for (int z = 0; z < second_matrix.Rows; z++)
                {
                    result[i, j] += (row[z] * column[z]);
                }
            }
        }
        return result;
    }

    public void RoundToZero()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (Math.Abs(this[i,j]) < (0.0000001))
                {
                    this[i, j] = 0;
                }
            }
        }
    }
}
