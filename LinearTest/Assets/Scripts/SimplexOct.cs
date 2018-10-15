//Linear programming, Simplex algorithm
//Harald Kneidinger

using System;
using System.Collections.Generic;
using UnityEngine;

public class SimplexOct : MonoBehaviour
{
    #region Membervariables
    private double[,] tableau;
    private List<double[,]> alltableaus;
    private int[] variables;
    private int pivotrow;
    private int pivotcolumn;
    private double targetfvalue;
    private double[] solutionvector;
    private SOL solution;
    #endregion //Membervariables


    public enum SOL
    {
        NOT_EVAL,           //not evaluated
        OPT,                //optimum solution reached
        OPT_DEG_CORNER,     //optimum solution, but degenerate corner
        RANGE_EMPTY,        //admissability range is empty
        INF,                //there is an infinite number of solutions
        NOT_OPT,            //currently not the optimum solution
        NONE,               //the tableau is not solveable
    }


    //public void SimplexOct() { }


    /// <summary>
    /// Initalize the Simplex algorithm with the starting tableau, derived from
    /// the linear equations. 
    /// </summary>
    /// <param name="tableau">The starting tableau</param>
    /// <param name="variables">the marking row -1 means NBV, all other numbers
    /// represent the BVs</param>
    public void Init(double[,] tableau, int[] variables)
    {
        //this.tableau = (double[,])tableau.Clone();
        this.tableau = tableau;
        pivotcolumn = 0;
        pivotrow = 0;
        this.variables = variables;
        this.solutionvector = new Double[tableau.GetLength(1) - 1];
        targetfvalue = 0.0;
        alltableaus = new List<double[,]>();
        alltableaus.Add((double[,])tableau.Clone());
    }


    /// <summary>
    /// Iteratively solve the linear problem.
    /// </summary>
    public void Solve()
    {
        try
        {
            //loop through as long as no optimal solution is found.
            //if there is no solution or a degenerate corner, an exception is thrown.
            while (CheckTableau() == SOL.NOT_OPT)
            {
                SetPivotRowCol();
                ChangeTableau();
                Result();
            }

        }
        catch (NoSolutionException) { solution = SOL.NONE; }
        catch (DegenerateCornerException) { solution = SOL.OPT_DEG_CORNER; }

    }

    private SOL CheckTableau()
    {
        //--------------------------------------------------
        //Check the cost function for the opt. solution

        for (int i = 0; i < variables.Length; i++)
        {

            if (variables[i] == -1)
            {
                //if on if the NBVs is less than zero, the optimum is not found yet.
                if (tableau[tableau.GetLength(0) - 1, i + 1] < 0.0)
                    return this.solution = SOL.NOT_OPT;

                else if (tableau[tableau.GetLength(0) - 1, i + 1] == 0.0)
                    //if one or more NBV coefficient in the cost function is zero, 
                    //there is an inifinite number of solution. S.14 (iv)
                    return this.solution = SOL.INF;

            }

        }

        if (targetfvalue < 0.0)
            //all NBV values in the cost function are non negative.
            //target function value is < 0   -->  admissability range is empty.
            return this.solution = SOL.RANGE_EMPTY;
        else
            return this.solution = SOL.OPT;
        //---------------------------------------------
    }

