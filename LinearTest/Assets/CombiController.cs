using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class CombiController : MonoBehaviour {

    public FileChooser fileChooser;
    private string fileContentString;
    public alglib alg;
    public Export export;
    public List<string> samples = new List<string>();
    public List<Toggle> sampleToggles = new List<Toggle>();
    public static double[] combiSampleValues = { 8.67, 0.0021, 0.068, 1.22, 0.0149, 2.66, 2.92, 0.64, 0.0477, 0.0008, 3.97, 0.073, 0.01, 0.3, 0.00051, 79.40299, 15, 52, 23, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 5, 0, 0, 4, 0, 0, 0, 100 };

    public GameObject mainMenu;
    public GameObject combiMenu;
    public CSVReader csvController;
    public BothController bothController;
    public GameObject samplePrefab;
    public GameObject sampleScrollView; //content

    public double[] elementDoubleList;
    public double[] chemicalDoubleList;
    public static double[] combinedDoubleList;

    public static double exportFunc = -1;
    public static string exportResult = "N/A";

    // Use this for initialization
    void Start () {
        //Debug.Log("The calculation here is wrong. Need to compare it with the original version to check what the problem is. Seems to lie in the sumproduct of x and [THING], but there's also an issue where the final value (100) isn't being accounted for.");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    /*
    public void InitialiseCombi()
    {
        mainMenu.SetActive(false);
        combiMenu.SetActive(true);
        for(int i = 0; i < csvController.samplePositions.Count; i++)
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
    }*/

    public void CalculateCombi(bool toCSV)
    {
        string fullExportString = "";
        string columnNames = "Sample No.,Score";


        List<string> minList = new List<string>();
        List<double> weightList = new List<double>();

        List<string> elementList = new List<string>();
        List<int> columnIndexList = new List<int>();
        double result;

        foreach (Toggle tog in bothController.mineralCompScrollView.transform.GetComponentsInChildren<Toggle>())
        {
            if (tog.isOn)
            {
                string minString = tog.gameObject.GetComponent<MineralCompositionListEntry>().MineralComp;
                minList.Add(minString);
                weightList.Add(double.TryParse(tog.GetComponentInChildren<InputField>().text, out result) ? result : 1);
                columnNames += "," + minString;
            }
        }
        foreach (Toggle tog in bothController.elementCompScrollView.transform.GetComponentsInChildren<Toggle>())
        {
            if (tog.isOn)
            {
                string elemString = tog.gameObject.GetComponent<MineralCompositionListEntry>().MineralComp;
                minList.Add(elemString);
                weightList.Add(double.TryParse(tog.GetComponentInChildren<InputField>().text, out result) ? result : -100);
                columnNames += "," + elemString;
            }
        }
        foreach (Toggle tog in bothController.columnScrollView.transform.GetComponentsInChildren<Toggle>())
        {
            if (tog.isOn)
            {
                string elem = tog.gameObject.GetComponent<SampleListEntry>().SampleID;
                elementList.Add(elem.Substring(0, elem.IndexOf('_')));//, elem.Length-elem.IndexOf('_')));
                columnIndexList.Add(tog.gameObject.GetComponent<SampleListEntry>().index);
                Debug.Log("it = " + elem.Substring(0, elem.IndexOf('_')));//, elem.Length - elem.IndexOf('_')));
            }
        }
        if (toCSV)
            export.WriteStringToStringBuilder(columnNames);
        else
        {
            export.WriteStringToStringBuilder(System.DateTime.Now.ToString("dd-MM-yyyy") + "_" + System.DateTime.Now.ToString("hh-mmtt"));
            export.WriteStringToStringBuilder("==========");
        }

        foreach (Toggle tog in sampleToggles)
        {
            if(tog.isOn)
            {
                //calculate for sampleID
                //this involves combiSampleValues 
                int s = tog.gameObject.GetComponent<SampleListEntry>().index;
                elementDoubleList = new double[csvController.elementPositions.Count+1];
                int i = 0;
                foreach (int ep in csvController.elementPositions)
                {
                    //Debug.Log("element: " + ep + "," + s + " = " + csvController.grid[ep, s]);
                    if (!double.TryParse(csvController.grid[ep, s], out elementDoubleList[i]))
                        Debug.Log("<color=red>error: could not parse double</color>");
                    i++;
                }
                double otherDouble = 100;
                for(int j = 0; j < csvController.elementPositions.Count; j++)
                {
                    otherDouble -= elementDoubleList[j];
                }
                elementDoubleList[csvController.elementPositions.Count] = otherDouble;

                chemicalDoubleList = new double[csvController.chemicalPositions.Count];
                i = 0;
                foreach (int cp in csvController.chemicalPositions)
                {
                    //Debug.Log("chemical: " + cp + "," + s + " = " + csvController.grid[cp, s]);
                    if (!double.TryParse(csvController.grid[cp, s], out chemicalDoubleList[i]))
                        Debug.Log("<color=red>error: could not parse double</color>");
                    i++;
                }
                combinedDoubleList = new double[elementDoubleList.Length + chemicalDoubleList.Length + 1];
                Array.Copy(elementDoubleList, combinedDoubleList, elementDoubleList.Length);
                Array.Copy(chemicalDoubleList, 0, combinedDoubleList, elementDoubleList.Length, chemicalDoubleList.Length);
                combinedDoubleList[combinedDoubleList.Length-1] = 100;


                //double[] x = new double[chemicalDoubleList.Length]; //{ 15, 52, 23, 0, 0, 0, 0, 0, 0.5, 0, 0, 0, 0, 0, 1.0, 0, 0, 0, 0, 0, 0, 0, 0, 1.2, 0, 0.1, 1, 5, 0.5, 0, 0, 4, 0, 0, 0, 0 };//{ 1, 1, 1 };//
                //Array.Copy(chemicalDoubleList, x, chemicalDoubleList.Length);
                double[] x = new double[] { 15, 52, 23, 0, 0, 0, 0, 0, 0.5, 0, 0, 0, 0, 0, 1.0, 0, 0, 0, 0, 0, 0, 0, 0, 1.2, 0, 0.1, 1, 5, 0.5, 0, 0, 4, 0, 0, 0, 0 };
                double[] bndl = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//{ -1, +1, -1 }; //lower = 0
                double[] bndu = new double[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };//{ +1, +3, +1 }; //upper = 100
                double epsx = 0.0000000001;
                int maxits = 0;
                alglib.minlmstate state;
                alglib.minlmreport rep;


                /*string firstArray = "";
                foreach (double d in chemicalDoubleList)
                {
                    firstArray += d.ToString() + ", ";
                }
                Debug.Log("chemicalDoubleList: " + firstArray);

                string fourthArray = "";
                foreach (double d in elementDoubleList)
                {
                    fourthArray += d.ToString() + ", ";
                }
                Debug.Log("elementDoubleList: " + fourthArray);


                string thirdArray = "";
                foreach (double d in combinedDoubleList)
                {
                    thirdArray += d.ToString() + ", ";
                }
                Debug.Log("combinedDoubleList: " + thirdArray);

                Debug.Log("LENGTH OF SAMPLES: " + combinedDoubleList.Length);
                Debug.Log("LENGTH OF X: " + x.Length);*/


                x[0] = chemicalDoubleList[0];
                x[1] = chemicalDoubleList[1];
                x[2] = chemicalDoubleList[2];
                x[3] = chemicalDoubleList[3];
                x[4] = chemicalDoubleList[4];
                x[5] = chemicalDoubleList[5];
                x[6] = chemicalDoubleList[6];
                x[7] = chemicalDoubleList[7];
                x[8] = chemicalDoubleList[8] * 0.5;
                x[9] = chemicalDoubleList[9];
                x[10] = chemicalDoubleList[10];
                x[11] = chemicalDoubleList[11] * 0.5;
                x[12] = chemicalDoubleList[11] * 0.5;
                x[13] = chemicalDoubleList[13];
                x[14] = chemicalDoubleList[14];
                x[15] = chemicalDoubleList[15];
                x[16] = chemicalDoubleList[16];
                x[17] = chemicalDoubleList[17];
                x[18] = elementDoubleList[14] / 0.8;
                x[19] = elementDoubleList[1] / 0.5;
                x[20] = chemicalDoubleList[18];
                x[21] = chemicalDoubleList[19];
                x[22] = 0.01;
                x[23] = elementDoubleList[13] / 0.25;
                x[24] = chemicalDoubleList[20];
                x[25] = elementDoubleList[2] / 0.6;
                x[26] = chemicalDoubleList[21];
                x[27] = chemicalDoubleList[22];
                x[28] = chemicalDoubleList[8] * 0.5;
                x[29] = chemicalDoubleList[23];
                x[30] = chemicalDoubleList[24];
                x[31] = chemicalDoubleList[25];
                x[32] = chemicalDoubleList[26];
                x[33] = chemicalDoubleList[27];
                x[34] = chemicalDoubleList[28];
                x[35] = chemicalDoubleList[12];

                string secondArray = "";
                /*foreach (double d in x)
                {
                    secondArray += d.ToString() + ", ";
                }
                Debug.Log("x: " + secondArray);*/

                alglib.minlmcreatev(combinedDoubleList.Length, x, 0.0001, out state);
                alglib.minlmsetbc(state, bndl, bndu);
                alglib.minlmsetcond(state, epsx, maxits);
                alglib.minlmsetxrep(state, true);
                alglib.minlmoptimize(state, function1_fvec, function1_rep, null);
                alglib.minlmresults(state, out x, out rep);

                //Debug.Log("thething: " + alglib.ap.format(x, 2));// EXPECTED: [-3,+3]
                exportResult = alglib.ap.format(x, 2);

                string id = csvController.grid[0, s];

                int degreesOfFreedom = combinedDoubleList.Length - x.Length;

                double chiSquare = ChiSquarePval(Math.Sqrt(exportFunc), degreesOfFreedom);

                string truncatedExportResult = exportResult.Substring(1, exportResult.Length - 2);

                export.WriteStringToStringBuilder(id + "," + truncatedExportResult + "," + exportFunc + "," + degreesOfFreedom + "," + chiSquare);
            }


        }
    }   //CalculateCombi()


    public IEnumerator EnumerateCombi(bool toCSV)
    {
        yield return null;
    }

    public void SetSampleValuesList(double[] sampleArray)
    {
        combiSampleValues = sampleArray;
    }


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



    public void oldCalcCombi()
    {
        foreach (Toggle tog in sampleToggles)
        {
            if (tog.isOn)
            {
                //calculate for sampleID
                //this involves combiSampleValues 
                int s = tog.gameObject.GetComponent<SampleListEntry>().index;
                elementDoubleList = new double[csvController.elementPositions.Count + 1];
                int i = 0;
                foreach (int ep in csvController.elementPositions)
                {
                    //Debug.Log("element: " + ep + "," + s + " = " + csvController.grid[ep, s]);
                    if (!double.TryParse(csvController.grid[ep, s], out elementDoubleList[i]))
                        Debug.Log("<color=red>error: could not parse double</color>");
                    i++;
                }
                double otherDouble = 100;
                for (int j = 0; j < csvController.elementPositions.Count; j++)
                {
                    otherDouble -= elementDoubleList[j];
                }
                elementDoubleList[csvController.elementPositions.Count] = otherDouble;

                chemicalDoubleList = new double[csvController.chemicalPositions.Count];
                i = 0;
                foreach (int cp in csvController.chemicalPositions)
                {
                    //Debug.Log("chemical: " + cp + "," + s + " = " + csvController.grid[cp, s]);
                    if (!double.TryParse(csvController.grid[cp, s], out chemicalDoubleList[i]))
                        Debug.Log("<color=red>error: could not parse double</color>");
                    i++;
                }
                combinedDoubleList = new double[elementDoubleList.Length + chemicalDoubleList.Length + 1];
                Array.Copy(elementDoubleList, combinedDoubleList, elementDoubleList.Length);
                Array.Copy(chemicalDoubleList, 0, combinedDoubleList, elementDoubleList.Length, chemicalDoubleList.Length);
                combinedDoubleList[combinedDoubleList.Length - 1] = 100;


                //double[] x = new double[chemicalDoubleList.Length]; //{ 15, 52, 23, 0, 0, 0, 0, 0, 0.5, 0, 0, 0, 0, 0, 1.0, 0, 0, 0, 0, 0, 0, 0, 0, 1.2, 0, 0.1, 1, 5, 0.5, 0, 0, 4, 0, 0, 0, 0 };//{ 1, 1, 1 };//
                //Array.Copy(chemicalDoubleList, x, chemicalDoubleList.Length);
                double[] x = new double[] { 15, 52, 23, 0, 0, 0, 0, 0, 0.5, 0, 0, 0, 0, 0, 1.0, 0, 0, 0, 0, 0, 0, 0, 0, 1.2, 0, 0.1, 1, 5, 0.5, 0, 0, 4, 0, 0, 0, 0 };
                double[] bndl = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//{ -1, +1, -1 }; //lower = 0
                double[] bndu = new double[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };//{ +1, +3, +1 }; //upper = 100
                double epsx = 0.0000000001;
                int maxits = 0;
                alglib.minlmstate state;
                alglib.minlmreport rep;


                /*string firstArray = "";
                foreach (double d in chemicalDoubleList)
                {
                    firstArray += d.ToString() + ", ";
                }
                Debug.Log("chemicalDoubleList: " + firstArray);

                string fourthArray = "";
                foreach (double d in elementDoubleList)
                {
                    fourthArray += d.ToString() + ", ";
                }
                Debug.Log("elementDoubleList: " + fourthArray);


                string thirdArray = "";
                foreach (double d in combinedDoubleList)
                {
                    thirdArray += d.ToString() + ", ";
                }
                Debug.Log("combinedDoubleList: " + thirdArray);

                Debug.Log("LENGTH OF SAMPLES: " + combinedDoubleList.Length);
                Debug.Log("LENGTH OF X: " + x.Length);*/


                x[0] = chemicalDoubleList[0];
                x[1] = chemicalDoubleList[1];
                x[2] = chemicalDoubleList[2];
                x[3] = chemicalDoubleList[3];
                x[4] = chemicalDoubleList[4];
                x[5] = chemicalDoubleList[5];
                x[6] = chemicalDoubleList[6];
                x[7] = chemicalDoubleList[7];
                x[8] = chemicalDoubleList[8] * 0.5;
                x[9] = chemicalDoubleList[9];
                x[10] = chemicalDoubleList[10];
                x[11] = chemicalDoubleList[11] * 0.5;
                x[12] = chemicalDoubleList[11] * 0.5;
                x[13] = chemicalDoubleList[13];
                x[14] = chemicalDoubleList[14];
                x[15] = chemicalDoubleList[15];
                x[16] = chemicalDoubleList[16];
                x[17] = chemicalDoubleList[17];
                x[18] = elementDoubleList[14] / 0.8;
                x[19] = elementDoubleList[1] / 0.5;
                x[20] = chemicalDoubleList[18];
                x[21] = chemicalDoubleList[19];
                x[22] = 0.01;
                x[23] = elementDoubleList[13] / 0.25;
                x[24] = chemicalDoubleList[20];
                x[25] = elementDoubleList[2] / 0.6;
                x[26] = chemicalDoubleList[21];
                x[27] = chemicalDoubleList[22];
                x[28] = chemicalDoubleList[8] * 0.5;
                x[29] = chemicalDoubleList[23];
                x[30] = chemicalDoubleList[24];
                x[31] = chemicalDoubleList[25];
                x[32] = chemicalDoubleList[26];
                x[33] = chemicalDoubleList[27];
                x[34] = chemicalDoubleList[28];
                x[35] = chemicalDoubleList[12];

                string secondArray = "";
                /*foreach (double d in x)
                {
                    secondArray += d.ToString() + ", ";
                }
                Debug.Log("x: " + secondArray);*/

                alglib.minlmcreatev(combinedDoubleList.Length, x, 0.0001, out state);
                alglib.minlmsetbc(state, bndl, bndu);
                alglib.minlmsetcond(state, epsx, maxits);
                alglib.minlmsetxrep(state, true);
                alglib.minlmoptimize(state, function1_fvec, function1_rep, null);
                alglib.minlmresults(state, out x, out rep);

                //Debug.Log("thething: " + alglib.ap.format(x, 2));// EXPECTED: [-3,+3]
                exportResult = alglib.ap.format(x, 2);

                string id = csvController.grid[0, s];

                int degreesOfFreedom = combinedDoubleList.Length - x.Length;

                double chiSquare = ChiSquarePval(Math.Sqrt(exportFunc), degreesOfFreedom);

                string truncatedExportResult = exportResult.Substring(1, exportResult.Length - 2);

                export.WriteStringToStringBuilder(id + "," + truncatedExportResult + "," + exportFunc + "," + degreesOfFreedom + "," + chiSquare);
            }


        }
    }   //CalculateCombi()
}
