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

    public TestSimplexOct testSimplexOct;

    // Use this for initialization
    void Start()
    {
        alglib.testminlpunit.testminlp(false, null);
        //Debug.Log("The calculation here is wrong. Need to compare it with the original version to check what the problem is. Seems to lie in the sumproduct of x and [THING], but there's also an issue where the final value (100) isn't being accounted for.");

        double[] x = new double[] { };

        double[,] simplexArray = new double[,] {
                                                    //{ 7,5,5,7,7,3,0.3,5.4,3,0.7,10,10,7.1,5,9,9.5,8,7.5,6,4,3,7,10,8,5,10,10,10,6,2,-1,-1,7.2,7.2,5.0,7.5,6,-100,-100,-100,-100,-100,-100,-100,0,-100},
                                                    { 1,1 },
                                                    {2,1 }
                                               };
        double[] cn = csvController.chemTest.ToArray();

        double[] bndl = new double[] { 0, 0};//{ -1, +1, -1 }; //lower = 0
        double[] bndu = new double[] { double.PositiveInfinity, double.PositiveInfinity};//{ +1, +3, +1 }; //upper = 100

        double[] al = new double[] { double.NegativeInfinity, double.NegativeInfinity };//{ -1, +1, -1 }; //lower = 0
        double[] au = new double[] { 4, 5 };//{ +1, +3, +1 }; //upper = 100

        double[] c = {3,4};

        alglib.minlpstate state;
        alglib.minlpreport rep;
        alglib.minlpcreate(bndl.Length, out state);
        alglib.minlpsetcost(state, c);
        alglib.minlpsetbc(state, bndl, bndu);
        //alglib.minlpsetcost(state, bndl); //replace bndl with something
        //alglib.minlpsetbc(state, bndl, bndu);
        double[] ct = { };
        //alglib.minlpsetlc1(state, simplexArray, ct); //a, ct is constraint types so >0 is >= =0 is = and <0 is <=, k
        /*
CT      -   constraint types, array[K]:
            * if CT[i]>0, then I-th constraint is A[i,*]*x >= A[i,n]
            * if CT[i]=0, then I-th constraint is A[i,*]*x  = A[i,n]
            * if CT[i]<0, then I-th constraint is A[i,*]*x <= A[i,n]*/
        alglib.minlpsetlc2(state, simplexArray, al, au);
        alglib.minlpoptimize(state);
        alglib.minlpresults(state, out x, out rep);

        Debug.Log("type = " + rep.terminationtype);



        Debug.Log("Values: " + alglib.ap.format(x, 2));// EXPECTED: [-3,+3]
        string exportResult = alglib.ap.format(x, 2);

        string truncatedExportResult = exportResult.Substring(1, exportResult.Length - 2);

        double[] resultArray = Array.ConvertAll(truncatedExportResult.Split(','), new Converter<string, double>(Double.Parse));

        double result = SumProduct(c, resultArray);
        Debug.Log("Result: " + result);
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

        int f = 1;

        foreach (Toggle tog in sampleToggles)
        {
            if (tog.isOn)
            {
                //calculate for sampleID
                //this involves combiSampleValues 
                int s = tog.gameObject.GetComponent<SampleListEntry>().index;
                //Debug.Log(tog.gameObject.GetComponent<SampleListEntry>().SampleID);

                csvController.GetChemTestList(csvController.grid, s);
                /*try
                {
                    lpp.StartLPP(csvController.chemTest);
                    export.WriteStringToStringBuilder(tog.gameObject.GetComponent<SampleListEntry>().SampleID + " successfully calculated");
                }
                catch (Exception e)
                {
                    export.WriteStringToStringBuilder(tog.gameObject.GetComponent<SampleListEntry>().SampleID + " - error occured, could not calculate");
                    Debug.Log(e);

                }*/
                //Debug.Log("chemtest: " + csvController.chemTest[0]);

                //testSimplexOct.CalcSimplexForList(csvController.chemTest, f, tog.gameObject.GetComponent<SampleListEntry>().SampleID);
                //f++;

                string id = csvController.grid[0, s];

                //int degreesOfFreedom = combinedDoubleList.Length - x.Length;

                //double chiSquare = ChiSquarePval(Math.Sqrt(exportFunc), degreesOfFreedom);

                //string truncatedExportResult = exportResult.Substring(1, exportResult.Length - 2);

                //export.WriteStringToStringBuilder(id + "," + truncatedExportResult + "," + exportFunc + "," + degreesOfFreedom + "," + chiSquare);


                /*alglib.minlmcreatev(combinedDoubleList.Length, x, 0.0001, out state);
                alglib.minlmsetbc(state, bndl, bndu);
                alglib.minlmsetcond(state, epsx, maxits);
                alglib.minlmsetxrep(state, true);
                alglib.minlmoptimize(state, function1_fvec, function1_rep, null);
                alglib.minlmresults(state, out x, out rep);*/

                //double[] x = new double[] { 15, 52, 23, 0, 0, 0, 0, 0, 0.5, 0, 0, 0, 0, 0, 1.0, 0, 0, 0, 0, 0, 0, 0, 0, 1.2, 0, 0.1, 1, 5, 0.5, 0, 0, 4, 0, 0, 0, 0 };
                //double epsx = 0.0000000001;
                //int maxits = 0;

                double[,] a = new double[,] { { 1,1,1},
                                                {1,1,1 },
                                                {1,1,1 }
                                            };

                double[] x = new double[] { };

                double[,] simplexArray = new double[,] {
                                                    //{ 7,5,5,7,7,3,0.3,5.4,3,0.7,10,10,7.1,5,9,9.5,8,7.5,6,4,3,7,10,8,5,10,10,10,6,2,-1,-1,7.2,7.2,5.0,7.5,6,-100,-100,-100,-100,-100,-100,-100,0,-100},
                                                    { 0,10.41,10.4,9.817163594,9.85,15.3732,0,0,0,0,9.6754,10.0939,19.5,16.46,7.5391,7.3702,1.5633,3.42,0.9295,12.5294,17.8137858,0,0,0,0,0,0,0,0,0,0,0,20.75,9.44,0,13.07,18.03,0,0,0,0,0,0,0,2.05,0},
                                                    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,46,0,0,0,0,0,0,0,0,0,0,100,0,0,0,0,0,0,0,0 },
                                                    { 0,0,0,0,0.25,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,58.84,0,0,0,0,0,0,100,0,0,0,0,0,0,0},
                                                    { 0,0.38,0,0.12149786,0,7.8908,40.04426534,20.0185592,21.73,29.44,0.1066,0.0377,0,0,0.0156,0.1426,8.7168,8.63,9.3243,16.5257,17.63863053,0,39.05798859,0,0,0,0,0,0,20.44022825,0,0,0.04,0.49,0,1.43,0.45,0,0,100,0,0,0,0,0,0},
                                                    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,34.5,0,0,0,0,79.85,0,0,0,0,0,0,0,0,0,0,0,100,0,0,0,33.86,0},
                                                    { 0,0.05,0.04,0.0979097,0.08,0.199,0,12.55342226,0,0,20.3305,10.5241,0.08,3.19,7.9141,13.0428,12.7119,11.26,6.3783,9.494,0,72.34827478,0,30.49887156,46.59103012,0,34.3,0,0,0,0,0,0.16,2.23,33.45,9.21,2.38,0,0,0,0,0,0,0,0,0},
                                                    { 0,0.11,0.24,13.33707643,13.5,0.1423,0,0,0,0,0.0161,0.0335,8.37,9.14,7.8631,7.3473,0.1038,0.43,0.0668,0,0,0,0,0,0,0,0,0,0,0,0,0,0.17,0.37,7.81,0.04,0.01,0,0,0,0,100,0,0,0,0},
                                                    { 0,0.01,0.02,0,0,-0.0012,0,6.071985112,13.18,0,9.3554,14.3654,0.05,0.81,11.8337,8.2094,8.065,8.03,11.4574,0.0194,0,0,0,0,0,0,0,0,0,0,0,0,0.04,1.87,0,5.23,5.69,0,0,0,0,0,100,0,0,0},
                                                    { 0,0,0,0,0.03,-0.0065,0,0,0,0,0.0787,0.0387,0.093,0,0.052,0.0686,0.0905,0.48,0.1231,0.1798,0,0,0,0,0,0,0,0,0,0,0,0,0,0.023,0,0,0,0,0,0,0,0,0,0,0,100},
                                                    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,59.9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                                                    { 0,7.8,8.21,0.126114876,0.31,3.8507,0,0,0,0,0.0455,0.0293,0.47,0.08,0.0718,0.0769,0.204,0.78,0.2079,0,0,0,0,0,0,0,0,0,0,0,0,0,0.07,1.61,0,1.26,1.46,0,0,0,0,0,0,100,0,0},
                                                    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,18.17529238,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                                                    { 0,0,0,0,0,0,0,0,0,23.55,0,0,0,0,0,0,0,0,0,0,0,0,0,35,53.4,0,19.7,40.1,20.15,0,0,13.74,0,0,12.81,0,0,0,0,0,0,0,0,0,0,0},
                                                    { 0,0,0,0,0.03,0.002,0,0,0,0,0.0163,0.0267,0,0.1,1.0092,0.9201,0.0574,0.53,0.0767,0,0,0,0,0,0,0,0,0,0,24.46569855,59.96494742,0,0.05,0.04,0,1.51,0.24,0,0,0,0,0,0,0,0,0},
                                                    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,88.15,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                                                    { 100.00,81.24,81.09,76.50,75.95,72.55,59.96,61.36,65.09,47.01,60.38,64.85,71.44,70.22,63.70,62.82,68.49,66.44,71.44,61.25,64.55,27.65,42.77,0.00,0.01,11.85,0.00,0.00,0.00,55.09,40.04,27.42,78.72,83.93,45.93,68.25,71.74,0.00,0.00,0.00,0.00,0.00,0.00,0.00,64.09,0.00}
                                                };
                double[] cn = csvController.chemTest.ToArray();

                double[] bndl = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//{ -1, +1, -1 }; //lower = 0
                double[] bndu = new double[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };//{ +1, +3, +1 }; //upper = 100

                double[] al = new double[] { cn[0], cn[1], cn[2], cn[3], cn[4], cn[5], cn[6], cn[7], cn[8], cn[9], cn[10], cn[11], cn[12], cn[13], cn[14], cn[15] };//{ -1, +1, -1 }; //lower = 0
                double[] au = new double[] { cn[0], cn[1], cn[2], cn[3], cn[4], cn[5], cn[6], cn[7], cn[8], cn[9], cn[10], cn[11], cn[12], cn[13], cn[14], cn[15] };//{ +1, +3, +1 }; //upper = 100

                double[] c = { 7, 5, 5, 7, 7, 3, 0.3, 5.4, 3, 0.7, 10, 10, 7.1, 5, 9, 9.5, 8, 7.5, 6, 4, 3, 7, 10, 8, 5, 10, 10, 10, 6, 2, -1, -1, 7.2, 7.2, 5.0, 7.5, 6, -100, -100, -100, -100, -100, -100, -100, 0, -100 };

                alglib.minlpstate state;
                alglib.minlpreport rep;
                Debug.Log("check 1");
                alglib.minlpcreate(bndl.Length, out state);
                alglib.minlpsetcost(state, c);
                alglib.minlpsetbc(state, bndl, bndu);
                //alglib.minlpsetcost(state, bndl); //replace bndl with something
                //alglib.minlpsetbc(state, bndl, bndu);
                /*alglib.minlpsetlc1(state, true); //a, ct is constraint types so >0 is >= =0 is = and <0 is <=, k*/
                /*
        CT      -   constraint types, array[K]:
                    * if CT[i]>0, then I-th constraint is A[i,*]*x >= A[i,n]
                    * if CT[i]=0, then I-th constraint is A[i,*]*x  = A[i,n]
                    * if CT[i]<0, then I-th constraint is A[i,*]*x <= A[i,n]*/
                Debug.Log("check 2");
                alglib.minlpsetlc2(state, simplexArray, al, au);
                Debug.Log("check 3");
                alglib.minlpoptimize(state);
                Debug.Log("check 4");
                alglib.minlpresults(state, out x, out rep);
                Debug.Log("check 5");

                

                Debug.Log("Values: " + alglib.ap.format(x, 2));// EXPECTED: [-3,+3]
                string exportResult =  alglib.ap.format(x, 2);

                string truncatedExportResult = exportResult.Substring(1, exportResult.Length - 2);
                
                double[] resultArray = Array.ConvertAll(truncatedExportResult.Split(','), new Converter<string, double>(Double.Parse));

                double result = SumProduct(c, resultArray);
                Debug.Log("Result: " + result);
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


    public static void function1_fvec(double[] x, double[] fi, object obj)
    {
        //
        // this callback calculates
        // f0(x0,x1) = 100*(x0+3)^4,
        // f1(x0,x1) = (x1-3)^4
        //

        /*fi[0] = 10 * System.Math.Pow(x[0] + 3, 2);
        fi[1] = System.Math.Pow(x[1] - 3, 2);*/

        //fi[0] = x[0] * 3;
        //fi[1] = System.Math.Pow(x[1] * 4,2);
        //fi[2] = x[2] * 5 + x[0] - x[1];


        double[] relativeError = { 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, -1, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, -1 };
        double[] absoluteError = { 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, -1, 0.50, 0.50, 0.50, 0.50, 0.10, 0.10, 0.10, 0.50, 0.50, 0.50, 0.10, 0.50, 0.10, 0.10, 0.50, 0.50, 0.50, 0.50, 0.10, 0.50, 0.10, 0.50, 0.50, 0.50, 0.10, 0.10, 0.10, 0.10, 0.10, -1 };


        double[][] minerals = {
            new double[] { 0,10.4,9.85,15.3732,0,0,0,9.6754,19.5,7.3702,0,1.5633,5.0515,12.5294,0,0,0,0,0,0,0,0,0,0,0,0,20.75,9.44,16.46,13.07,2.05,8.13,33.3,14.98,0,1.12 },
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,46,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0.25,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,58.84,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,7.8908,40.04426534,20.0185592,29.44,0.1066,0,0.1426,21.73,8.7168,8.269,16.5257,0,39.05798859,0,0,0,0,0,0,0,20.44022825,0,0,0.04,0.49,0,1.43,0,0,0,0,17.25,0.69},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,34.5,0,0,0,0,0,79.85,0,0,0,0,0,0,0,33.86,0,0,0,0,0},
            new double[] { 0,0.04,0.08,0.199,0,12.55342226,0,20.3305,0.08,13.0428,0,12.7119,10.3061,9.494,72.34827478,0,30.49887156,46.59103012,0,34.3,33.45,0,0,0,0,0,0.16,2.23,3.19,9.21,0,5.6,0,0,12.02,11.57},
            new double[] { 0,0.24,13.5,0.1423,0,0,0,0.0161,8.37,7.3473,0,0.1038,0.5901,0,0,0,0,0,0,0,7.81,0,0,0,0,0,0.17,0.37,9.14,0.04,0,0,0,0,0,0.04},
            new double[] { 0,0.02,0,0,0,6.071985112,0,9.3554,0.05,8.2094,13.18,8.065,8.2338,0.0194,0,0,0,0,0,0,0,0,0,0,19.23,0,0.04,1.87,0.81,5.23,0,13.62,0,0,5.23,23.97},
            new double[] { 0,0,0.03,0,0,0,0,0.0787,0.093,0.0686,0,0.0905,0.2274,0.1798,0,0,0,0,0,0,0,0,0,0,0,0,0,0.023,0,0,0,0,0,0,0,0.25},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,59.9,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,8.21,0.31,3.8507,0,0,0,0.0455,0.47,0.0769,0,0.204,0.8272,0,0,0,0,0,0,0,0,0,0,0,0,0,0.07,1.61,0.08,1.26,0,0,0,0,0,0.24},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,18.17529238,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,23.55,0,0,0,0,0,0,0,0,0,35,53.4,0,19.7,12.81,40.1,20.15,0,0,13.74,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0.03,0.002,0,0,0,0.0163,0,0.9201,0,0.0574,0.4134,0,0,0,0,0,0,0,0,0,0,24.46569855,0,0,0.05,0.04,0.1,1.51,0,0,0,0,0,0.21},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,88.15,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 100.0,81.1,76.0,72.5,60.0,61.4,47.0,60.4,71.4,62.8,65.1,68.5,66.1,61.3,27.7,42.8,0.0,0.0,11.9,0.0,45.9,0.0,0.0,55.1,80.8,27.4,78.7,83.9,70.2,68.3,64.1,72.7,66.7,85.0,65.5,61.9},

            new double[] { 100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,100,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,100,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,100},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,100,0,0,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,100,0,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,100,0,0,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,100,0,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,100,0,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,100,0,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,100,0,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,100,0,0},
            new double[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,100,0},
            new double[] { 100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100}
        };

        //crucial misunderstanding - the lowest sum of squares needs to relate to B8

        //fi[0] = 0;//(B6-0.01*SUMPRODUCT(B16:B51,[the column]))/(B6*B1/100+B2) where B6 = arbitrary number from csv (B1 and B2 are both constants)   (((remove (B6*B1/100+B2) for 'Other')))
        //Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        /*foreach (double d in x)
        {
            Debug.Log("x/" + d);
        }*/

        for (int acrossCounter = 0; acrossCounter < fi.Length; acrossCounter++)
        {
            double product = SumProduct(x, minerals[acrossCounter]);
            if (relativeError[acrossCounter] != -1)
            {
                fi[acrossCounter] = (combinedDoubleList[acrossCounter] - (0.01 * product)) / ((combinedDoubleList[acrossCounter] * (relativeError[acrossCounter] / 100) + absoluteError[acrossCounter]));
                //Debug.Log("<color=green>fi[" + acrossCounter + "] = (" + combiSampleValues[acrossCounter] + " - (0.01 * " + product + ")) / ((" + combiSampleValues[acrossCounter] + " * " + relativeError[acrossCounter] + " / 100) + " + absoluteError[acrossCounter] + ")) = " + fi[acrossCounter] + "</color>");
            }   //ONLY GOES UP TO fi[35] THATS WRONG
            else
            {
                fi[acrossCounter] = (combinedDoubleList[acrossCounter] - (0.01 * product));
                //Debug.Log("<color=yellow>fi[" + acrossCounter + "] = (" + combiSampleValues[acrossCounter] + " - (0.01 * " + product + ")) = " + fi[acrossCounter] + "</color>");
            }

        } //cahnge dimensionality of fi to the length of fi and not x, then try that out i guess
          //THEN MAYBE WE CAN ACTUALLY GET SOMEWHERE???

        /*foreach (double f in fi)
        {
            Debug.Log("fi/" + f);
        }*/
    }   //function1_fvec()


    public static void function1_rep(double[] arg, double func, object obj)
    {
        //Code goes here
        //Debug.Log("---- ");
        //foreach(double d in arg)
        //{
        //    Debug.Log("/" + d);
        //}

        exportFunc = func;
        //Debug.Log("func: " + func); //func is the sum of fi^2
    }   //function1_rep()
}