    /// <summary>
    /// Search for the pivot element in the tableau
    /// </summary>
    private void SetPivotRowCol()
    {

        double minval = 0.0;
        double minvaltemp = 0.0;
        int minrowindex = 0;
        int mincolindex = 0;

        //---------------------------------------------------------------
        //search for the minimum coefficients of all the non basic variables in the cost function
        for (int i = 0; i < variables.Length; i++)
        {
            if (variables[i] == -1)
            {
                if (tableau[tableau.GetLength(0) - 1, i + 1] <= minval)
                {
                    minval = tableau[tableau.GetLength(0) - 1, i + 1];
                    mincolindex = i + 1;
                }
            }
        }
        //---------------------------------------------------------------



        //---------------------------------------------------------------
        //search for the minimum positiv value of the division 
        //of the elements in the beta value (left side) devided by the pivot column elements			
        minval = Double.MaxValue;

        for (int i = 0; i < tableau.GetLength(0) - 1; i++)
        {
            minvaltemp = tableau[i, 0] / tableau[i, mincolindex];
            if (minvaltemp >= 0.0 && minvaltemp < minval)
            {
                minval = minvaltemp;
                minrowindex = i;
            }
        }

        pivotrow = minrowindex;
        pivotcolumn = mincolindex;
        //---------------------------------------------------------------




        //--------------------------------------------------------------
        //Change the marking row (first row) entry of the pivot column element
        //to a BV and the element which was a BV gets a NBV
        for (int i = 0; i < variables.Length; i++)
        {
            Debug.Log("check if variables[" + i + "] (" + variables[i] + ") = " + pivotrow);
            if (variables[i] == pivotrow)
                variables[i] = -1;
        }
        variables[pivotcolumn - 1] = pivotrow;
        Debug.Log("variables[" + (pivotcolumn-1) + "] = " + pivotrow);
        //----------------------------------------------------------------

    }


    /// <summary>
    /// Changing the BVs and the NBVs
    /// </summary>
    private void ChangeTableau()
    {
        //------------------------------------------
        //if all elements of the pivotcolumn are <=0.0 the LOP is NOT solveable
        //This check is done before the actual changing of NBVs and BVs
        int counter = 0;

        for (int i = 0; i < tableau.GetLength(0) - 1; i++)
        {
            if (tableau[i, pivotcolumn] <= 0.0)
                counter++;
        }
        //if all the values in the pivot column are less or equal zero, there is no solution
        if (counter == tableau.GetLength(0) - 1)
            throw (new NoSolutionException());

        //-------------------------------------------





        double[,] arr_temp = new double[tableau.GetLength(0), tableau.GetLength(1)];
        Array.Copy(tableau, arr_temp, arr_temp.Length);
        //get the pivot element
        double pivotelement = tableau[pivotrow, pivotcolumn];

        double val = 0;

        //go through the tableau rows
        for (int i = 0; i < arr_temp.GetLength(0); i++)
        {
            //calculate the value that leads to the zeros in the pivot column when it is substracted from the
            //element value
            val = tableau[i, pivotcolumn] / pivotelement;

            //go through the tableau columns
            for (int j = 0; j < arr_temp.GetLength(1); j++)
            {

                if (i == pivotrow)
                    //the elements of the pivot row must all be devided by the pivotelement
                    //at the pivot element this leads to a 1 entry
                    arr_temp[i, j] = tableau[i, j] / pivotelement;

                else if (j == pivotcolumn)
                    arr_temp[i, j] = 0.0;
                else
                    //substract the value from all other entries
                    arr_temp[i, j] = tableau[i, j] - tableau[pivotrow, j] * val;

            }
        }

        Array.Copy(arr_temp, tableau, arr_temp.Length);
        alltableaus.Add((double[,])tableau.Clone());


    }

    /// <summary>
    /// Extract the solution from the tableau and evaluate it.
    /// </summary>
    /// <returns>Type of solution</returns>
    private void Result()
    {
        //-------------------------------------------------
        //extract the current solutionvector of the cost function value
        int zeroscounter = 0;

        for (int i = 0; i < variables.Length; i++)
        {
            if (variables[i] == -1)
                solutionvector[i] = 0;
            else
                //solution of the basic variables on the left side of the tableau
                solutionvector[i] = tableau[variables[i], 0];

            if (Math.Abs(solutionvector[i]) < 1e-14)
                zeroscounter++;
        }

        targetfvalue = tableau[tableau.GetLength(0) - 1, 0];

        //find out if it's a degenerated corner
        //if a beta value gets zero  (|zeros| > n - m )
        if (zeroscounter > variables.Length - (tableau.GetLength(0) - 1))
        {
            this.solution = SOL.OPT_DEG_CORNER;
            throw (new DegenerateCornerException());
        }
        //--------------------------------------------------

    }


