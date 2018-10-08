using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Simplex
{
    public class Simplex : MonoBehaviour
    {
        private double[] c;
        private double[,] A;
        private double[] b;
        private HashSet<int> N = new HashSet<int>();
        private HashSet<int> B = new HashSet<int>();
        private double v = 0;


        public void DontStart()
        {
           var s = new Simplex(
              new[] { -2.0, -3, -4, 0, 0 },
              new[,] {
          {3.0, 2, 1, 1, 0},
          {2, 5, 3, 0, 1}
              },
              new double[] {10, 15 }
            );
            /*
            var s = new Simplex(
               new[] { 7, 5, 5, 7, 7, 3, 0.3, 5.4, 3, 0.7, 10, 10, 7.1, 5, 9, 9.5, 8, 7.5, 6, 4, 3, 7, 10, 8, 5, 10, 10, 10, 6, 2, -1, -1, 7.2, 7.2, 5.0, 7.5, 6, -100, -100, -100, -100, -100, -100, -100, 0, -100 },
               new[,] {
          { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
          { 0, 10.41, 10.4, 9.817163594, 9.85, 15.3732, 0, 0, 0, 0, 9.6754, 10.0939, 19.5, 16.46, 7.5391, 7.3702, 1.5633, 3.42, 0.9295, 12.5294, 17.8137858, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 20.75, 9.44, 0, 13.07, 18.03, 0, 0, 0, 0, 0, 0, 0, 2.05, 0 },
          { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 46, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0 },
          { 0, 0, 0, 0, 0.25, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 58.84, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0 },
          { 0, 0.38, 0, 0.12149786, 0, 7.8908, 40.04426534, 20.0185592, 21.73, 29.44, 0.1066, 0.0377, 0, 0, 0.0156, 0.1426, 8.7168, 8.63, 9.3243, 16.5257, 17.63863053, 0, 39.05798859, 0, 0, 0, 0, 0, 0, 20.44022825, 0, 0, 0.04, 0.49, 0, 1.43, 0.45, 0, 0, 100, 0, 0, 0, 0, 0, 0 },
          { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 34.5, 0, 0, 0, 0, 79.85, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 33.86, 0 },
          { 0, 0.05, 0.04, 0.0979097, 0.08, 0.199, 0, 12.55342226, 0, 0, 20.3305, 10.5241, 0.08, 3.19, 7.9141, 13.0428, 12.7119, 11.26, 6.3783, 9.494, 0, 72.34827478, 0, 30.49887156, 46.59103012, 0, 34.3, 0, 0, 0, 0, 0, 0.16, 2.23, 33.45, 9.21, 2.38, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
          { 0, 0.11, 0.24, 13.33707643, 13.5, 0.1423, 0, 0, 0, 0, 0.0161, 0.0335, 8.37, 9.14, 7.8631, 7.3473, 0.1038, 0.43, 0.0668, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.17, 0.37, 7.81, 0.04, 0.01, 0, 0, 0, 0, 100, 0, 0, 0, 0 },
          { 0, 0.01, 0.02, 0, 0, -0.0012, 0, 6.071985112, 13.18, 0, 9.3554, 14.3654, 0.05, 0.81, 11.8337, 8.2094, 8.065, 8.03, 11.4574, 0.0194, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.04, 1.87, 0, 5.23, 5.69, 0, 0, 0, 0, 0, 100, 0, 0, 0 },
          { 0, 0, 0, 0, 0.03, -0.0065, 0, 0, 0, 0, 0.0787, 0.0387, 0.093, 0, 0.052, 0.0686, 0.0905, 0.48, 0.1231, 0.1798, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.023, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100 },
          { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 59.9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
          { 0, 7.8, 8.21, 0.126114876, 0.31, 3.8507, 0, 0, 0, 0, 0.0455, 0.0293, 0.47, 0.08, 0.0718, 0.0769, 0.204, 0.78, 0.2079, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.07, 1.61, 0, 1.26, 1.46, 0, 0, 0, 0, 0, 0, 100, 0, 0 },
          { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 18.17529238, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
          { 0, 0, 0, 0, 0, 0, 0, 0, 0, 23.55, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 35, 53.4, 0, 19.7, 40.1, 20.15, 0, 0, 13.74, 0, 0, 12.81, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
          { 0, 0, 0, 0, 0.03, 0.002, 0, 0, 0, 0, 0.0163, 0.0267, 0, 0.1, 1.0092, 0.9201, 0.0574, 0.53, 0.0767, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 24.46569855, 59.96494742, 0, 0.05, 0.04, 0, 1.51, 0.24, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
          { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 88.15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
          { 100.00, 81.24, 81.09, 76.50, 75.95, 72.55, 59.96, 61.36, 65.09, 47.01, 60.38, 64.85, 71.44, 70.22, 63.70, 62.82, 68.49, 66.44, 71.44, 61.25, 64.55, 27.65, 42.77, 0.00, 0.01, 11.85, 0.00, 0.00, 0.00, 55.09, 40.04, 27.42, 78.72, 83.93, 45.93, 68.25, 71.74, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 64.09, 0.00 },
        },
               new double[] { 100, 867, 0.21, 6.8, 122, 1.49, 266, 292, 64, 4.77, 0.08, 397, 7.3, 1, 30, 0.051, 7940.299 }
             );
            */
            var answer = s.maximize();
            UnityEngine.Debug.Log("ANSWER: " + answer.Item1);
            for(int i = 0; i < answer.Item2.Length; i++)
            {
                UnityEngine.Debug.Log(i + ": " + answer.Item2[i] + ", ");

            }
        }


        public Simplex(double[] c, double[,] A, double[] b)
        {
            int vars = c.Length, constraints = b.Length;

            if (vars != A.GetLength(1))
            {
                throw new Exception("Number of variables in c doesn't match number in A.");
            }

            if (constraints != A.GetLength(0))
            {
                throw new Exception("Number of constraints in A doesn't match number in b.");
            }

            // Extend max fn coefficients vector with 0 padding
            this.c = new double[vars + constraints];
            Array.Copy(c, this.c, vars);

            // Extend coefficient matrix with 0 padding
            this.A = new double[vars + constraints, vars + constraints];
            for (int i = 0; i < constraints; i++)
            {
                for (int j = 0; j < vars; j++)
                {
                    this.A[i + vars, j] = A[i, j];
                }
            }

            // Extend constraint right-hand side vector with 0 padding
            this.b = new double[vars + constraints];
            Array.Copy(b, 0, this.b, vars, constraints);

            // Populate non-basic and basic sets
            for (int i = 0; i < vars; i++)
            {
                N.Add(i);
            }

            for (int i = 0; i < constraints; i++)
            {
                B.Add(vars + i);
            }
        }

        public Tuple<double, double[]> maximize()
        {
            while (true)
            {
                // Find highest coefficient for entering var
                int e = -1;
                double ce = 0;
                foreach (var _e in N)
                {
                    if (c[_e] > ce)
                    {
                        ce = c[_e];
                        e = _e;
                    }
                }

                // If no coefficient > 0, there's no more maximizing to do, and we're almost done
                if (e == -1) break;

                // Find lowest check ratio
                double minRatio = double.PositiveInfinity;
                int l = -1;
                foreach (var i in B)
                {
                    if (A[i, e] > 0)
                    {
                        double r = b[i] / A[i, e];
                        if (r < minRatio)
                        {
                            minRatio = r;
                            l = i;
                        }
                    }
                }

                // Unbounded
                if (double.IsInfinity(minRatio))
                {
                    return Tuple.Create<double, double[]>(double.PositiveInfinity, null);
                }

                pivot(e, l);
            }

            // Extract amounts and slack for optimal solution
            double[] x = new double[b.Length];
            int n = b.Length;
            for (var i = 0; i < n; i++)
            {
                x[i] = B.Contains(i) ? b[i] : 0;
            }

            // Return max and variables
            return Tuple.Create<double, double[]>(v, x);
        }

        private void pivot(int e, int l)
        {
            N.Remove(e);
            B.Remove(l);

            b[e] = b[l] / A[l, e];

            foreach (var j in N)
            {
                A[e, j] = A[l, j] / A[l, e];
            }

            A[e, l] = 1 / A[l, e];

            foreach (var i in B)
            {
                b[i] = b[i] - A[i, e] * b[e];

                foreach (var j in N)
                {
                    A[i, j] = A[i, j] - A[i, e] * A[e, j];
                }

                A[i, l] = -1 * A[i, e] * A[e, l];
            }

            v = v + c[e] * b[e];

            foreach (var j in N)
            {
                c[j] = c[j] - c[e] * A[e, j];
            }

            c[l] = -1 * c[e] * A[e, l];

            N.Add(l);
            B.Add(e);
        }
    }

    public class Tuple<T, U>
    {
        public T Item1 { get; private set; }
        public U Item2 { get; private set; }

        public Tuple(T item1, U item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }

    public static class Tuple
    {
        public static Tuple<T, U> Create<T, U>(T item1, U item2)
        {
            return new Tuple<T, U>(item1, item2);
        }
    }
}