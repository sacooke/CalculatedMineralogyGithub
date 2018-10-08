using System;
using UnityEngine;
using UnityEngine.UI;
  public class ChiSquaredProgram : MonoBehaviour
    {
    void Start()//static void Main(string[] args)
    {
        if (false)
        {
            Debug.Log("\nBegin chi-squared goodness-of-fit demo \n");

            // 1. calculate chi-squared stat
            Debug.Log("Goal is to determine if roulette wheel is fair \n");

            Debug.Log("Wheel was spun 380 times \n");

            int[] observed = new int[] { 192, 163, 25 };  // sum = 380  (180, 180, 20)
            Debug.Log("Observed counts of red, black, green: ");
            ShowVector(observed);

            double[] probs = new double[] { 18.0 / 38, 18.0 / 38, 2.0 / 38 };
            Debug.Log("Probabilities if fair:");
            ShowVector(probs, 4);

            double[] expected = ExpectedFromProbs(probs, 380);
            Debug.Log("Expected counts if fair:");
            ShowVector(expected, 1);

            double chi = ChiFromProbs(observed, probs);  // 3.66
            Debug.Log("Calculated chi-squared = " + chi.ToString("F2"));

            // 2. calculate p-value
            int df = observed.Length - 1;
            double pval = ChiSquarePval(chi, 2);
            Debug.Log("The pval with df of " + df + " = " + pval.ToString("F4"));

            Debug.Log("\nThe pval is approximate probability that, if wheel is fair,");
            Debug.Log("you'd see a chi-squared value as extreme as calculated");

            Debug.Log("\nEnd demo");
        }

    } // Main

    public static void ShowVector(int[] v)
    {
      for (int i = 0; i < v.Length; ++i)
        Debug.Log(v[i] + "  ");
      Debug.Log("\n");
    }

    public static void ShowVector(double[] v, int dec)
    {
      for (int i = 0; i < v.Length; ++i)
        Debug.Log(v[i].ToString("F" + dec) + "  ");
      Debug.Log("\n");
    }

    public static double ChiFromFreqs(int[] observed, double[] expected)
    {
      double sum = 0.0;
      for (int i = 0; i < observed.Length; ++i) {
        sum += ((observed[i] - expected[i]) *
          (observed[i] - expected[i])) / expected[i];
       }
      return sum;
    }

    public static double ChiFromProbs(int[] observed, double[] probs)
    {
      int n = observed.Length;
      int sumObs = 0;
      for (int i = 0; i < n; ++i)
        sumObs += observed[i];

      double[] expected = ExpectedFromProbs(probs, sumObs);
      return ChiFromFreqs(observed, expected);
    }

    public static double[] ExpectedFromProbs(double[] probs, int N)
    {
      double[] expected = new double[probs.Length];
      for (int i = 0; i < probs.Length; ++i)
        expected[i] = probs[i] * N;
      return expected;
    }

    public static double ChiSquarePval(double x, int df)
    {
      // x = a computed chi-square value. df = degrees of freedom.
      // output = prob. the x value occurred by chance.
      // so, for example, if result < 0.05 there is only a 5% chance
      // that the x value ocurred by chance and therefore
      // we conclude that the actual data which produced x is
      // NOT the same as the expected data.
      // this function can be used to create a ChiSquareTest proedure.
      // ACM Algorithm 299 and update ACM TOMS June 1985.
      // Uses custom Exp() function below.

      if (x <= 0.0 || df < 1)
        throw new Exception("Bad arg in ChiSquarePval()");

      double a = 0.0; // 299 variable names
      double y = 0.0;
      double s = 0.0;
      double z = 0.0;
      double ee = 0.0; // change from e
      double c;

      bool even; // is df even?

      a = 0.5 * x;
      if (df % 2 == 0) even = true; else even = false;

      if (df > 1) y = Exp(-a); // ACM update remark (4)

      if (even == true) s = y;
      else s = 2.0 * Gauss(-Math.Sqrt(x));

      if (df > 2)
      {
        x = 0.5 * (df - 1.0);
        if (even == true) z = 1.0; else z = 0.5;
        if (a > 40.0) // ACM remark (5)
        {
          if (even == true) ee = 0.0;
          else ee = 0.5723649429247000870717135; // log(sqrt(pi))
          c = Math.Log(a); // log base e
          while (z <= x)
          {
            ee = Math.Log(z) + ee;
            s = s + Exp(c * z - a - ee); // ACM update remark (6)
            z = z + 1.0;
          }
          return s;
        } // a > 40.0
        else
        {
          if (even == true) ee = 1.0;
          else ee = 0.5641895835477562869480795 / Math.Sqrt(a); // (1 / sqrt(pi))
          c = 0.0;
          while (z <= x)
          {
            ee = ee * (a / z); // ACM update remark (7)
            c = c + ee;
            z = z + 1.0;
          }
          return c * y + s;
        }
      } // df > 2
      else
      {
        return s;
      }
    } // ChiSquarePval()

    private static double Exp(double x) // ACM update remark (3)
    {
      if (x < -40.0) // ACM update remark (8)
        return 0.0;
      else
        return Math.Exp(x);
    }

    public static double Gauss(double z)
    {
      // input = z-value (-inf to +inf)
      // output = p under Normal curve from -inf to z
      // e.g., if z = 0.0, function returns 0.5000
      // ACM Algorithm #209
      double y; // 209 scratch variable
      double p; // result. called ‘z’ in 209
      double w; // 209 scratch variable

      if (z == 0.0)
        p = 0.0;
      else
      {
        y = Math.Abs(z) / 2;
        if (y >= 3.0)
        {
          p = 1.0;
        }
        else if (y < 1.0)
        {
          w = y * y;
          p = ((((((((0.000124818987 * w - 0.001075204047) * w + 0.005198775019) * w
            - 0.019198292004) * w + 0.059054035642) * w
            - 0.151968751364) * w + 0.319152932694) * w
            - 0.531923007300) * w + 0.797884560593) * y * 2.0;
        }
        else
        {
          y = y - 2.0;
          p = (((((((((((((-0.000045255659 * y
            + 0.000152529290) * y - 0.000019538132) * y
            - 0.000676904986) * y + 0.001390604284) * y
            - 0.000794620820) * y - 0.002034254874) * y
           + 0.006549791214) * y - 0.010557625006) * y
          + 0.011630447319) * y - 0.009279453341) * y
         + 0.005353579108) * y - 0.002141268741) * y
        + 0.000535310849) * y + 0.999936657524;
        }
      }

      if (z > 0.0)
        return (p + 1.0) / 2;
      else
        return (1.0 - p) / 2;
    } // Gauss()
    
  } // Program