    public int[] GetRelationRow(double[,] tableau)
    {
        int[] variables = new int[tableau.GetLength(1) - 1];
        int countones = 0;
        int countzeros = 0;
        int index = 0;
        for (int j = 1; j < tableau.GetLength(1); j++)
        {
            for (int i = 0; i < tableau.GetLength(0) - 1; i++)
            {
                if (tableau[i, j] == 1.0)
                {
                    index = i;
                    countones++;
                }
                else if (tableau[i, j] == 0.0)
                    countzeros++;

            }
            if (countones == 1 && countzeros == tableau.GetLength(0) - 2)
                variables[j - 1] = index;
            else
                variables[j - 1] = -1;

            countones = 0;
            countzeros = 0;

        }
        return variables;
    }

    public string AllTableausToString()
    {
        string s = "";
        int tabcount = 0;

        foreach (double[,] arr in alltableaus)
        {
            s += "\nTableau " + tabcount + ":\n";

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    s += String.Format("{0,-10:0.000}", arr[i, j]);
                }
                s += "\n";

            }
            tabcount++;

        }

        s += "Solution Vector:\n          ";

        for (int i = 0; i < this.solutionvector.Length; i++)
        {
            s += String.Format("{0,-10:0.000}", this.solutionvector[i]);
        }

        s += "\nCosts:   " + String.Format("{0,-10:0.0000}", targetfvalue) + "\n";

        s += "Solution:   " + solution + "\n";

        System.IO.File.WriteAllText(@"F:\LinearTest\CalculatedMineralogyGithub\CalculatedMineralogyGithub\WriteText.txt", s);

        return s;
    }


    public override string ToString()
    {
        string s = "Tableau:\n";

        for (int i = 0; i < this.tableau.GetLength(0); i++)
        {
            for (int j = 0; j < this.tableau.GetLength(1); j++)
            {
                s += String.Format("{0,-10:0.000}", this.tableau[i, j]);
            }
            s += "\n";

        }

        s += "\nSolution Vector:\n          ";

        for (int i = 0; i < this.solutionvector.Length; i++)
        {
            s += String.Format("{0,-10:0.000}", this.solutionvector[i]);
        }

        s += "\nCosts:   " + String.Format("{0,-10:0.0000}", targetfvalue) + "\n";

        s += "Solution:   " + solution + "\n";

        return s;
    }

    #region Properties

    /// <summary>
    /// Returns the Tableau
    /// </summary>
    public double[,] Tableau
    {
        get
        {
            return this.tableau;
        }
    }

    /// <summary>
    /// Get the value of the cost function
    /// </summary>
    public double Targetfvalue
    {
        get
        {
            return this.targetfvalue;
        }
    }
    /// <summary>
    /// Get the vector x := (x1,x2,...,xn)^T with the optimum values
    /// </summary>
    public double[] Solutionvector
    {
        get
        {
            return this.solutionvector;
        }
    }
    /// <summary>
    /// Get the description of the solution optimal, infinite, not optimal, no solution
    /// </summary>
    public SOL Solution
    {
        get
        {
            return this.solution;
        }
    }
    /// <summary>
    /// Get all the calculated tableaus during the optimization process
    /// </summary>
    public List<double[,]> Alltableaus
    {
        get
        {
            return this.alltableaus;
        }
    }

    #endregion //Properties


    #region Exceptionclasses
    private class NoSolutionException : System.ApplicationException
    {
        public NoSolutionException() { }
        public NoSolutionException(string message) { }
        public NoSolutionException(string message, System.Exception inner) { }

        // Constructor needed for serialization 
        // when exception propagates from a remoting server to the client.
        protected NoSolutionException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }

    private class DegenerateCornerException : System.ApplicationException
    {
        public DegenerateCornerException() { }
        public DegenerateCornerException(string message) { }
        public DegenerateCornerException(string message, System.Exception inner) { }

        // Constructor needed for serialization 
        // when exception propagates from a remoting server to the client.
        protected DegenerateCornerException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }

    #endregion //Exceptionclasses
}



