using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class TrainingController : MonoBehaviour {

    public alglib alg;
    public FileChooser fileChooser;
    public string CombiResultsFilename;
    public string AssayTestingFilename;
    public Text CombiResultsText;
    public Text AssayTestingText;
    public BothController bothController;

    public Dropdown datasetDropdown;

    string path;

    string[,] CombiResultTable;
    string[,] AssayTrainingTable;
    string[,] DatasetTable;

    public GameObject QXRDPrefab;
    public GameObject QXRDScrollview;

    List<float> bestWeights = new List<float>();

    // Use this for initialization
    void Start() {
        PopulateDatasetDropdown();
    }

    // Update is called once per frame
    void Update() {

    }



    public void LoadCombiResultsCSV()
    {

        fileChooser.setup(FileChooser.OPENSAVE.OPEN, "csv");
        fileChooser.callbackYes = delegate (string filename) {
            fileChooser.gameObject.SetActive(false);
            if (File.Exists(filename))
            {
                CombiResultsFilename = filename;
                CombiResultsText.text = filename;
                //ExtractAndLoadCSV(filename, isAssay);

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
    public void LoadAssayTestingCSV()
    {

        fileChooser.setup(FileChooser.OPENSAVE.OPEN, "csv");
        fileChooser.callbackYes = delegate (string filename) {
            fileChooser.gameObject.SetActive(false);
            if (File.Exists(filename))
            {
                AssayTestingFilename = filename;
                AssayTestingText.text = filename;
                //ExtractAndLoadCSV(filename, isAssay);

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


    public void PopulateDatasetDropdown()
    {
        path = Application.dataPath;
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
    }

    public void BeginTraining()
    {
        StartCoroutine("CreateAndValidateTables");
    }
    /*
    public IEnumerable<T> SliceRow<T>(this T[,] array, int row)
    {
        for (var i = array.GetLowerBound(1); i <= array.GetUpperBound(1); i++)
        {
            yield return array[row, i];
        }
    }

    public IEnumerable<T> SliceColumn<T>(this T[,] array, int column)
    {
        for (var i = array.GetLowerBound(0); i <= array.GetUpperBound(0); i++)
        {
            yield return array[i, column];
        }
    }*/
    public string[] GetSlice(string[,] grid, int index, bool isColumn)
    {
        string[] slice;
        if (isColumn)
        {
            slice = new string[grid.GetUpperBound(1) + 1];
            for (int y = 0; y <= grid.GetUpperBound(1); y++)
                slice[y] = grid[index, y];
        }
        else
        {
            slice = new string[grid.GetUpperBound(0) + 1];
            for (int x = 0; x <= grid.GetUpperBound(0); x++)
                slice[x] = grid[x, index];

        }
        return slice;
    }


    public string[] GetSlice(string[,] grid, string index, bool isColumn)
    {
        string[] slice;
        if (isColumn)
        {
            int indexVal = -1;
            for (int x = 0; x <= grid.GetUpperBound(0); x++)
                if (MatchesNoCase(grid[x, 0], index))
                    indexVal = x;
            if (indexVal != -1)
            {
                slice = new string[grid.GetUpperBound(1) + 1];
                for (int y = 0; y <= grid.GetUpperBound(1); y++)
                    slice[y] = grid[indexVal, y];
            }
            else
            {
                Debug.Log("<color=red>Error: Could not find column with the header '" + index + "'");
                slice = new string[0];
            }
        }
        else
        {
            int indexVal = -1;
            for (int y = 0; y <= grid.GetUpperBound(0); y++)
                if (MatchesNoCase(grid[0, y], index))
                    indexVal = y;
            if (indexVal != -1)
            {
                slice = new string[grid.GetUpperBound(0) + 1];
                for (int x = 0; x <= grid.GetUpperBound(0); x++)
                    slice[x] = grid[x, indexVal];
            }
            else
            {
                Debug.Log("<color=red>Error: Could not find column with the header '" + index + "'");
                slice = new string[0];
            }

        }
        return slice;
    }


    public IEnumerator CreateAndValidateTables()
    {
        string fileContentString = "";
        //Parse Assay file
        yield return StartCoroutine(FastDownload(AssayTestingFilename, fileContents => fileContentString = fileContents));
        AssayTrainingTable = CSVReader.SplitCsvGrid(fileContentString);

        //Parse combi file
        yield return StartCoroutine(FastDownload(CombiResultsFilename, fileContents => fileContentString = fileContents));
        CombiResultTable = CSVReader.SplitCsvGrid(fileContentString);

        //Parse dataset
        yield return StartCoroutine(FastDownload(path + datasetDropdown.options[datasetDropdown.value].text, fileContents => fileContentString = fileContents));
        DatasetTable = CSVReader.SplitCsvGrid(fileContentString);

        Dictionary<string, BothController.MineralComposition> mineralDict = new Dictionary<string, BothController.MineralComposition>();

        for (int i = 1; i <= DatasetTable.GetUpperBound(1); i++)
        {
            string columnHeader = DatasetTable[0, i];

            string mineralComp = DatasetTable[0, i];
            BothController.MineralComposition MC = new BothController.MineralComposition(mineralComp);
            double[] val = new double[] { Double.Parse(DatasetTable[1, i]),
                                                    Double.Parse(DatasetTable[2, i]),
                                                    Double.Parse(DatasetTable[3, i]),
                                                    Double.Parse(DatasetTable[4, i]),
                                                    Double.Parse(DatasetTable[5, i]),
                                                    Double.Parse(DatasetTable[6, i]),
                                                    Double.Parse(DatasetTable[7, i]),
                                                    Double.Parse(DatasetTable[8, i]),
                                                    Double.Parse(DatasetTable[9, i]),
                                                    Double.Parse(DatasetTable[10, i]),
                                                    Double.Parse(DatasetTable[11, i]),
                                                    Double.Parse(DatasetTable[12, i]),
                                                    Double.Parse(DatasetTable[13, i]),
                                                    Double.Parse(DatasetTable[14, i]),
                                                    Double.Parse(DatasetTable[15, i]),
                                                    Double.Parse(DatasetTable[16, i]),
                                                    Double.Parse(DatasetTable[17, i]),
                                                    Double.Parse(DatasetTable[18, i]),
                                                    Double.Parse(DatasetTable[19, i]),
                                                    Double.Parse(DatasetTable[20, i]),
                                                    Double.Parse(DatasetTable[21, i]) };
            bothController.FillMCDatabase(MC, val);
            MC.weight = double.Parse(DatasetTable[22, i]);
            MC.startPoint = double.Parse(DatasetTable[23, i]);

            mineralDict.Add(mineralComp, MC);

        }



        FillQXRDList();

        yield return null;
    }

    public void DoValidate()
    {
        //CHECK:
        //Does dataset grid have the name number of rows as combi results grid has columns?
        //Does Assay have the same number of samples as Combi?

        bool isValid = true;
        for (int y = 1; y <= AssayTrainingTable.GetUpperBound(1); y++)
        {
            string[] b = { "", "" };
            Debug.Log("does combicolumn contain " + AssayTrainingTable[0, y]);
            if (GetSlice(CombiResultTable, 0, true).Any(AssayTrainingTable[0, y].Contains))
                Debug.Log("yep");
        }



    }

    public void FillQXRDList()
    {
        QXRDListEntry QLE;
        List<string> mineralList = new List<string>();

        foreach (string column in GetSlice(CombiResultTable, 0, false))
        {
            if (column != "" && column != "Residual SSQ" && column != "degreesFreedom" && column != "probability" && column != "Sample no")
                mineralList.Add(column);
        }
        int i = 0;
        foreach (string row in GetSlice(DatasetTable, 0, true))
        {
            GameObject g = GameObject.Instantiate(QXRDPrefab) as GameObject;
            g.transform.SetParent(QXRDScrollview.transform, false);
            QLE = g.GetComponent<QXRDListEntry>();
            QLE.index = i;
            QLE.SetLabel(row);
            QLE.AddToDropdown(mineralList);
            int optionVal = 0;
            foreach (Dropdown.OptionData option in QLE.dropdown.options)
            {
                if (option.text.Length > 2 && QLE.MineralComp.Length > 2)
                {
                    if (ContainsNoCase(option.text, QLE.MineralComp.Substring(0, 3)))
                    {
                        Debug.Log(option.text + " has first three letters of " + QLE.MineralComp.Substring(0, 3));
                        QLE.dropdown.value = optionVal;
                    }
                    optionVal++;
                }
            }
            i++;

        }
    }

    public void TrainAssay()
    {



        Debug.Log("THE NEXT STEP IS LIKELY TO TURN THIS FOR LOOP INTO A SINGLE EXECUTION AND PASS THE NECESSARY VALUES TO VARIABLES THAT CAN HANDLE THE NEXT LOOPS AFTER THE CALCULATION IS DONE");




        string[] groupColumn = GetSlice(DatasetTable, "group", true);
        string[] maxWeightsColumn = GetSlice(DatasetTable, "max", true);
        string[] minWeightsColumn = GetSlice(DatasetTable, "min", true);
        List<int> group1Indexes = new List<int>();
        List<int> group2Indexes = new List<int>();
        List<int> group3Indexes = new List<int>();
        List<int> group4Indexes = new List<int>();
        List<int> group5Indexes = new List<int>();
        List<float> maxWeights = new List<float>();
        List<float> minWeights = new List<float>();

        //for each of the items in the QXRD list that arent paired with a None, cross check their index with the groupcolumn
        //if it's 0, nothing happens
        //if it's greater than zero,
        //      
        int i = 0;
        int row = 1;
        foreach (Transform QXRD in QXRDScrollview.transform)
        {
            QXRDListEntry QLE = QXRD.GetComponent<QXRDListEntry>();
            //if the paired mineral is not 'None'
            if (QLE.dropdown.value != 0)
            {
                float f;
                if (float.TryParse(maxWeightsColumn[row], out f))
                {
                    bestWeights.Add(f);
                    maxWeights.Add(f);
                }
                else
                {
                    bestWeights.Add(0);
                    maxWeights.Add(0);
                }
                if (float.TryParse(minWeightsColumn[row], out f))
                {
                    minWeights.Add(f);
                }
                else
                {
                    minWeights.Add(0);
                }
                if (groupColumn[row] == "1")
                    group1Indexes.Add(i);
                if (groupColumn[row] == "2")
                    group2Indexes.Add(i);
                if (groupColumn[row] == "3")
                    group3Indexes.Add(i);
                if (groupColumn[row] == "4")
                    group4Indexes.Add(i);
                if (groupColumn[row] == "5")
                    group5Indexes.Add(i);

                i++;
            }
            row++;
        }


        //RON'S PSEUDOLOOP
        //bestWeights = create first list of weights
        //while(number of completed loops < 3)
        //for(int m = 0; m < numberofMineralCompositionsinGroup1; m++)
        //  thisMin = listOfMinsInGroup1(m)
        //  thisMinWeights = 0, min, (0.75*min+0.25*max), (min+max)/2, (0.25*min+0.75*max), max
        //  for(int j = 0; j <= 5; j++)
        //      minWeights(thisMin) = thisMinWeights[j]
        //      startCoroutine(EnumerateAssayWeightIterator)
        //      if(absluteDifference < currentDifference)
        //          bestWeights[thisMin] = thisMinWeights[j]
        //repeat for loop for other groups
        
        //Group 1
        for(int g = 0; g < group1Indexes.Count; g++)
        {
            float min = minWeights[group1Indexes[g]];
            float max = maxWeights[group1Indexes[g]];

            float[] weightRange = { 0f, min, (0.75f * min) + (0.25f * max), (min+max)/2, (0.25f * min) + (0.75f * max), max };
            List<float> currentWeights = bestWeights;
            for (int w = 0; w <= 5; w++)
            {
                currentWeights[group1Indexes[g]] = w;

            }
        }
    }

    public string[] GetColumn(string[,] grid, int column)
    {
        //return grid[column];
        string[] toReturn = { };
        return toReturn;
    }

    private IEnumerator FastDownload(string url, System.Action<string> result)
    {
        string s = "";
        try
        {
            //s = File.ReadAllText(url).TrimEnd('\r', '\n');
            //s = File.ReadAllText(url).Split(" \r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(c => c.Length > 0);
            foreach (string b in File.ReadAllLines(url).Where(line => line != ""))
                if (Regex.IsMatch(b, "[^,]"))//.TrimEnd('\r', '\n')
                    s += b + "\n";//Debug.Log(b);
        }
        catch (IOException e)
        {
            Debug.Log("<color=red>Error: " + e.GetType().Name + ": " + e.Message + "</color>");
            //errorMessage = "Error! " + e.GetType().Name + ": " + e.Message;
        }
        yield return null;
        result(s);
    }


    public IEnumerator EnumerateAssayWeightIterator(bool toCSV)
    {

        //string fullExportString = "";
        //string columnNames = "Sample No.,Score";


        List<string> minList = new List<string>();
        List<int> combiColumnPositions = new List<int>();
        List<double> weightList = new List<double>();

        List<string> elementList = new List<string>();
        List<int> columnIndexList = new List<int>();
        double result;



        //Compile list of mineral compositions to be used in the calculations from the scrollviews of minerals and elements
        //but do this elsewhere, it's so inefficient
        foreach (Transform child in QXRDScrollview.transform)
        {
            QXRDListEntry qle = child.gameObject.GetComponent<QXRDListEntry>();
            if (qle.dropdown.value != 0)
            {
                //add in the mineral compositions
                //these should be the ones that are the qxrd list names, not the dropdowns
                string minString = qle.MineralComp;
                minList.Add(minString);
                combiColumnPositions.Add(qle.dropdown.value);
                //weightList.Add(double.TryParse(tog.GetComponentInChildren<InputField>().text, out result) ? result : 1);
                Debug.Log("COME BACK TO THE WEIGHT");
                //columnNames += "," + minString;
            }
        }
        //probably include all elements?
        /*foreach (Toggle tog in bothController.elementCompScrollView.transform.GetComponentsInChildren<Toggle>())
        {
            if (tog.isOn)
            {
                string elemString = tog.gameObject.GetComponent<MineralCompositionListEntry>().MineralComp;
                tog.isOn = true;
                minList.Add(elemString);
                weightList.Add(double.TryParse(tog.GetComponentInChildren<InputField>().text, out result) ? result : -100);
                columnNames += "," + elemString;

            }
        }*/

        //==========================================================================================================================
        //compile list of samples to use from the samples that are shared between combiresults and assaytraining
        //and do it elsewhere
        /*foreach (Toggle tog in bothController.columnScrollView.transform.GetComponentsInChildren<Toggle>())
        {
            if (tog.isOn)
            {
                string elem = tog.gameObject.GetComponent<SampleListEntry>().SampleID;
                elementList.Add(elem.Substring(0, elem.IndexOf('_')));//, elem.Length-elem.IndexOf('_')));
                columnIndexList.Add(tog.gameObject.GetComponent<SampleListEntry>().index);
            }
        }*/

        int maxSamples = 0;

        foreach (Toggle tog in bothController.sampleToggles)
        {
            if (tog.isOn)
            {
                maxSamples += 1;
            }
        }
        bothController.progressBar.value = 0;
        bothController.progressBar.maxValue = maxSamples;

        foreach (Toggle tog in bothController.sampleToggles)
        {
            if (tog.isOn)
            {
                int s = tog.gameObject.GetComponent<SampleListEntry>().index;

                //'chemtest' list is a list of all element values in the given row
                //csvController.GetChemTestList(csvController.grid, s);


                string id = AssayTrainingTable[0, s];



                double[] x = new double[] { };



                double[,] simplexArray = new double[elementList.Count + 1, minList.Count];

                double[] c = new double[simplexArray.GetUpperBound(1) + 1];

                Debug.Log("don't forget to set the correct bothcontroller.elementDict");

                for (int i = 0; i <= simplexArray.GetUpperBound(1); i++)
                {

                    double other = 100;

                    for (int j = 0; j <= simplexArray.GetUpperBound(0) - 1; j++)//i <= simplexArray.GetUpperBound(0); i++)
                    {
                        BothController.MineralComposition MC;
                        double val = 0;
                        if (bothController.currentMineralDict.TryGetValue(minList[i], out MC))
                        {
                            MC.elementDictionary.TryGetValue(elementList[j], out val);
                            simplexArray[j, i] = val;
                        }
                        else if (bothController.elementDict.TryGetValue(minList[i], out MC))
                        {
                            MC.elementDictionary.TryGetValue(elementList[j], out val);
                            simplexArray[j, i] = val;
                        }
                        else
                            Debug.Log("failed");
                        other -= val;
                    }

                    simplexArray[simplexArray.GetUpperBound(0), i] = other;

                    c[i] = -weightList[i];
                }

                /*double[,] simplexArray = new double[,]
                                                    {
                                                    { 0, -10.6, -9.69, -16.95, -20.3, -17.82, -20.9, -100, 0, 0, 0},
                                                    { 0, 0, 0, -25.18, 0, -17.64, 0, 0, -100, 0, 0},
                                                    { 0, 0, -14.05, 0, -9.8, 0, 0, 0, 0, -100, 0},
                                                    { 0, -8.5, 0, 0, 0, 0, 0, 0, 0, 0, -100},
                                                    { -100, -80.6, -76.26, -57.87, -69.8, -64.54, -79.1, 0, 0, 0, 0}
                                                    };*/

                //list of elements in the assayTable grid columns?
                double[] cn = new double[columnIndexList.Count + 1];//csvController.chemTest.ToArray();
                double cnOther = 10000;
                for (int i = 0; i < columnIndexList.Count; i++)
                {
                    double.TryParse(csvController.grid[columnIndexList[i], s], out cn[i]);
                    cn[i] *= 100;
                    cnOther -= cn[i];
                }
                cn[cn.Length - 1] = cnOther;




                if (!toCSV)
                {
                    string temp = "Sample Composition: ";
                    foreach (double d in cn)
                        temp += d + ", ";
                    export.WriteStringToStringBuilder("");
                    export.WriteStringToStringBuilder(temp);

                    temp = "Weights: ";
                    foreach (double d in c)
                    {
                        double dFixed = d * -1;
                        temp += dFixed + ", ";
                    }
                    export.WriteStringToStringBuilder(temp);
                }


                alglib.minlpstate state;
                alglib.minlpreport rep;
                alglib.minlpcreate(simplexArray.GetUpperBound(1) + 1, out state);
                alglib.minlpsetcost(state, c);
                alglib.minlpsetbcall(state, 0, 100);
                //alglib.minlpsetscale(state, scale);
                //alglib.minlpsetcost(state, bndl); //replace bndl with something
                //alglib.minlpsetbc(state, bndl, bndu);
                /*alglib.minlpsetlc1(state, true); //a, ct is constraint types so >0 is >= =0 is = and <0 is <=, k*/
                /*
        CT      -   constraint types, array[K]:
                    * if CT[i]>0, then I-th constraint is A[i,*]*x >= A[i,n]
                    * if CT[i]=0, then I-th constraint is A[i,*]*x  = A[i,n]
                    * if CT[i]<0, then I-th constraint is A[i,*]*x <= A[i,n]*/
                alglib.minlpsetlc2(state, simplexArray, cn, cn); //al, au
                                                                 //int[] ct = { };
                                                                 //alglib.minlpsetlc1(state, simplexArrayLC1, ct);
                alglib.minlpoptimize(state);
                alglib.minlpresults(state, out x, out rep);

                if (!toCSV)
                {
                    export.WriteStringToStringBuilder("");
                    export.WriteStringToStringBuilder("");
                    switch (rep.terminationtype)
                    {
                        case -4:
                            export.WriteStringToStringBuilder("LP problem is primal unbounded(dual infeasible)");
                            Debug.Log("LP problem is primal unbounded(dual infeasible)");
                            break;
                        case -3:
                            export.WriteStringToStringBuilder("LP problem is primal infeasible(dual unbounded)");
                            Debug.Log("LP problem is primal infeasible(dual unbounded)");
                            break;
                        case 1:
                            export.WriteStringToStringBuilder("successful completion");
                            Debug.Log("successful completion 1");
                            break;
                        case 2:
                            export.WriteStringToStringBuilder("successful completion");
                            Debug.Log("successful completion 2");
                            break;
                        case 3:
                            export.WriteStringToStringBuilder("successful completion");
                            Debug.Log("successful completion 3");
                            break;
                        case 4:
                            export.WriteStringToStringBuilder("successful completion");
                            Debug.Log("successful completion 4");
                            break;
                        case 5:
                            export.WriteStringToStringBuilder("Max Iterations was reached");
                            Debug.Log("MaxIts steps was taken");
                            break;
                        case 7:
                            export.WriteStringToStringBuilder("stopping conditions are too stringent, further improvement is impossible, X contains best point found so far.");
                            Debug.Log("stopping conditions are too stringent, further improvement is impossible, X contains best point found so far.");
                            break;
                    }
                    export.WriteStringToStringBuilder("");
                    export.WriteStringToStringBuilder("Score: " + ((rep.f / 100) * -1));
                    export.WriteStringToStringBuilder("Values: " + alglib.ap.format(x, 2));

                    string dualLine = "Dual Variables: ";

                    foreach (double d in rep.y)
                    {
                        dualLine += d;
                        dualLine += ", ";
                    }

                    export.WriteStringToStringBuilder(dualLine);


                    export.WriteStringToStringBuilder("primal feasibility error: " + rep.primalerror);
                    export.WriteStringToStringBuilder("iteration count: " + rep.iterationscount);

                    string statsLine = "stats: ";

                    foreach (int i in rep.stats)
                    {
                        statsLine += i;
                        statsLine += ", ";
                    }

                    ///Debug.Log("array[N+M], statuses of box (N) and linear (M) constraints: " + statsLine);
                    export.WriteStringToStringBuilder("array[N+M], statuses of box (N) and linear (M) constraints: " + statsLine);
                    export.WriteStringToStringBuilder("");
                    export.WriteStringToStringBuilder("---------------------------------");
                    export.WriteStringToStringBuilder("");
                }

                //Debug.Log("target function value: " + rep.f);
                //Debug.Log("Values: " + alglib.ap.format(x, 2));





                string exportResult = alglib.ap.format(x, 2);
                //Debug.Log("exportResult: " + exportResult);

                string truncatedExportResult = exportResult.Substring(1, exportResult.Length - 2);

                double[] resultArray = Array.ConvertAll(truncatedExportResult.Split(','), new Converter<string, double>(Double.Parse));

                int d2 = simplexArray.GetUpperBound(1) + 1;
                const int doubleSize = 8;
                double[] target = new double[d2];

                /*Buffer.BlockCopy(simplexArray, doubleSize * d2 * 0, target, 0, doubleSize * d2);
                double column1 = SumProduct(target, resultArray);
                Debug.Log("Result1: " + column1 + ", " + cn[0]);
                Buffer.BlockCopy(simplexArray, doubleSize * d2 * 1, target, 0, doubleSize * d2);
                column1 = SumProduct(target, resultArray);
                Debug.Log("Result2: " + column1 + ", " + cn[1]);
                Buffer.BlockCopy(simplexArray, doubleSize * d2 * 2, target, 0, doubleSize * d2);
                column1 = SumProduct(target, resultArray);
                Debug.Log("Result3: " + column1 + ", " + cn[2]);
                Buffer.BlockCopy(simplexArray, doubleSize * d2 * 3, target, 0, doubleSize * d2);
                column1 = SumProduct(target, resultArray);
                Debug.Log("Result4: " + column1 + ", " + cn[3]);
                Buffer.BlockCopy(simplexArray, doubleSize * d2 * 4, target, 0, doubleSize * d2);
                column1 = SumProduct(target, resultArray);
                Debug.Log("Result5: " + column1 + ", " + cn[4]);

                Debug.Log("c length = " + c.Length);
                Debug.Log("resultArray length = " + resultArray.Length);
                Debug.Log("Simp " + simplexArray.GetUpperBound(0) + ", " + simplexArray.GetUpperBound(1));
                Debug.Log("Cn " + cn.Length);*/



                string resultLine = "resultArray: ";

                foreach (double i in resultArray)
                {
                    resultLine += i;
                    resultLine += ", ";
                }
                Debug.Log(resultLine);

                double endProduct = SumProduct(c, resultArray);

                double score = ((rep.f / 100.0) * -1);

                if (toCSV)
                    export.WriteStringToStringBuilder(id + "," + score.ToString() + "," + truncatedExportResult);

                //Debug.Log("<color=green>--------------------------</color>");
                bothController.progressBar.value++;
                bothController.progressText.text = bothController.progressBar.value + "/" + maxSamples;
                yield return null;

                //Debug.Log("'Result': " + result);
            }


        }

        // Create a file to write to.
        using (StreamWriter sw = File.CreateText(export.tempFilename))
        {
            sw.Write(export.sb.ToString());
        }
        bothController.loadingMenu.SetActive(false);
        bothController.sharedMenu.SetActive(true); */
                 yield return null;
    }
    bool ContainsNoCase(string source, string toCheck)
    {
        bool returnable = false;
        if (source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0)
            returnable = true;
        return returnable;
    }
    bool MatchesNoCase(string source, string toCheck)
    {
        bool returnable = false;
        if (System.String.Equals(source, toCheck, System.StringComparison.OrdinalIgnoreCase))
            returnable = true;
        return returnable;
    }

}
