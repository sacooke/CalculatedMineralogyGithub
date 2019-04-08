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
    public string CombiResultsFilename = "null";
    public string AssayTestingFilename = "null";
    public string CombiResultsShortFilename;
    public string AssayTestingShortFilename;
    public Button LoadCSVsButton;
    public Text CombiResultsText;
    public Text AssayTestingText;
    public BothController bothController;
    public Export export;
    public GameObject mainMenu;
    public GameObject trainingMenu;
    public GameObject trainingSubmenu1;
    public GameObject trainingSubmenu2;
    public GameObject trainingSubmenu3;
    public GameObject trainingLoadingMenu;
    public InputField automatchInputField;


    public Dropdown datasetDropdown;

    string path;

    string[,] CombiResultTable;
    double[,] CombiResultTableDoubles;
    string[,] AssayTrainingTable;
    string[,] DatasetTable;

    public GameObject QXRDPrefab;
    public GameObject QXRDScrollview;

    public int currentGroup = -1;
    public int currentGroupIndex = -1;
    public int currentWeight = -1;
    public int currentLoop = -1;
    public int totalLoops = 3;

    string[] groupColumn;
    string[] maxWeightsColumn;
    string[] minWeightsColumn;
    string[] combiSamplesColumn;
    string[] assaySamplesColumn;
    string[] assayElementsRow;
    List<int> combiSampleIndexes;
    List<int> assaySampleIndexes;
    List<int> group1Indexes;
    List<int> group2Indexes;
    List<int> group3Indexes;
    List<int> group4Indexes;
    List<int> group5Indexes;
    List<float> maxWeights;
    List<float> minWeights;
    List<double> weightsArray;
    List<int> combiColumnPositions;
    List<int> QXRDRowPositions; 
    List<int> assayElementIndexList;
    List<string> elementList;
    List<string> minList;
    List<string> assayMinList;
    List<string> combiMinList;
    List<string> sampleList;
    float[] weightRange;
    double bestDifference = Double.PositiveInfinity;
    double[,] resultTable;
    double[,] bestTable;
    double[,] differenceTable;

    List<double> bestWeights = new List<double>();

    // Use this for initialization
    void Start() {
        PopulateDatasetDropdown();
    }

    // Update is called once per frame
    void Update() {

    }

    public void ResetAllValues()
    {
        foreach(Transform t in QXRDScrollview.transform)
        {
            Destroy(t.gameObject);
        }
        CombiResultsText.text = "<None>";
        AssayTestingText.text = "<None>";
        bothController.datasetDict = new Dictionary<string, BothController.Dataset>();
        CombiResultsFilename = "null";
        AssayTestingFilename = "null";
        LoadCSVsButton.interactable = false;
    }

    public void GoBack()
    {
        if (trainingSubmenu2.activeSelf == true)
        {
            trainingSubmenu2.SetActive(false);
            trainingSubmenu1.SetActive(true);
            ResetAllValues();
        }
        else if (trainingSubmenu3.activeSelf == true)
        {
            trainingSubmenu3.SetActive(false);
            trainingSubmenu2.SetActive(true);
        }
    }

    bool StringIsNull(string s)
    {
        bool result = false;
        if (s == "null" || s == "" || s == null)
            result = true;
        return result;
    }



    public void LoadCombiResultsCSV()
    {

        fileChooser.setup(FileChooser.OPENSAVE.OPEN, "csv");
        fileChooser.callbackYes = delegate (string filename) {
            fileChooser.gameObject.SetActive(false);
            if (File.Exists(filename))
            {
                CombiResultsFilename = filename;
                CombiResultsShortFilename = Path.GetFileNameWithoutExtension(filename);
                CombiResultsText.text = CombiResultsShortFilename;

                //ExtractAndLoadCSV(filename, isAssay);
                if (!StringIsNull(AssayTestingFilename))
                {
                    Debug.Log("AssayTestingFilename = " + AssayTestingFilename);
                    LoadCSVsButton.interactable = true;
                }

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
                AssayTestingShortFilename = Path.GetFileNameWithoutExtension(filename);
                AssayTestingText.text = AssayTestingShortFilename;
                //ExtractAndLoadCSV(filename, isAssay);
                if (!StringIsNull(CombiResultsFilename))
                {
                    Debug.Log("CombiResultsFilename = " + CombiResultsFilename);
                    LoadCSVsButton.interactable = true;
                }

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
        trainingLoadingMenu.SetActive(true);
        trainingSubmenu1.SetActive(false);
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
            Debug.Log("<color=red>Object reference not set to an instance of an object when using default</color>");
            slice = new string[grid.GetUpperBound(1) + 1];/////
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
        CombiResultTableDoubles = CSVReader.SplitCsvGridJustDoubles(fileContentString);

        //Parse dataset
        if (datasetDropdown.value != 0)//if option != 0
        {
            yield return StartCoroutine(FastDownload(path + datasetDropdown.options[datasetDropdown.value].text, fileContents => fileContentString = fileContents));
            DatasetTable = CSVReader.SplitCsvGrid(fileContentString);


            Dictionary<string, BothController.MineralComposition> mineralDict = new Dictionary<string, BothController.MineralComposition>();

            for (int i = 1; i <= DatasetTable.GetUpperBound(1); i++)
            {

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

            BothController.Dataset ds = new BothController.Dataset(mineralDict);
            bothController.datasetDict.Add(datasetDropdown.options[datasetDropdown.value].text, ds);
            bothController.currentMineralDict = ds.mineralDict;
        }
        else
        {

            bothController.datasetDict.Add("Default", bothController.defaultDataset);
            Dictionary<string, BothController.MineralComposition> defaultMineralDict = bothController.defaultDataset.mineralDict;
            bothController.currentMineralDict = defaultMineralDict;


        }




        FillQXRDList();

        trainingSubmenu2.SetActive(true);
        trainingLoadingMenu.SetActive(false);

        yield return null;
    }

    public void DoValidate()
    {

        Debug.Log("<color=red>start of thing==========================</color>");
        for(int i = 0; i < QXRDScrollview.transform.childCount; i++)
        {
            QXRDListEntry QLE = QXRDScrollview.transform.GetChild(i).GetComponent<QXRDListEntry>();
            if (QLE.index == i)
            {
                QLE.index++;
            }
        }//for some reason the list changing the top value to 0 despite me explicitly setting it to 1

        /*
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
        */
        TrainAssay();


    }

    public void FillQXRDList()
    {
        QXRDListEntry QLE;
        List<string> mineralList = new List<string>();

        Debug.Log("getslice1");
        foreach (string column in GetSlice(CombiResultTable, 0, false))
        {
            if (column != "" && column != "Residual SSQ" && column != "degreesFreedom" && column != "probability" && column != "Sample no")
                mineralList.Add(column);
        }
        int i = 0;
        Debug.Log("getslice2");
        foreach (string row in GetSlice(DatasetTable, 0, true))
        {
            if (i == 0)
            {
                i++;
                continue;
            }
            GameObject g = GameObject.Instantiate(QXRDPrefab) as GameObject;
            g.transform.SetParent(QXRDScrollview.transform, false);
            QLE = g.GetComponent<QXRDListEntry>();
            QLE.index = i;
            QLE.SetLabel(row);
            QLE.AddToDropdown(mineralList);
            int optionVal = 0;
            int len = 3;
            foreach (Dropdown.OptionData option in QLE.dropdown.options)
            {
                if (option.text.Length >= len && QLE.MineralComp.Length >= len)
                {
                    if (ContainsNoCase(option.text, QLE.MineralComp.Substring(0, len)))
                    {
                        //Debug.Log(option.text + " has first " + len + " letters of " + QLE.MineralComp.Substring(0, len));
                        QLE.dropdown.value = optionVal;
                    }
                }
                optionVal++;
            }
            i++;

        }
    }

    public void FindBestQXRDMatch()
    {

        int length;
        if (!int.TryParse(automatchInputField.text, out length))
            length = 0;
        QXRDListEntry QLE;
        if (length > 0)
        {
            foreach (Transform child in QXRDScrollview.transform)
            {
                QLE = child.gameObject.GetComponent<QXRDListEntry>();
                int optionVal = 0;
                foreach (Dropdown.OptionData option in QLE.dropdown.options)
                {
                    if (option.text.Length >= length && QLE.MineralComp.Length >= length)
                    {
                        if (ContainsNoCase(option.text, QLE.MineralComp.Substring(0, length)))
                        {
                            //Debug.Log(option.text + " has first " + len + " letters of " + QLE.MineralComp.Substring(0, len));
                            QLE.dropdown.value = optionVal;
                        }
                    }
                    optionVal++;
                }
            }
        }
    }

    public void TrainAssay()
    {
        bothController.loadingMenu.SetActive(true);
        trainingSubmenu2.SetActive(false);

        Debug.Log("getslice3");
        groupColumn = GetSlice(DatasetTable, "group", true);
        Debug.Log("getslice4");
        maxWeightsColumn = GetSlice(DatasetTable, "max", true);
        Debug.Log("getslice5");
        minWeightsColumn = GetSlice(DatasetTable, "min", true);
        combiSamplesColumn = GetSlice(CombiResultTable, 0, true);
        assaySamplesColumn = GetSlice(AssayTrainingTable, 0, true);
        assayElementsRow = GetSlice(AssayTrainingTable, 0, false);
        group1Indexes = new List<int>();
        group2Indexes = new List<int>();
        group3Indexes = new List<int>();
        group4Indexes = new List<int>();
        group5Indexes = new List<int>();
        maxWeights = new List<float>();
        minWeights = new List<float>();
        minList = new List<string>();
        assayMinList = new List<string>();
        combiMinList = new List<string>();
        sampleList = new List<string>();
        elementList = new List<string>();
        combiColumnPositions = new List<int>();
        QXRDRowPositions = new List<int>();
        assayElementIndexList = new List<int>();
        assaySampleIndexes = new List<int>();
        combiSampleIndexes = new List<int>();

        //for each of the items in the QXRD list that arent paired with a None, cross check their index with the groupcolumn
        //if it's 0, nothing happens
        //if it's greater than zero,
        //      
        int i = 0;
        int row = 1;///but should be 1?=====================================================================================
        //Debug.Log("maxWeightsColumn length = " + maxWeightsColumn.Length);
        foreach (Transform QXRD in QXRDScrollview.transform)
        {
            QXRDListEntry QLE = QXRD.GetComponent<QXRDListEntry>();
            //if the paired mineral is not 'None'
            if (QLE.dropdown.value != 0)
            {
                //Debug.Log("row = " + row + " " + maxWeightsColumn[row]);
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

                //add in the mineral compositions
                //these should be the ones that are the qxrd list names, not the dropdowns
                string minString = QLE.MineralComp;
                minList.Add(minString);
                assayMinList.Add(minString);
                combiMinList.Add(QLE.dropdown.options[QLE.dropdown.value].text);
                //Debug.Log("adding " + minString + " to list");
                combiColumnPositions.Add(QLE.dropdown.value);
                QXRDRowPositions.Add(QLE.index);

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

        
        
        foreach (BothController.MineralComposition mc in bothController.elementDict.Values)
        {
            minList.Add(mc.mineral);
            bestWeights.Add(-100);
        }
        //probably include all elements?
        /*
        foreach (Toggle tog in bothController.elementCompScrollView.transform.GetComponentsInChildren<Toggle>())
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
        //Debug.Log("assayElementsRow.Length = " + assayElementsRow.Length);
        for (int e = 1; e < assayElementsRow.Length; e++)
        {
            //Debug.Log("assayElementsRow[" + e + "] = " + assayElementsRow[e]);
            if(ContainsNoCase(assayElementsRow[e],"_pc"))
            {
                string elem = assayElementsRow[e];
                assayElementIndexList.Add(e);
                //elementList.Add(string sans _pc)================================================================================================================
                elementList.Add(elem.Substring(0, elem.IndexOf('_')));
            }
        }

        for (int a = 1; a < assaySamplesColumn.Length; a++)
        {
            if(combiSamplesColumn.Any(assaySamplesColumn[a].Contains))
            {
                for(int c = 0; c < combiSamplesColumn.Length; c++)
                {
                    if(ContainsNoCase(combiSamplesColumn[c], assaySamplesColumn[a]))
                    {
                        assaySampleIndexes.Add(a);
                        combiSampleIndexes.Add(c);
                        sampleList.Add(assaySamplesColumn[a]);
                        break;
                    }
                }
            }
        }

        //prepare for progress bar
        int maxSamples = 0;

        int numberOfValidMinerals = group1Indexes.Count + group2Indexes.Count + group3Indexes.Count + group4Indexes.Count + group5Indexes.Count;
        int numberOfSamples = assaySampleIndexes.Count;
        int totalNumber = numberOfSamples * numberOfValidMinerals * 18;

        bothController.progressBar.value = 0;
        bothController.progressBar.maxValue = totalNumber;


        //group we're up to (1-5), weight index we're up to (0-5), 
        currentGroup = 0;
        currentGroupIndex = 0;
        currentWeight = 0;
        currentLoop = 0;
        //Group 1========================================================================================================================
        float min = minWeights[group1Indexes[0]];
        float max = maxWeights[group1Indexes[0]];

        weightRange = new float[]{ 0f, min, (0.75f * min) + (0.25f * max), (min + max) / 2, (0.25f * min) + (0.75f * max), max };
        weightsArray = new List<double>(bestWeights);
        weightsArray[group1Indexes[0]] = weightRange[0];
        StartCoroutine("EnumerateAssayWeightIterator");
    }

    public void IterateTrainingLoop()
    {
        List<List<int>> groupLists = new List<List<int>>();
        groupLists.Add(group1Indexes);
        groupLists.Add(group2Indexes);
        groupLists.Add(group3Indexes);
        groupLists.Add(group4Indexes);
        groupLists.Add(group5Indexes);

        //compare entire table
        double diff = GetAbsoluteTableDifference(resultTable, CombiResultTableDoubles);
        if (diff < bestDifference)
        {
            Debug.Log("new bestDifference found(" + diff + " < " + bestDifference + ")");
            bestDifference = diff;
            bestWeights = new List<double>(weightsArray);
            bestTable = resultTable;
            string allBestWeights = "";
            int j = 0;
            foreach(double weight in bestWeights)
            {
                if(j == groupLists[currentGroup][currentGroupIndex])
                    allBestWeights += "{{{" + weight.ToString("F1") + "}}}, ";
                else
                    allBestWeights += weight.ToString("F1") + ", ";
                j++;
            }
            Debug.Log("Weights = " + allBestWeights);
        }



        currentWeight++;
        if(currentWeight > 5)
        {
            currentWeight = 0;
            currentGroupIndex++;
            if (currentGroupIndex >= groupLists[currentGroup].Count)
            {
                //Debug.Log("currentGroupIndex " + currentGroupIndex + " is >= " + currentGroup + "'s " + groupLists[currentGroup].Count);
                currentGroupIndex = 0;
                currentGroup++;
                if(currentGroup >= 5)
                {
                    currentGroup = 0;
                    currentLoop++;
                }
                //this check exists in case the current group has no associated mineral compositions
                while(groupLists[currentGroup].Count == 0 && currentLoop < 3)//this one goofed?
                {
                    currentGroup++;
                    if (currentGroup >= 5)
                    {
                        currentGroup = 0;
                        currentLoop++;
                    }
                }
            }
            //Debug.Log("groupLists legnth = " + groupLists.Count);
            //Debug.Log("groupLists[" + currentGroup + "].Length = " + groupLists[currentGroup].Count);
            //Debug.Log("minWEights length = " + minWeights.Count);
            //Debug.Log("currentGroupIndex = " + currentGroupIndex + ", currentGroup = " + currentGroup);
            float min = minWeights[groupLists[currentGroup][currentGroupIndex]];
            float max = maxWeights[groupLists[currentGroup][currentGroupIndex]];

            weightRange = new float[] { 0f, min, (0.75f * min) + (0.25f * max), (min + max) / 2, (0.25f * min) + (0.75f * max), max };
        }
        if (currentLoop < 3)
        {
            float replacementWeight = weightRange[currentWeight];
            //Debug.Log("loading weightRange[" + currentWeight + "] = " + weightRange[currentWeight]);
            weightsArray = new List<double>(bestWeights);
            weightsArray[groupLists[currentGroup][currentGroupIndex]] = replacementWeight;
            StartCoroutine("EnumerateAssayWeightIterator");
        }
        else
        {
            //end it
            Debug.Log("best weights are found");
            CSVReader.DebugOutputGrid(bestTable);
            string bestWeightString = "";
            foreach (double weight in bestWeights)
            {
                bestWeightString += weight.ToString("F3") + ", ";
            }
            Debug.Log(bestWeightString);
            bothController.loadingMenu.SetActive(false);
            trainingSubmenu3.SetActive(true);

            /*
            */
        }
    }

    public void ExportTrainingData(string filename, int option)
    {
        Debug.Log("here we at, filename = " + filename + ", option = " + option);
        //0 = just weights, 1 = just assay, 2 = assay and combi

        export.StartNewStringbuilder();
        

        string assayLineString = "";
        string combiLineString = "";
        string differenceLineString = "";
        string minString;
        string combiMinString;

        switch (option)
        {
            case 0:

                minString = AssayTrainingTable[0, 0] + ",";
                foreach (string min in assayMinList)
                    minString += min + ",";
                export.WriteStringToStringBuilder(minString);
                string weightString = "";
                for (int w = 0; w < assayMinList.Count; w++)
                    weightString += bestWeights[w].ToString() + ",";

                export.WriteStringToStringBuilder("Best Weights," + weightString);
                export.WriteToFile(filename);
                break;
            case 1:
                minString = AssayTrainingTable[0,0] + ",";
                foreach (string min in minList)
                    minString += min + ",";

                export.WriteStringToStringBuilder(minString);
                for (int s = 0; s <= bestTable.GetUpperBound(1); s++)
                {
                    assayLineString = "";
                    for (int c = 0; c <= bestTable.GetUpperBound(0); c++)
                    {
                        double assayTableDouble = bestTable[c, s];
                        assayLineString += assayTableDouble.ToString() + ",";
                    }
                    export.WriteStringToStringBuilder(sampleList[s] + "," + assayLineString);

                }
                export.WriteToFile(filename);
                break;
            case 2:
                minString = AssayTestingShortFilename + ",";
                //combiMinString = CombiResultsShortFilename + ",";
                foreach (string min in assayMinList)
                    minString += min + ",";
                //foreach (string min in combiMinList)
                //    combiMinString += min + ",";

                differenceTable = new double[minList.Count, assaySampleIndexes.Count];//assayElementIndexList.Count

                export.WriteStringToStringBuilder(minString);
                for (int s = 0; s < combiSampleIndexes.Count; s++)
                {
                    assayLineString = "";
                    int ASI = assaySampleIndexes[s];
                    for (int c = 0; c < combiColumnPositions.Count; c++)
                    {
                        int QRP = QXRDRowPositions[c];
                        double assayTableDouble = bestTable[QRP - 1, ASI - 1];
                        differenceTable[c, s] = assayTableDouble;
                        assayLineString += assayTableDouble.ToString() + ",";
                    }
                    export.WriteStringToStringBuilder(sampleList[s] + "," + assayLineString);

                }

                export.WriteStringToStringBuilder(CombiResultsShortFilename);

                for (int s = 0; s < combiSampleIndexes.Count; s++)
                {
                    combiLineString = "";
                    int CSI = combiSampleIndexes[s];
                    for (int c = 0; c < combiColumnPositions.Count; c++)
                    {
                        int CCP = combiColumnPositions[c];
                        double combiTableDouble = CombiResultTableDoubles[CCP - 1, CSI - 1];
                        differenceTable[c, s] = Math.Abs(differenceTable[c, s] - combiTableDouble);
                        combiLineString += combiTableDouble.ToString() + ",";
                    }
                    export.WriteStringToStringBuilder(sampleList[s] + "," + combiLineString);

                }

                export.WriteStringToStringBuilder("Difference");

                for (int s = 0; s < combiSampleIndexes.Count; s++)
                {
                    differenceLineString = "";
                    for (int c = 0; c < combiColumnPositions.Count; c++)
                    {
                        differenceLineString += differenceTable[c,s].ToString() + ",";
                    }
                    export.WriteStringToStringBuilder(sampleList[s] + "," + differenceLineString);

                }
                export.WriteToFile(filename);
                break;
            default:
                break;
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


    public IEnumerator EnumerateAssayWeightIterator()
    {
        //string fullExportString = "";
        //string columnNames = "Sample No.,Score";

        List<double> weightList = new List<double>();
        weightList = weightsArray;
        /*string weightsString = "";
        foreach (double weight in weightList)
        {
            weightsString += weight.ToString("F1") + ", ";
        }
        Debug.Log("Weights = " + weightsString);*/


        double result;

        resultTable = new double[minList.Count, assaySampleIndexes.Count];//assayElementIndexList.Count
        //Debug.Log("resultTable dimensions = " + minList.Count + ", " + assaySampleIndexes.Count);

        int sampleIndex = 0;

        foreach (int s in assaySampleIndexes)//==========================================================================
        {

            //'chemtest' list is a list of all element values in the given row
            //csvController.GetChemTestList(csvController.grid, s);


            string id = AssayTrainingTable[0, s];



            double[] x = new double[] { };



            double[,] simplexArray = new double[elementList.Count + 1, minList.Count];

            double[] c = new double[simplexArray.GetUpperBound(1) + 1];

            //Debug.Log("don't forget to set the correct bothcontroller.elementDict");

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
            double[] cn = new double[assayElementIndexList.Count + 1];//csvController.chemTest.ToArray();
            double cnOther = 10000;
            for (int i = 0; i < assayElementIndexList.Count; i++)
            {
                double.TryParse(AssayTrainingTable[assayElementIndexList[i], s], out cn[i]);
                cn[i] *= 100;
                cnOther -= cn[i];
            }
            cn[cn.Length - 1] = cnOther;

            //Debug.Log("Simplex array:");
            //CSVReader.DebugOutputGrid(simplexArray);

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
            bool debug = false;
            if (debug)
            {
                switch (rep.terminationtype)
                {
                    case -4:
                        Debug.Log("LP problem is primal unbounded(dual infeasible)");
                        break;
                    case -3:
                        Debug.Log("LP problem is primal infeasible(dual unbounded)");
                        break;
                    case 1:
                        Debug.Log("successful completion 1");
                        break;
                    case 2:
                        Debug.Log("successful completion 2");
                        break;
                    case 3:
                        Debug.Log("successful completion 3");
                        break;
                    case 4:
                        Debug.Log("successful completion 4");
                        break;
                    case 5:
                        Debug.Log("MaxIts steps was taken");
                        break;
                    case 7:
                        Debug.Log("stopping conditions are too stringent, further improvement is impossible, X contains best point found so far.");
                        break;
                }
                /*export.WriteStringToStringBuilder("");
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
                export.WriteStringToStringBuilder("");*/
            }

            
            //Debug.Log("target function value: " + rep.f);
            //Debug.Log("Values: " + alglib.ap.format(x, 2));
            




            string exportResult = alglib.ap.format(x, 2);
            //Debug.Log("exportResult: " + exportResult);

            string truncatedExportResult = exportResult.Substring(1, exportResult.Length - 2);

            double[] resultArray = Array.ConvertAll(truncatedExportResult.Split(','), new Converter<string, double>(Double.Parse));
            /*string resultString = "";
            foreach (double d in resultArray)
                resultString += d.ToString("F3") + ",";
            Debug.Log("results: " + resultString);*/
            /*
            int d2 = simplexArray.GetUpperBound(1) + 1;
            const int doubleSize = 8;
            double[] target = new double[d2];

            
            Buffer.BlockCopy(simplexArray, doubleSize * d2 * 0, target, 0, doubleSize * d2);
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

            //Debug.Log("adding a = ");
            for (int i = 0; i <= resultTable.GetUpperBound(0); i++)
            {
                if(i == resultTable.GetUpperBound(0) && resultArray[resultTable.GetUpperBound(0)] != 0)
                    Debug.Log("value that was missed was " + resultArray[resultTable.GetUpperBound(0)]);
                resultTable[i, sampleIndex] = resultArray[i];
            }
            sampleIndex++;

            
            /*string resultLine = "resultArray: ";

            foreach (double i in resultArray)
            {
                resultLine += i;
                resultLine += ", ";
            }
            Debug.Log(resultLine);*/

            double endProduct = SumProduct(c, resultArray);

            double score = ((rep.f / 100.0) * -1);


            //Debug.Log("<color=green>--------------------------</color>");
            bothController.progressBar.value++;
            bothController.progressText.text = bothController.progressBar.value + "/" + bothController.progressBar.maxValue;
            yield return null;

            //Debug.Log("'Result': " + result);



        }

        //CSVReader.DebugOutputGrid(resultTable);
        /*string bestWeightString = "";
        foreach (double weight in bestWeights)
        {
            bestWeightString += weight.ToString("F3") + ", ";
        }
        Debug.Log(bestWeightString);*/
        IterateTrainingLoop();
        yield return null;
    }

    public double GetAbsoluteTableDifference(double[,] assayTable, double[,] combiTable)
    {
        double absoluteDifference = Double.PositiveInfinity;
        double theDifference = 0;
        bool wasTested = false;

        for (int s = 0; s < combiSampleIndexes.Count; s++)
        {
            int ASI = assaySampleIndexes[s];
            int CSI = combiSampleIndexes[s];
            for(int c = 0; c < combiColumnPositions.Count; c++)
            {
                wasTested = true;
                int QRP = QXRDRowPositions[c];
                int CCP = combiColumnPositions[c];
                double assayTableDouble = assayTable[QRP - 1, ASI - 1];
                double combiTableDouble = combiTable[CCP - 1, CSI - 1];
                //==========================================================================================================================================

                theDifference += Math.Abs(assayTableDouble - combiTableDouble);
            }
        }
        if (wasTested)
            absoluteDifference = theDifference;
        //Debug.Log("absDiff = " + absoluteDifference);
        return absoluteDifference;
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

}
