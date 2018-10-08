using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class AssayController : MonoBehaviour
{

    public FileChooser fileChooser;
    private string fileContentString;
    public alglib alg;
    public Export export;
    public List<string> samples = new List<string>();
    public List<Toggle> sampleToggles = new List<Toggle>();
    public static double[] combiSampleValues = { 8.67, 0.0021, 0.068, 1.22, 0.0149, 2.66, 2.92, 0.64, 0.0477, 0.0008, 3.97, 0.073, 0.01, 0.3, 0.00051, 79.40299, 15, 52, 23, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 5, 0, 0, 4, 0, 0, 0, 100 };

    public GameObject mainMenu;
    public GameObject assayMenu;
    public CSVReader csvController;
    public GameObject samplePrefab;
    public GameObject sampleScrollView; //content

    public double[] elementDoubleList;
    public double[] chemicalDoubleList;
    public static double[] combinedDoubleList;

    public static double exportFunc = -1;
    public static string exportResult = "N/A";

    public LPPNamespace.LPP lpp;

    // Use this for initialization
    void Start()
    {
        //Debug.Log("The calculation here is wrong. Need to compare it with the original version to check what the problem is. Seems to lie in the sumproduct of x and [THING], but there's also an issue where the final value (100) isn't being accounted for.");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitialiseAssay()
    {
        mainMenu.SetActive(false);
        assayMenu.SetActive(true);
        for (int i = 0; i < csvController.samplePositions.Count; i++)
        {
            string sampleID = csvController.sampleList[i];
            int samplePos = csvController.samplePositions[i];
            samples.Add(sampleID);

            GameObject g = GameObject.Instantiate(samplePrefab) as GameObject;
            g.transform.SetParent(sampleScrollView.transform, false);
            g.GetComponentInChildren<Text>().text = sampleID;
            g.GetComponent<SampleListEntry>().SampleID = sampleID;
            g.GetComponent<SampleListEntry>().index = samplePos;

            sampleToggles.Add(g.GetComponent<Toggle>());
        }
    }

    public void CalculateAssay()
    {
        string columnNames = "Sample No.,,Other(Quartz),Feld_Alb_Ca-Na,Feld_Alb_Na,FeldsparK,FeldsKBa,Feld_Plag,Carb_Calc,Carb_Ank,Carb_Dol,Anhydrite,ChloriteFe,ChloriteMg,Muscovite,Musc_Phengite,BiotMg,BiotFe,Amph_Act,Amph_HorMg,Amph_Trem,Epid_LC,Epid_Clzt,Magnetite,Apatite,Chalcopyrite,Pyrite,Uraninite,Arsenopyrite,Molybdenite,Chalcocite,Sphene,Rutile,Barite,Kaolinite,Smectite/Montmorillonite,Jarosite,Tour_Fe,Tour_Mg,As,Ba,Ca,Cu,K,Mg,Na,Chrysocolla,Mn";

        export.WriteStringToStringBuilder(columnNames);

        foreach (Toggle tog in sampleToggles)
        {
            if (tog.isOn)
            {
                //calculate for sampleID
                //this involves combiSampleValues 
                int s = tog.gameObject.GetComponent<SampleListEntry>().index;
                Debug.Log(tog.gameObject.GetComponent<SampleListEntry>().SampleID);

                csvController.GetChemTestList(csvController.grid, s);
                try
                {
                    lpp.StartLPP(csvController.chemTest);
                    export.WriteStringToStringBuilder(tog.gameObject.GetComponent<SampleListEntry>().SampleID + " successfully calculated");
                }
                catch (Exception e)
                {
                    export.WriteStringToStringBuilder(tog.gameObject.GetComponent<SampleListEntry>().SampleID + " - error occured, could not calculate");
                    Debug.Log(e);

                }
                

                string id = csvController.grid[0, s];

                //int degreesOfFreedom = combinedDoubleList.Length - x.Length;

                //double chiSquare = ChiSquarePval(Math.Sqrt(exportFunc), degreesOfFreedom);

                //string truncatedExportResult = exportResult.Substring(1, exportResult.Length - 2);

                //export.WriteStringToStringBuilder(id + "," + truncatedExportResult + "," + exportFunc + "," + degreesOfFreedom + "," + chiSquare);
            }


        }
    }   //CalculateCombi()

    public void SetSampleValuesList(double[] sampleArray)
    {
        combiSampleValues = sampleArray;
    }
    
    public static double SumProduct(double[] arrayA, double[] arrayB)
    {
        string firstArray = "";
        foreach (double d in arrayA)
        {
            firstArray += d.ToString() + ", ";
        }
        //Debug.Log("arrayA: " + firstArray);

        string secondArray = "";
        foreach (double d in arrayB)
        {
            secondArray += d.ToString() + ", ";
        }
        //Debug.Log("arrayB: " + secondArray);

        double result = 0;
        for (int i = 0; i < arrayA.Length; i++)
            result += arrayA[i] * arrayB[i];
        return result;
    }   //SumProduct()
    

    public static double ChiSquarePval(double x, int df)
    {
        // x = a computed chi-square value.
        // df = degrees of freedom.
        // output = prob. x value occurred by chance.
        // ACM 299.
        if (x <= 0.0 || df < 1)
            throw new Exception("Bad arg in ChiSquarePval()");
        double a = 0.0; // 299 variable names
        double y = 0.0;
        double s = 0.0;
        double z = 0.0;
        double ee = 0.0; // change from e
        double c;
        bool even; // Is df even?
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
                else ee = 0.5723649429247000870717135;
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
                else
                    ee = 0.5641895835477562869480795 / Math.Sqrt(a);
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
    private static double Exp(double x)
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
                p = ((((((((0.000124818987 * w
                  - 0.001075204047) * w + 0.005198775019) * w
                  - 0.019198292004) * w + 0.059054035642) * w
                  - 0.151968751364) * w + 0.319152932694) * w
                  - 0.531923007300) * w + 0.797884560593) * y
                  * 2.0;
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
}
