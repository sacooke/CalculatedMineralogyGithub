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

    public Dictionary<string, Dataset> datasetDict;
    public Dictionary<string, MineralComposition> elementDict;
    public Dictionary<string, MineralComposition> currentMineralDict;
    public Dataset defaultDataset;

    public bool assayMode;

    // Use this for initialization
    void Start()
    {
        datasetDropdown.onValueChanged.AddListener(delegate { StartCoroutine(ChangeDataset()); });

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

        AddAllMCs();

        datasetDict = new Dictionary<string, Dataset>();

        var texture = new Texture2D(1, 1); // creating texture with 1 pixel
        texture.SetPixel(0, 0, Color.blue); // setting to this pixel some color
        texture.Apply(); //applying texture. necessarily
        datasetDropdown.options[0].image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
    }
    
    // Update is called once per frame
    void Update () {
	
	}

    public void Initialise(bool isAssay)
    {
        assayMode = isAssay;
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
                    else if (ContainsNoCase(columnHeader, "_pc"))
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

        //Add all mineral compositions and elements to the corresponding lists
        int k = 0;
        //Dataset dataset = new Dataset(defaultMineralDict);
        
        datasetDict.Add("Default", defaultDataset);


        Dictionary<string, MineralComposition> defaultMineralDict = defaultDataset.mineralDict;

        currentMineralDict = defaultMineralDict;

        foreach (MineralComposition mc in defaultMineralDict.Values)
        {

            GameObject g = GameObject.Instantiate(mineralCompListPrefab) as GameObject;
            g.transform.SetParent(mineralCompScrollView.transform, false);
            MineralCompositionListEntry mcle = g.GetComponent<MineralCompositionListEntry>();
            mcle.label.text = mc.mineral;
            mcle.MineralComp = mc.mineral;
            mcle.index = k;
            if (isAssay)
                g.GetComponentInChildren<InputField>().text = mc.weight.ToString();
            else
                g.GetComponentInChildren<InputField>().text = mc.startPoint.ToString();
            mineralToggles.Add(g.GetComponent<Toggle>());
            k++;
        }

        foreach (MineralComposition mc in elementDict.Values)
        {

            GameObject g = GameObject.Instantiate(mineralCompListPrefab) as GameObject;
            g.transform.SetParent(elementCompScrollView.transform, false);
            MineralCompositionListEntry mcle = g.GetComponent<MineralCompositionListEntry>();
            mcle.label.text = mc.mineral;
            mcle.MineralComp = mc.mineral;
            mcle.index = k;
            g.GetComponent<Toggle>().isOn = false;
            if (isAssay)
                g.GetComponentInChildren<InputField>().text = mc.weight.ToString();
            else
                g.GetComponentInChildren<InputField>().text = mc.startPoint.ToString();
            elementToggles.Add(g.GetComponent<Toggle>());
            k++;
        }
        /*
        if (isAssay)
        {
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
        }*/
    }

    public IEnumerator ChangeDataset()
    {

        Dataset ds;
        if (!datasetDict.TryGetValue(datasetDropdown.options[datasetDropdown.value].text, out ds))
        {
            Debug.Log("dataset not found - creating new one");
            Dictionary<string, MineralComposition> mineralDict = new Dictionary<string, MineralComposition>();
            yield return StartCoroutine(FastDownload(path + datasetDropdown.options[datasetDropdown.value].text, fileContents => fileContentString = fileContents));
            string[,] datasetGrid = CSVReader.SplitCsvGrid(fileContentString);

            for (int i = 1; i <= datasetGrid.GetUpperBound(1); i++)
            {
                string columnHeader = datasetGrid[0, i];

                string mineralComp = datasetGrid[0, i];
                MineralComposition MC = new MineralComposition(mineralComp);
                double[] val = new double[] { Double.Parse(datasetGrid[1, i]),
                                                    Double.Parse(datasetGrid[2, i]),
                                                    Double.Parse(datasetGrid[3, i]),
                                                    Double.Parse(datasetGrid[4, i]),
                                                    Double.Parse(datasetGrid[5, i]),
                                                    Double.Parse(datasetGrid[6, i]),
                                                    Double.Parse(datasetGrid[7, i]),
                                                    Double.Parse(datasetGrid[8, i]),
                                                    Double.Parse(datasetGrid[9, i]),
                                                    Double.Parse(datasetGrid[10, i]),
                                                    Double.Parse(datasetGrid[11, i]),
                                                    Double.Parse(datasetGrid[12, i]),
                                                    Double.Parse(datasetGrid[13, i]),
                                                    Double.Parse(datasetGrid[14, i]),
                                                    Double.Parse(datasetGrid[15, i]),
                                                    Double.Parse(datasetGrid[16, i]),
                                                    Double.Parse(datasetGrid[17, i]),
                                                    Double.Parse(datasetGrid[18, i]),
                                                    Double.Parse(datasetGrid[19, i]),
                                                    Double.Parse(datasetGrid[20, i]),
                                                    Double.Parse(datasetGrid[21, i]) };
                FillMCDatabase(MC, val);
                MC.weight = double.Parse(datasetGrid[22, i]);
                MC.startPoint = double.Parse(datasetGrid[23, i]);

                mineralDict.Add(mineralComp, MC);
            }
            ds = new Dataset(mineralDict);
            datasetDict.Add(datasetDropdown.options[datasetDropdown.value].text, ds);
        }

        currentMineralDict = ds.mineralDict;
        mineralToggles.Clear();
        foreach (Transform child in mineralCompScrollView.transform)
            GameObject.Destroy(child.gameObject);
        int k = 0;

        foreach (MineralComposition mc in currentMineralDict.Values)
        {
            GameObject g = GameObject.Instantiate(mineralCompListPrefab) as GameObject;
            g.transform.SetParent(mineralCompScrollView.transform, false);
            MineralCompositionListEntry mcle = g.GetComponent<MineralCompositionListEntry>();
            mcle.label.text = mc.mineral;
            mcle.MineralComp = mc.mineral;
            mcle.index = k;
            if (assayMode)
                g.GetComponentInChildren<InputField>().text = mc.weight.ToString();
            else
                g.GetComponentInChildren<InputField>().text = mc.startPoint.ToString();
            mineralToggles.Add(g.GetComponent<Toggle>());
            k++;
        }

        foreach (Transform child in elementCompScrollView.transform)
        {
            
            MineralCompositionListEntry mcle = child.gameObject.GetComponent<MineralCompositionListEntry>();
            mcle.index = k;
            k++;
        }
        yield return null;

    }
    public class Dataset
    {
        public Dictionary<string, MineralComposition> mineralDict { get; set; }

        public Dataset(Dictionary<string, MineralComposition> mineralDict)
        {
            this.mineralDict = mineralDict;
        }
    }


    public void AddAllMCs()
    {

        Dictionary<string, MineralComposition> defaultMineralDict = new Dictionary<string, MineralComposition>();
        elementDict = new Dictionary<string, MineralComposition>();

        string mineralComp;
        MineralComposition MC;

        mineralComp = "Other(Quartz)";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = 7;
        MC.startPoint = 10.00;

        mineralComp = "Feld_Alb_Ca-Na";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 10.41, 0, 0, 0, 0.38, 0, 0.05, 0.11, 0.01, 0, 0, 7.8, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = 5;
        MC.startPoint = 10.00;

        mineralComp = "Feld_Alb_Na";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 10.4, 0, 0, 0, 0, 0, 0.04, 0.24, 0.02, 0, 0, 8.21, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = 5;
        MC.startPoint = 10.00;

        mineralComp = "FeldsparK";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 9.817163594, 0, 0, 0, 0.12149786, 0, 0.0979097, 13.33707643, 0, 0, 0, 0.126114876, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = 7;
        MC.startPoint = 10.00;

        mineralComp = "FeldsKBa";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 9.85, 0, 0, 0.25, 0, 0, 0.08, 13.5, 0, 0.03, 0, 0.31, 0, 0, 0, 0, 0.03, 0, 0, 0 });
        MC.weight = 7;
        MC.startPoint = 2.00;

        mineralComp = "Feld_Plag";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 15.3732, 0, 0, 0, 7.8908, 0, 0.199, 0.1423, -0.0012, -0.0065, 0, 3.8507, 0, 0, 0, 0, 0.002, 0, 0, 0 });
        MC.weight = 3;
        MC.startPoint = 10.00;

        mineralComp = "Carb_Calc";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 40.04426534, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = 0.3;
        MC.startPoint = 2.00;

        mineralComp = "Carb_Ank";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 20.0185592, 0, 12.55342226, 0, 6.071985112, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = 5.7;
        MC.startPoint = 2.00;

        mineralComp = "Carb_Dol";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 21.73, 0, 0, 0, 13.18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = 3;
        MC.startPoint = 2.00;

        mineralComp = "Rhodonite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 47.8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = 3;
        MC.startPoint = 1.00;

        mineralComp = "Siderite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 53.4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = 3;
        MC.startPoint = 1.00;

        mineralComp = "Anhydrite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 29.44, 0, 0, 0, 0, 0, 0, 0, 0, 0, 23.55, 0, 0, 0, 0, 0 });
        MC.weight = 1.3;
        MC.startPoint = 1.00;

        mineralComp = "ChloriteFe";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 9.6754, 0, 0, 0, 0.1066, 0, 20.3305, 0.0161, 9.3554, 0.0787, 0, 0.0455, 0, 0, 0, 0, 0.0163, 0, 0, 0 });
        MC.weight = 10;
        MC.startPoint = 5.00;

        mineralComp = "ChloriteMg";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 10.0939, 0, 0, 0, 0.0377, 0, 10.5241, 0.0335, 14.3654, 0.0387, 0, 0.0293, 0, 0, 0, 0, 0.0267, 0, 0, 0 });
        MC.weight = 10;
        MC.startPoint = 5.00;

        mineralComp = "Muscovite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 19.5, 0, 0, 0, 0, 0, 0.08, 8.37, 0.05, 0.093, 0, 0.47, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = 7;
        MC.startPoint = 2.00;

        mineralComp = "Musc_Phengite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 16.46, 0, 0, 0, 0, 0, 3.19, 9.14, 0.81, 0, 0, 0.08, 0, 0, 0, 0, 0.1, 0, 0, 0 });
        MC.weight = 4;
        MC.startPoint = 2.00;

        mineralComp = "BiotMg";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 7.5391, 0, 0, 0, 0.0156, 0, 7.9141, 7.8631, 11.8337, 0.052, 0, 0.0718, 0, 0, 0, 0, 1.0092, 0, 0, 0 });
        MC.weight = 9;
        MC.startPoint = 2.00;

        mineralComp = "BiotFe";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 7.3702, 0, 0, 0, 0.1426, 0, 13.0428, 7.3473, 8.2094, 0.0686, 0, 0.0769, 0, 0, 0, 0, 0.9201, 0, 0, 0 });
        MC.weight = 9;
        MC.startPoint = 2.00;

        mineralComp = "Amph_tscher";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 7.33, 0, 0, 0, 7.7, 0, 13.72, 0.62, 5.76, 0.33, 0, 1.43, 0, 0, 0, 0, 0.74, 0, 0, 0 });
        MC.weight = 5;
        MC.startPoint = 1.00;

        mineralComp = "Amph_Act";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 1.5633, 0, 0, 0, 8.7168, 0, 12.7119, 0.1038, 8.065, 0.0905, 0, 0.204, 0, 0, 0, 0, 0.0574, 0, 0, 0 });
        MC.weight = 8;
        MC.startPoint = 2.00;

        mineralComp = "Amph_HorMg";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 3.42, 0, 0, 0, 8.63, 0, 11.26, 0.43, 8.03, 0.48, 0, 0.78, 0, 0, 0, 0, 0.53, 0, 0, 0 });
        MC.weight = 7.5;
        MC.startPoint = 2.00;

        mineralComp = "Amph_Trem";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0.9295, 0, 0, 0, 9.3243, 0, 6.3783, 0.0668, 11.4574, 0.1231, 0, 0.2079, 0, 0, 0, 0, 0.0767, 0, 0, 0 });
        MC.weight = 6;
        MC.startPoint = 2.00;

        mineralComp = "Epid_LC";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 12.5294, 0, 0, 0, 16.5257, 0, 9.494, 0, 0.0194, 0.1798, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = 4;
        MC.startPoint = 2.00;

        mineralComp = "Epid_Clzt";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 17.8137858, 0, 0, 0, 17.63863053, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = 3;
        MC.startPoint = 2.00;

        mineralComp = "Magnetite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 72.34827478, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = 7;
        MC.startPoint = 2.00;

        mineralComp = "Apatite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 39.05798859, 0, 0, 0, 0, 0, 0, 0, 18.17529238, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = 10;
        MC.startPoint = 1.00;

        mineralComp = "Chalcopyrite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 34.5, 30.49887156, 0, 0, 0, 0, 0, 0, 0, 35, 0, 0, 0, 0, 0 });
        MC.weight = 8;
        MC.startPoint = 1.00;

        mineralComp = "Pyrite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 46.59103012, 0, 0, 0, 0, 0, 0, 0, 53.4, 0, 0, 0, 0, 0 });
        MC.weight = 5;
        MC.startPoint = 1.00;

        mineralComp = "Spalerite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 2.86, 0, 0, 0, 0, 0, 0, 0, 33.06, 0, 0, 0, 64.07, 0 });
        MC.weight = 5;
        MC.startPoint = 1.00;

        mineralComp = "galena";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 86.6, 13.4, 0, 0, 0, 0, 0 });
        MC.weight = 5;
        MC.startPoint = 1.00;

        mineralComp = "Uraninite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 88.15, 0, 0 });
        MC.weight = 10;
        MC.startPoint = 0.10;

        mineralComp = "Arsenopyrite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 46, 0, 0, 0, 0, 34.3, 0, 0, 0, 0, 0, 0, 0, 19.7, 0, 0, 0, 0, 0 });
        MC.weight = 10;
        MC.startPoint = 1.00;

        mineralComp = "Molybdenite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 59.9, 0, 0, 0, 40.1, 0, 0, 0, 0, 0 });
        MC.weight = 10;
        MC.startPoint = 1.00;

        mineralComp = "Chalcocite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 79.85, 0, 0, 0, 0, 0, 0, 0, 0, 20.15, 0, 0, 0, 0, 0 });
        MC.weight = 6;
        MC.startPoint = 1.00;

        mineralComp = "Sphene/titanite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 20.44022825, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 24.46569855, 0, 0, 0 });
        MC.weight = 2;
        MC.startPoint = 1.00;

        mineralComp = "Rutile";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 59.96494742, 0, 0, 0 });
        MC.weight = -1;
        MC.startPoint = 1.00;

        mineralComp = "Barite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 58.84, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 13.74, 0, 0, 0, 0, 0 });
        MC.weight = -1;
        MC.startPoint = 1.00;

        mineralComp = "Kaolinite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 20.75, 0, 0, 0, 0.04, 0, 0.16, 0.17, 0.04, 0, 0, 0.07, 0, 0, 0, 0, 0.05, 0, 0, 0 });
        MC.weight = 7.1;
        MC.startPoint = 1.00;

        mineralComp = "Smectite/Montmorillonite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 9.44, 0, 0, 0, 0.49, 0, 2.23, 0.37, 1.87, 0.023, 0, 1.61, 0, 0, 0, 0, 0.04, 0, 0, 0 });
        MC.weight = 7.2;
        MC.startPoint = 1.00;

        mineralComp = "Jarosite";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 33.45, 7.81, 0, 0, 0, 0, 0, 0, 12.81, 0, 0, 0, 0, 0 });
        MC.weight = 5.0;
        MC.startPoint = 1.00;

        mineralComp = "Tour_Fe";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 13.07, 0, 0, 0, 1.43, 0, 9.21, 0.04, 5.23, 0, 0, 1.26, 0, 0, 0, 0, 1.51, 0, 0, 0 });
        MC.weight = 7.5;
        MC.startPoint = 1.00;

        mineralComp = "Tour_Mg";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 18.03, 0, 0, 0, 0.45, 0, 2.38, 0.01, 5.69, 0, 0, 1.46, 0, 0, 0, 0, 0.24, 0, 0, 0 });
        MC.weight = 6;
        MC.startPoint = 1.00;

        mineralComp = "Chrysocolla";
        MC = new MineralComposition(mineralComp);
        defaultMineralDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 2.05, 0, 0, 0, 0, 33.86, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = 0;
        MC.startPoint = 1.00;


        defaultDataset = new Dataset(defaultMineralDict);




        mineralComp = "Ag";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "Al";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "As";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "Au";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "Ba";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "Ca";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "Cu";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "Fe";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "K";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "Mg";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "Mn";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "Mo";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "Na";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "P";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "Pb";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "S";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "Te";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "Ti";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "U";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "Zn";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0 });
        MC.weight = -100;
        MC.startPoint = 0.01;

        mineralComp = "Zr";
        MC = new MineralComposition(mineralComp);
        elementDict.Add(mineralComp, MC);
        FillMCDatabase(MC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100 });
        MC.weight = -100;
        MC.startPoint = 0.01;
    }

    public void FillMCDatabase(MineralComposition MC, double[] val)
    {
        MC.elementDictionary.Add("Ag", val[0]);
        MC.elementDictionary.Add("Al", val[1]);
        MC.elementDictionary.Add("As", val[2]);
        MC.elementDictionary.Add("Au", val[3]);
        MC.elementDictionary.Add("Ba", val[4]);
        MC.elementDictionary.Add("Ca", val[5]);
        MC.elementDictionary.Add("Cu", val[6]);
        MC.elementDictionary.Add("Fe", val[7]);
        MC.elementDictionary.Add("K", val[8]);
        MC.elementDictionary.Add("Mg", val[9]);
        MC.elementDictionary.Add("Mn", val[10]);
        MC.elementDictionary.Add("Mo", val[11]);
        MC.elementDictionary.Add("Na", val[12]);
        MC.elementDictionary.Add("P", val[13]);
        MC.elementDictionary.Add("Pb", val[14]);
        MC.elementDictionary.Add("S", val[15]);
        MC.elementDictionary.Add("Te", val[16]);
        MC.elementDictionary.Add("Ti", val[17]);
        MC.elementDictionary.Add("U", val[18]);
        MC.elementDictionary.Add("Zn", val[19]);
        MC.elementDictionary.Add("Zr", val[20]);


        //Ag	Al	As	Au	Ba	Ca	Cu	Fe	K	Mg	Mn	Mo	Na	P	Pb	S	Te	Ti	U	Zn	Zr


        //string[] ArrayOfElements = new string[] { "Al", "As", "Ba", "Ca", "Cu", "Fe", "K", "Mg", "Mn", "Mo", "Na", "P", "S", "Ti", "W" };
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

    public void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
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
