using System;
using UnityEngine;

public class RevisedSimplex : MonoBehaviour
{
    public static void revisedSimplex(bool maximize, int n, int m, double[,] a, double epsilon, int[] basicvar)
    {
        int i, j, k, m2, p, idx = 0;
        double[] objcoeff = new double[n + 1];
        double[] varsum = new double[n + 1];
        double[] optbasicval = new double[m + 3];
        double[] aux = new double[m + 3];
        double[,] work = new double[m + 3, m + 3];
        double part, sum;
        bool infeasible, unbound, abort, outres, iterate;
        if (maximize)
            for (j = 1; j <= n; j++)
            {
                Debug.Log("j = " + j);
                Debug.Log("a[0, " + j + "] = " + a[0, j]);
                a[0, j] = -a[0, j];
            }
        infeasible = false;
        unbound = false;
        m2 = m + 2;
        p = m + 2;
        outres = true;
        k = m + 1;
        for (j = 1; j <= n; j++)
        {
            objcoeff[j] = a[0, j];
            sum = 0.0;
            for (i = 1; i <= m; i++)
                sum -= a[i, j];
            varsum[j] = sum;
        }
        sum = 0.0;
        for (i = 1; i <= m; i++)
        {
            basicvar[i] = n + i;
            optbasicval[i] = a[i, 0];
            sum -= a[i, 0];
        }
        optbasicval[k] = 0.0;
        optbasicval[m2] = sum;
        for (i = 1; i <= m2; i++)
        {
            for (j = 1; j <= m2; j++)
                work[i, j] = 0.0;
            work[i, i] = 1.0;
        }
        iterate = true;
        do
        {
            if ((optbasicval[m2] >= -epsilon) && outres)
            {
                outres = false;
                p = m + 1;
            }
            part = 0.0;
            for (j = 1; j <= n; j++)
            {
                sum = work[p, m + 1] * objcoeff[j] + work[p, m + 2] * varsum[j];
                for (i = 1; i <= m; i++)
                    sum += work[p, i] * a[i, j];
                if (part > sum)
                {
                    part = sum;
                    k = j;
                }
            }
            if (part > -epsilon)
            {
                iterate = false;
                if (outres)
                    infeasible = true;
                else
                    a[0, 0] = -optbasicval[p];
            }
            else
            {
                for (i = 1; i <= p; i++)
                {
                    sum = work[i, m + 1] * objcoeff[k] + work[i, m + 2] * varsum[k];
                    for (j = 1; j <= m; j++)
                        sum += work[i, j] * a[j, k];
                    aux[i] = sum;
                }
                abort = true;
                for (i = 1; i <= m; i++)
                    if (aux[i] >= epsilon)
                    {
                        sum = optbasicval[i] / aux[i];
                        if (abort || (sum < part))
                        {
                            part = sum;
                            idx = i;
                        }
                        abort = false;
                    }
                if (abort)
                {
                    unbound = true;
                    iterate = false;
                }
                else
                {
                    basicvar[idx] = k;
                    sum = 1.0 / aux[idx];
                    for (j = 1; j <= m; j++)
                        work[idx, j] *= sum;
                    i = ((idx == 1) ? 2 : 1);
                    do
                    {
                        sum = aux[i];
                        optbasicval[i] -= part * sum;
                        for (j = 1; j <= m; j++)
                            work[i, j] -= work[idx, j] * sum;
                        i += ((i == idx - 1) ? 2 : 1);
                    } while (i <= p);
                    optbasicval[idx] = part;
                }
            }
        } while (iterate);
        // return results
        basicvar[m + 1] = (infeasible ? 1 : 0);
        basicvar[m + 2] = (unbound ? 1 : 0);
        for (i = 1; i <= m; i++)
            a[i, 0] = optbasicval[i];
        if (maximize)
        {
            for (j = 1; j <= n; j++)
                a[0, j] = -a[0, j];
            a[0, 0] = -a[0, 0];
        }
    }


    public void DontStart()
    {
        int n = 3; //N independent variables
        int m = 2; //M = m1 + m2 + m3 additional constraints
        double eps = 1.0e-5;
        int[] basicvar = new int[m + 3];

        /*double[,] a = { { 0, .42, .35, 0, 0, 0},
{100, 6, 0, 1, 0, 0},
{ 60, 2, 7, 0, 1, 0},
{ 90, 2, 3, 0, 0, 1}};*/

        double[,] a = {
            {1, 2, 3, 4, 0, 0, 0 }, //originally c
          {0,3,2,1,1,0,10},
          {0,2,5,3,0,1,15}
              };


        /*int vars = c.Length, constraints = b.Length;

        if (vars != A.GetLength(1))
        {
            throw new Exception("Number of variables in c doesn't match number in A.");
        }

        if (constraints != A.GetLength(0))
        {
            throw new Exception("Number of constraints in A doesn't match number in b.");
        }*/

        /*
              new[] { 10.2, 422.3, 6.91, 853 },
              new[,] {
          {0.1, 0.5, 0.333333, 1},
          {30, 15, 19, 12},
          {1000, 6000, 4100, 9100},
          {50, 20, 21, 10},
          {4, 6, 19, 30}
              },
              new double[] { 2000, 1000, 1000000, 640, 432 }*/

        revisedSimplex(true, n, m, a, eps, basicvar);
        Debug.Log("Search optimal mix for maximum profit\n");

        if (basicvar[m + 1] > 0)
            Debug.Log("No solution found.");
        else
        {
            if (basicvar[m + 2] > 0)
                Debug.Log("No solution found.");
            else
            {
                Debug.Log("Optimal solution found.\nValues ​​of CHEWY and NUTTY and slack variables");
                for (int i = 1; i <= m; i++)
                {
                    Debug.Log("basicvar[" + i + "] = " + basicvar[i] + ", a[" + i + ", 0] = " + a[i, 0]);
                }
                Debug.Log("Optimal value of the function = " + a[0, 0]);
            }
        }
    }
}
 