using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

public class BothController : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject sharedMenu;
    public GameObject assayMenu;
    public GameObject combiMenu;
    public GameObject loadingMenu;
    public GameObject errorMenu;
    public GameObject QXRDMenu;
    public CSVReader csvController;

    public AssayController assayController;
    public CombiController combiController;

    public FileChooser fileChooser;
    private string fileContentString;
    public alglib alg;
    public Export export;

    public GameObject samplePrefab;
    public GameObject combiElemMinPrefab;
    public GameObject sampleScrollView;
    public GameObject columnScrollView;
    public GameObject mineralCompScrollView;
    public GameObject elementCompScrollView;
    public GameObject combiColumnScrollView;
    public Dropdown datasetDropdown;

    public GameObject mineralCompListPrefab;

    public List<string> samples = new List<string>();
    public List<string> columns = new List<string>();
    public List<Toggle> sampleToggles = new List<Toggle>();
    public List<Toggle> columnToggles = new List<Toggle>();
    public List<Toggle> mineralToggles = new List<Toggle>();
    public List<Toggle> elementToggles = new List<Toggle>();

    public InputField setAllMineralWeightsInputField;
    public InputField setAllElementWeightsInputField;

    public Slider progressBar;
    public Text progressText;

    string path;

    // Use this for initialization
    void Start()
    {
        datasetDropdown.onValueChanged.AddListener(delegate { ChangeDataset(); });

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
    }
    
    // Update is called once per frame
    void Update () {
	
	}

    public void Initialise(bool isAssay)
    {
        mainMenu.SetActive(false);
        sharedMenu.SetActive(true);
        assayMenu.SetActive(isAssay);
        combiMenu.SetActive(!isAssay);
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
        if (isAssay)
        {
            for (int i = 1; i < csvController.grid.GetUpperBound(0); i++)
            {
                string columnHeader = csvController.grid[i, 0];

                if (columnHeader != "" && columnHeader != null)
                {
                    columns.Add(columnHeader);

                    GameObject g = GameObject.Instantiate(samplePrefab) as GameObject;
                    g.transform.SetParent(columnScrollView.transform, false);
                    g.GetComponentInChildren<Text>().text = columnHeader;
                    g.GetComponent<SampleListEntry>().SampleID = columnHeader;
                    g.GetComponent<SampleListEntry>().index = i;
                    if (!ContainsNoCase(columnHeader, "_pc"))
                        g.GetComponent<Toggle>().isOn = false;
                    columnToggles.Add(g.GetComponent<Toggle>());

                }
                //int samplePos = csvController.samplePositions[i];

                //sampleToggles.Add(g.GetComponent<Toggle>());
            }
        }
        else
        {
            for (int i = 1; i < csvController.grid.GetUpperBound(0); i++)
            {
                string columnHeader = csvController.grid[i, 0];

                if (columnHeader != "" && columnHeader != null)
                {
                    columns.Add(columnHeader);

                    GameObject g = GameObject.Instantiate(combiElemMinPrefab) as GameObject;
                    g.transform.SetParent(combiColumnScrollView.transform, false);
                    g.GetComponentInChildren<Text>().text = columnHeader;
                    g.GetComponent<CombiMineralElementListEntry>().SampleID = columnHeader;
                    g.GetComponent<CombiMineralElementListEntry>().label.text = columnHeader;
                    g.GetComponent<CombiMineralElementListEntry>().index = i;
                    if (ContainsNoCase(columnHeader, "_ppm"))
                        g.GetComponent<CombiMineralElementListEntry>().ElementToggle.isOn = true;
                    else if (ContainsNoCase(columnHeader, "_wt%"))
                        g.GetComponent<CombiMineralElementListEntry>().MineralToggle.isOn = true;
                    //g.GetComponent<CombiMineralElementListEntry>().ElementToggle.
                    //columnToggles.Add(g.GetComponent<Toggle>());

                }
                //int samplePos = csvController.samplePositions[i];

                //sampleToggles.Add(g.GetComponent<Toggle>());


            //NEXT TIME:
            //Add the mineral comps to the- oh shit i need to make the CMC a real thing huh, okay brb gotta do that
            //then add them to the CombiMineralCompScrollview which definitely already exists in code
            }
        }


        if(isAssay)
        {
            int i = 0;
            foreach (AssayController.AssayMineralComposition amc in assayController.assayMineralDict.Values)
            {

                GameObject g = GameObject.Instantiate(mineralCompListPrefab) as GameObject;
                g.transform.SetParent(mineralCompScrollView.transform, false);
                MineralCompositionListEntry mcle = g.GetComponent<MineralCompositionListEntry>();
                mcle.label.text = amc.mineral;
                mcle.MineralComp = amc.mineral;
                mcle.index = i;
                g.GetComponentInChildren<InputField>().text = amc.weight.ToString();
                mineralToggles.Add(g.GetComponent<Toggle>());
                i++;
            }
            foreach (AssayController.AssayMineralComposition amc in assayController.assayElementDict.Values)
            {

                GameObject g = GameObject.Instantiate(mineralCompListPrefab) as GameObject;
                g.transform.SetParent(elementCompScrollView.transform, false);
                MineralCompositionListEntry mcle = g.GetComponent<MineralCompositionListEntry>();
                mcle.label.text = amc.mineral;
                mcle.MineralComp = amc.mineral;
                mcle.index = i;
                g.GetComponent<Toggle>().isOn = false;
                g.GetComponentInChildren<InputField>().text = amc.weight.ToString();
                elementToggles.Add(g.GetComponent<Toggle>());
                i++;
            }
        }
        else
        {
            int i = 0;
            foreach (CombiController.CombiMineralComposition cmc in combiController.combiMineralDict.Values)
            {

                GameObject g = GameObject.Instantiate(mineralCompListPrefab) as GameObject;
                g.transform.SetParent(mineralCompScrollView.transform, false);
                MineralCompositionListEntry mcle = g.GetComponent<MineralCompositionListEntry>();
                mcle.label.text = cmc.mineral;
                mcle.MineralComp = cmc.mineral;
                mcle.index = i;
                g.GetComponentInChildren<InputField>().text = cmc.weight.ToString();
                mineralToggles.Add(g.GetComponent<Toggle>());
                i++;
            }
            foreach (CombiController.CombiMineralComposition cmc in combiController.combiElementDict.Values)
            {

                GameObject g = GameObject.Instantiate(mineralCompListPrefab) as GameObject;
                g.transform.SetParent(elementCompScrollView.transform, false);
                MineralCompositionListEntry mcle = g.GetComponent<MineralCompositionListEntry>();
                mcle.label.text = cmc.mineral;
                mcle.MineralComp = cmc.mineral;
                mcle.index = i;
                g.GetComponent<Toggle>().isOn = false;
                g.GetComponentInChildren<InputField>().text = cmc.weight.ToString();
                elementToggles.Add(g.GetComponent<Toggle>());
                i++;
            }
        }
    }

    public void ChangeDataset()
    {
        Debug.Log("name = " + datasetDropdown.options[datasetDropdown.value].text);
        StartCoroutine(FastDownload(path + datasetDropdown.options[datasetDropdown.value].text, fileContents => fileContentString = fileContents));
        string[,] datasetGrid = CSVReader.SplitCsvGrid(fileContentString);
    }

    public class MineralComposition
    {
        public string mineral { get; set; }
        public double weight { get; set; }
        public double startPoint { get; set; }

        public MineralComposition(string mineral, double weight = 1, double startPoint = 1)
        {
            this.mineral = mineral;
            this.weight = weight;
            this.startPoint = startPoint;
        }

        public Dictionary<string, double> elementDictionary = new Dictionary<string, double>();

        public double GetOtherValue()
        {
            double other = 100.0;
            foreach (double val in elementDictionary.Values)
            {
                other -= val;
            }
            return other;
        }
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

    public void SetAllMineralWeights()
    {
        double result;
        if (double.TryParse(setAllMineralWeightsInputField.text, out result))
        {
            foreach (InputField inpF in mineralCompScrollView.transform.GetComponentsInChildren<InputField>())
            {
                inpF.text = result.ToString();
            }
        }
    }

    public void SetAllElementWeights()
    {
        double result;
        if (double.TryParse(setAllElementWeightsInputField.text, out result))
        {
            foreach (InputField inpF in elementCompScrollView.transform.GetComponentsInChildren<InputField>())
            {
                inpF.text = result.ToString();
            }
        }
    }

    public void ToggleAll(int list)
    {
        switch(list)
        {
            case 0:
                if(sampleToggles[0].isOn)
                {
                    foreach (Toggle t in sampleToggles)
                        t.isOn = false;
                }
                else
                {
                    foreach (Toggle t in sampleToggles)
                        t.isOn = true;
                }
                break;
            case 1:
                if (columnToggles[0].isOn)
                {
                    foreach (Toggle t in columnToggles)
                        t.isOn = false;
                }
                else
                {
                    foreach (Toggle t in columnToggles)
                        t.isOn = true;
                }
                break;
            case 2:
                if (mineralToggles[0].isOn)
                {
                    foreach (Toggle t in mineralToggles)
                        t.isOn = false;
                }
                else
                {
                    foreach (Toggle t in mineralToggles)
                        t.isOn = true;
                }
                break;
            case 3:
                if (elementToggles[0].isOn)
                {
                    foreach (Toggle t in elementToggles)
                        t.isOn = false;
                }
                else
                {
                    foreach (Toggle t in elementToggles)
                        t.isOn = true;
                }
                break;
            default:
                Debug.Log("default executed");
                break;
        }
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
