using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class TestScript1 : MonoBehaviour {

    public FileChooser fileChooser;
    private string fileContentString;
    public alglib alg;
    public Export export;

    public LPPNamespace.LPP lpp;

    public CSVReader csvController = null;

    public Dropdown datasetDropdown;

    // Use this for initialization
    void Start () {
        //Doit();
        //alglib.read_csv();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OpenFileDialog(bool isAssay)
    {
        fileChooser.setup(FileChooser.OPENSAVE.OPEN, "csv");
        fileChooser.callbackYes = delegate (string filename) {
            fileChooser.gameObject.SetActive(false);
            if (File.Exists(filename))
            {
                ExtractAndLoadCSV(filename, isAssay);

            }
            else
            {
                Debug.Log("The file does not exist: " + filename);
            }
        };
        fileChooser.callbackNo = delegate () {
            fileChooser.gameObject.SetActive(false);
        };
    }

    void ExtractAndLoadCSV(string filename, bool isAssay)
    {

        string path = Application.dataPath;
        if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            path += "/../../";
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            path += "/../";
        }
        path += "/Mineral Tables/";
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] info = dir.GetFiles("*.csv");
        List<string> datasetStrings = new List<string>();
        foreach (FileInfo f in info)
        {
            datasetStrings.Add(f.Name);
        }
        datasetDropdown.AddOptions(datasetStrings);
        //turn off UI
        export.SetFilenameString(filename);
        StartCoroutine(ImportSamples(filename, isAssay));
    }


    private IEnumerator ImportSamples(string url, bool isAssay)
    {
        //FilenameText.text = url.Substring(url.LastIndexOf("\\") + 1);
        yield return StartCoroutine(FastDownload(url, fileContents => fileContentString = fileContents));
        csvController.csvFile = fileContentString;
        if (fileContentString != null && fileContentString.Length > 0 && fileContentString != "null")
        {
            csvController.InitialiseGrid(isAssay); 
        }
        else
        {
            Debug.Log("Error: File Content is null");
            //DisplayErrorMessage(errorMessage);
        }

        yield return null;

    }

    private IEnumerator FastDownload(string url, System.Action<string> result)
    {
        string s = "null";
        try
        {
            s = File.ReadAllText(url).TrimEnd('\r', '\n');

        }
        catch (IOException e)
        {
            Debug.Log("<color=red>Error: " + e.GetType().Name + ": " + e.Message + "</color>");
            //errorMessage = "Error! " + e.GetType().Name + ": " + e.Message;
        }
        yield return null;
        result(s);
    }

    public void CalcForRow(int row)
    {
        if (csvController.assayMode)
        {
            csvController.GetChemTestList(csvController.grid, row);
            lpp.StartLPP(csvController.chemTest);
        }
        else
        {

            /*add to list
            if (!heatmapPositionList.Contains(entry))
            {
                GameObject g = GameObject.Instantiate(listEntryPrefab) as GameObject;
                g.transform.SetParent(positionScrollView.transform, false);
                g.GetComponent<Text>().text = entry.ToString("F2");
                g.GetComponent<AutomatedScreenshotListEntry>().position = entry;
                g.GetComponent<AutomatedScreenshotListEntry>().hmb = this;
                heatmapPositionList.Add(entry);
            }*/
        }
    }


    public static void function1_rep(double[] arg, double func, object obj)
    {
        //Code goes here
        //Debug.Log("---- ");
        //foreach(double d in arg)
        //{
        //    Debug.Log("/" + d);
        //}
        Debug.Log("func: " + func); //func is the sum of fi^2
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
    }

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


        double[] relativeError = { 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, -1, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, -1};
        double[] absoluteError = { 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, -1, 0.50, 0.50, 0.50, 0.50, 0.10, 0.10, 0.10, 0.50, 0.50, 0.50, 0.10, 0.50, 0.10, 0.10, 0.50, 0.50, 0.50, 0.50, 0.10, 0.50, 0.10, 0.50, 0.50, 0.50, 0.10, 0.10, 0.10, 0.10, 0.10, -1 };
        double[] samples = { 8.67, 0.0021, 0.068, 1.22, 0.0149, 2.66, 2.92, 0.64, 0.0477, 0.0008, 3.97, 0.073, 0.01, 0.3, 0.00051, 79.40299, 15, 52, 23, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 5, 0, 0, 4, 0, 0, 0, 100 };

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
                fi[acrossCounter] = (samples[acrossCounter] - (0.01 * product)) / ((samples[acrossCounter] * (relativeError[acrossCounter] / 100) + absoluteError[acrossCounter]));
                if (acrossCounter < 1)
                    Debug.Log("<color=green>fi[" + acrossCounter + "] = (" + samples[acrossCounter] + " - (0.01 * " + product + ")) / ((" + samples[acrossCounter] + " * " + relativeError[acrossCounter] + " / 100) + " + absoluteError[acrossCounter] + ")) = " + fi[acrossCounter] + "</color>");
            }   //ONLY GOES UP TO fi[35] THATS WRONG
            else
            {
                fi[acrossCounter] = (samples[acrossCounter] - (0.01 * product));
                if(acrossCounter<1)
                    Debug.Log("<color=yellow>fi[" + acrossCounter + "] = (" + samples[acrossCounter] + " - (0.01 * " + product + ")) = " + fi[acrossCounter] + "</color>");
            }

        } //cahnge dimensionality of fi to the length of fi and not x, then try that out i guess
        //THEN MAYBE WE CAN ACTUALLY GET SOMEWHERE???

        
        /*foreach (double f in fi)
        {
            Debug.Log("fi/" + f);
        }*/
    }
    public static int Doit()
    {
        //
        // This example demonstrates minimization of F(x0,x1) = f0^2+f1^2, where 
        //
        //     f0(x0,x1) = 10*(x0+3)^2
        //     f1(x0,x1) = (x1-3)^2
        //
        // using "V" mode of the Levenberg-Marquardt optimizer.
        //
        // Optimization algorithm uses:
        // * function vector f[] = {f1,f2}
        //
        // No other information (Jacobian, gradient, etc.) is needed.
        //
        double[] samples = { 8.67, 0.0021, 0.068, 1.22, 0.0149, 2.66, 2.92, 0.64, 0.0477, 0.0008, 3.97, 0.073, 0.01, 0.3, 0.00051, 79.40299, 15, 52, 23, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 5, 0, 0, 4, 0, 0, 0, 100 };
        double[] x = new double[] { 15, 52, 23, 0, 0, 0, 0, 0, 0.5, 0, 0, 0, 0, 0, 1.0, 0, 0, 0, 0, 0, 0, 0, 0, 1.2, 0, 0.1, 1, 5, 0.5, 0, 0, 4, 0, 0, 0, 0 };//{ 1, 1, 1 };//
        double[] bndl = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//{ -1, +1, -1 }; //lower = 0
        double[] bndu = new double[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };//{ +1, +3, +1 }; //upper = 100
        double epsx = 0.0000000001;
        int maxits = 0;
        alglib.minlmstate state;
        alglib.minlmreport rep;

        string secondArray = "";
        foreach (double d in x)
        {
            secondArray += d.ToString() + ", ";
        }
        Debug.Log("x: " + secondArray);

        string firstArray = "";
        foreach (double d in samples)
        {
            firstArray += d.ToString() + ", ";
        }
        Debug.Log("samples: " + firstArray);

        Debug.Log("LENGTH OF SAMPLES: " + samples.Length);
        Debug.Log("LENGTH OF X: " + x.Length);

        double[] relativeError = { 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, -1, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, -1 };
        double[] absoluteError = { 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, -1, 0.50, 0.50, 0.50, 0.50, 0.10, 0.10, 0.10, 0.50, 0.50, 0.50, 0.10, 0.50, 0.10, 0.10, 0.50, 0.50, 0.50, 0.50, 0.10, 0.50, 0.10, 0.50, 0.50, 0.50, 0.10, 0.10, 0.10, 0.10, 0.10, -1 };


        Debug.Log("relativeError: " + DebugWholeArray(relativeError) + " (" + relativeError.Length + ")");
        Debug.Log("absoluteError: " + DebugWholeArray(absoluteError) + " (" + absoluteError.Length + ")");
        Debug.Log("X: " + DebugWholeArray(x) + " (" + x.Length + ")");
        Debug.Log("bndl: " + DebugWholeArray(bndl) + " (" + bndl.Length + ")");
        Debug.Log("bndu: " + DebugWholeArray(bndu) + " (" + bndu.Length + ")");

        alglib.minlmcreatev(samples.Length, x, 0.0001, out state);
        alglib.minlmsetbc(state, bndl, bndu);
        alglib.minlmsetcond(state, epsx, maxits);
        alglib.minlmsetxrep(state, true);
        alglib.minlmoptimize(state, function1_fvec, function1_rep, null);
        alglib.minlmresults(state, out x, out rep);

        Debug.Log("thething: " + alglib.ap.format(x, 2)); // EXPECTED: [-3,+3]
        //System.Console.ReadLine();
        return 0;
    }
    public static string DebugWholeArray(string[] array)
    {

        string outputLine = "";
        for (int l = 0; l < array.Length; l++)
        {
            //Debug.Log("1 = " + u + ", 2 = " + l);
            outputLine += array[l].ToString() + ", ";
        }
        return outputLine;
    }

    public static string DebugWholeArray(double[] array)
    {

        string outputLine = "";
        for (int l = 0; l < array.Length; l++)
        {
            //Debug.Log("1 = " + u + ", 2 = " + l);
            outputLine += array[l].ToString() + ", ";
        }
        return outputLine;
    }
    public static void function1_fvec2(double[] x, double[] fi, object obj)
    {
        //
        // this callback calculates
        // f0(x0,x1) = 100*(x0+3)^4,
        // f1(x0,x1) = (x1-3)^4
        //
        fi[0] = 10 * System.Math.Pow(x[0] + 3, 2);
        fi[1] = System.Math.Pow(x[1] - 3, 2);
    }
    public void LevenbergTest()
    {
        //
        // This example demonstrates minimization of F(x0,x1) = f0^2+f1^2, where 
        //
        //     f0(x0,x1) = 10*(x0+3)^2
        //     f1(x0,x1) = (x1-3)^2
        //
        // with boundary constraints
        //
        //     -1 <= x0 <= +1
        //     -1 <= x1 <= +1
        //
        // using "V" mode of the Levenberg-Marquardt optimizer.
        //
        // Optimization algorithm uses:
        // * function vector f[] = {f1,f2}
        //
        // No other information (Jacobian, gradient, etc.) is needed.
        //
        double[] x = new double[] { 0, 0 };
        double[] bndl = new double[] { -1, -1 };
        double[] bndu = new double[] { +1, +1 };
        double epsx = 0.0000000001;
        int maxits = 0;
        alglib.minlmstate state;
        alglib.minlmreport rep;

        alglib.minlmcreatev(2, x, 0.0001, out state);
        alglib.minlmsetbc(state, bndl, bndu);
        alglib.minlmsetcond(state, epsx, maxits);
        alglib.minlmoptimize(state, function1_fvec2, null, null);
        alglib.minlmresults(state, out x, out rep);

        System.Console.WriteLine("{0}", alglib.ap.format(x, 2)); // EXPECTED: [-1,+1]
        System.Console.ReadLine();
    }


    
}
