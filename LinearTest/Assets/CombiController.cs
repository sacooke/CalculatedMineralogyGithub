using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

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
    public GameObject errorPrefab;
    public GameObject QXRDPrefab;
    public GameObject sampleScrollView; //content
    public GameObject QXRDScrollview;
    public GameObject absErrElementScrollView;
    public GameObject absErrMineralScrollView;
    public GameObject relErrElementScrollView;
    public GameObject relErrMineralScrollView;

    public double[] elementDoubleList;
    public double[] chemicalDoubleList;
    public static double[] combinedDoubleList;

    public static double exportFunc = -1;
    public static string exportResult = "N/A";

    public Dictionary<string, CombiMineralComposition> combiMineralDict;
    public Dictionary<string, CombiMineralComposition> combiElementDict;

    static public double[][] mineral2DArray;
    static public double[] relativeError;
    static public double[] absoluteError;

    public InputField setAllRelativeAssayErrorInputField;
    public InputField setAllAbsoluteAssayErrorInputField;
    public InputField setAllRelativeQXRDErrorInputField;
    public InputField setAllAbsoluteQXRDErrorInputField;

    public bool QXRDListIsFull = false;
    public List<int> compQXRDIndexList = new List<int>();

    // Use this for initialization
    void Start () {
        //Debug.Log("The calculation here is wrong. Need to compare it with the original version to check what the problem is. Seems to lie in the sumproduct of x and [THING], but there's also an issue where the final value (100) isn't being accounted for.");

        AddAllCMCs();

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

    public void AddAllCMCs()
    {
        combiMineralDict = new Dictionary<string, CombiMineralComposition>();
        combiElementDict = new Dictionary<string, CombiMineralComposition>();

        string mineralComp;
        CombiMineralComposition CMC;

        mineralComp = "Other(Quartz)";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 10.00;

        mineralComp = "Feld_Alb_Ca-Na";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 10.41, 0, 0, 0, 0.38, 0, 0.05, 0.11, 0.01, 0, 0, 7.8, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 10.00;

        mineralComp = "Feld_Alb_Na";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 10.4, 0, 0, 0, 0, 0, 0.04, 0.24, 0.02, 0, 0, 8.21, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 10.00;

        mineralComp = "FeldsparK";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 9.817163594, 0, 0, 0, 0.12149786, 0, 0.0979097, 13.33707643, 0, 0, 0, 0.126114876, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 10.00;

        mineralComp = "FeldsKBa";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 9.85, 0, 0, 0.25, 0, 0, 0.08, 13.5, 0, 0.03, 0, 0.31, 0, 0, 0, 0, 0.03, 0, 0, 0 });
        CMC.weight = 2.00;

        mineralComp = "Feld_Plag";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 15.3732, 0, 0, 0, 7.8908, 0, 0.199, 0.1423, -0.0012, -0.0065, 0, 3.8507, 0, 0, 0, 0, 0.002, 0, 0, 0 });
        CMC.weight = 10.00;

        mineralComp = "Carb_Calc";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 40.04426534, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 2.00;

        mineralComp = "Carb_Ank";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 20.0185592, 0, 12.55342226, 0, 6.071985112, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 2.00;

        mineralComp = "Carb_Dol";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 21.73, 0, 0, 0, 13.18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 2.00;

        mineralComp = "Rhodonite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 47.8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Siderite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 53.4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Anhydrite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 29.44, 0, 0, 0, 0, 0, 0, 0, 0, 0, 23.55, 0, 0, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "ChloriteFe";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 9.6754, 0, 0, 0, 0.1066, 0, 20.3305, 0.0161, 9.3554, 0.0787, 0, 0.0455, 0, 0, 0, 0, 0.0163, 0, 0, 0 });
        CMC.weight = 5.00;

        mineralComp = "ChloriteMg";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 10.0939, 0, 0, 0, 0.0377, 0, 10.5241, 0.0335, 14.3654, 0.0387, 0, 0.0293, 0, 0, 0, 0, 0.0267, 0, 0, 0 });
        CMC.weight = 5.00;

        mineralComp = "Muscovite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 19.5, 0, 0, 0, 0, 0, 0.08, 8.37, 0.05, 0.093, 0, 0.47, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 2.00;

        mineralComp = "Musc_Phengite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 16.46, 0, 0, 0, 0, 0, 3.19, 9.14, 0.81, 0, 0, 0.08, 0, 0, 0, 0, 0.1, 0, 0, 0 });
        CMC.weight = 2.00;

        mineralComp = "BiotMg";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 7.5391, 0, 0, 0, 0.0156, 0, 7.9141, 7.8631, 11.8337, 0.052, 0, 0.0718, 0, 0, 0, 0, 1.0092, 0, 0, 0 });
        CMC.weight = 2.00;

        mineralComp = "BiotFe";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 7.3702, 0, 0, 0, 0.1426, 0, 13.0428, 7.3473, 8.2094, 0.0686, 0, 0.0769, 0, 0, 0, 0, 0.9201, 0, 0, 0 });
        CMC.weight = 2.00;

        mineralComp = "Amph_tscher";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 7.33, 0, 0, 0, 7.7, 0, 13.72, 0.62, 5.76, 0.33, 0, 1.43, 0, 0, 0, 0, 0.74, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Amph_Act";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 1.5633, 0, 0, 0, 8.7168, 0, 12.7119, 0.1038, 8.065, 0.0905, 0, 0.204, 0, 0, 0, 0, 0.0574, 0, 0, 0 });
        CMC.weight = 2.00;

        mineralComp = "Amph_HorMg";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 3.42, 0, 0, 0, 8.63, 0, 11.26, 0.43, 8.03, 0.48, 0, 0.78, 0, 0, 0, 0, 0.53, 0, 0, 0 });
        CMC.weight = 2.00;

        mineralComp = "Amph_Trem";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0.9295, 0, 0, 0, 9.3243, 0, 6.3783, 0.0668, 11.4574, 0.1231, 0, 0.2079, 0, 0, 0, 0, 0.0767, 0, 0, 0 });
        CMC.weight = 2.00;

        mineralComp = "Epid_LC";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 12.5294, 0, 0, 0, 16.5257, 0, 9.494, 0, 0.0194, 0.1798, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 2.00;

        mineralComp = "Epid_Clzt";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 17.8137858, 0, 0, 0, 17.63863053, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 2.00;

        mineralComp = "Magnetite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 72.34827478, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 2.00;

        mineralComp = "Apatite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 39.05798859, 0, 0, 0, 0, 0, 0, 0, 18.17529238, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Chalcopyrite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 34.5, 30.49887156, 0, 0, 0, 0, 0, 0, 0, 35, 0, 0, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Pyrite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 46.59103012, 0, 0, 0, 0, 0, 0, 0, 53.4, 0, 0, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Spalerite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 2.86, 0, 0, 0, 0, 0, 0, 0, 33.06, 0, 0, 0, 64.07, 0 });
        CMC.weight = 1.00;

        mineralComp = "galena";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 86.6, 13.4, 0, 0, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Uraninite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 88.15, 0, 0 });
        CMC.weight = 0.10;

        mineralComp = "Arsenopyrite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 46, 0, 0, 0, 0, 34.3, 0, 0, 0, 0, 0, 0, 0, 19.7, 0, 0, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Molybdenite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 59.9, 0, 0, 0, 40.1, 0, 0, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Chalcocite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 79.85, 0, 0, 0, 0, 0, 0, 0, 0, 20.15, 0, 0, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Sphene/titanite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 20.44022825, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 24.46569855, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Rutile";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 59.96494742, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Barite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 58.84, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 13.74, 0, 0, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Kaolinite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 20.75, 0, 0, 0, 0.04, 0, 0.16, 0.17, 0.04, 0, 0, 0.07, 0, 0, 0, 0, 0.05, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Smectite/Montmorillonite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 9.44, 0, 0, 0, 0.49, 0, 2.23, 0.37, 1.87, 0.023, 0, 1.61, 0, 0, 0, 0, 0.04, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Jarosite";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 33.45, 7.81, 0, 0, 0, 0, 0, 0, 12.81, 0, 0, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Tour_Fe";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 13.07, 0, 0, 0, 1.43, 0, 9.21, 0.04, 5.23, 0, 0, 1.26, 0, 0, 0, 0, 1.51, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Tour_Mg";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 18.03, 0, 0, 0, 0.45, 0, 2.38, 0.01, 5.69, 0, 0, 1.46, 0, 0, 0, 0, 0.24, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Chrysocolla";
        CMC = new CombiMineralComposition(mineralComp);
        combiMineralDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 2.05, 0, 0, 0, 0, 33.86, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 1.00;

        mineralComp = "Ag";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "Al";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "As";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "Au";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "Ba";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "Ca";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "Cu";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "Fe";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "K";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "Mg";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "Mn";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "Mo";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "Na";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "P";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "Pb";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "S";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "Te";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "Ti";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "U";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0 });
        CMC.weight = 0.01;

        mineralComp = "Zn";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0 });
        CMC.weight = 0.01;

        mineralComp = "Zr";
        CMC = new CombiMineralComposition(mineralComp);
        combiElementDict.Add(mineralComp, CMC);
        FillCMCDatabase(CMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100 });
        CMC.weight = 0.01;
    }

    public void FillCMCDatabase(CombiMineralComposition CMC, double[] val)
    {
        CMC.elementDictionary.Add("Ag", val[0]);
        CMC.elementDictionary.Add("Al", val[1]);
        CMC.elementDictionary.Add("As", val[2]);
        CMC.elementDictionary.Add("Au", val[3]);
        CMC.elementDictionary.Add("Ba", val[4]);
        CMC.elementDictionary.Add("Ca", val[5]);
        CMC.elementDictionary.Add("Cu", val[6]);
        CMC.elementDictionary.Add("Fe", val[7]);
        CMC.elementDictionary.Add("K", val[8]);
        CMC.elementDictionary.Add("Mg", val[9]);
        CMC.elementDictionary.Add("Mn", val[10]);
        CMC.elementDictionary.Add("Mo", val[11]);
        CMC.elementDictionary.Add("Na", val[12]);
        CMC.elementDictionary.Add("P", val[13]);
        CMC.elementDictionary.Add("Pb", val[14]);
        CMC.elementDictionary.Add("S", val[15]);
        CMC.elementDictionary.Add("Te", val[16]);
        CMC.elementDictionary.Add("Ti", val[17]);
        CMC.elementDictionary.Add("U", val[18]);
        CMC.elementDictionary.Add("Zn", val[19]);
        CMC.elementDictionary.Add("Zr", val[20]);


        //string[] ArrayOfElements = new string[] { "Al", "As", "Ba", "Ca", "Cu", "Fe", "K", "Mg", "Mn", "Mo", "Na", "P", "S", "Ti", "W" };
    }


    public class CombiMineralComposition
    {
        public string mineral { get; set; }
        public double weight { get; set; }

        public CombiMineralComposition(string mineral, double weight = 1)
        {
            this.mineral = mineral;
            this.weight = weight;
        }

        public Dictionary<string, double> elementDictionary = new Dictionary<string, double>();
        public Dictionary<string, double> mineralDictionary = new Dictionary<string, double>();

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

    public void PopulateErrorScrollviews()
    {
        ErrorListEntry ELE;
        foreach (Transform column in bothController.combiColumnScrollView.transform)
        {
            CombiMineralElementListEntry CMELE = column.gameObject.GetComponent<CombiMineralElementListEntry>();
            if (CMELE.ElementToggle.isOn)
            {
                if (double.IsNegativeInfinity(CMELE.ElementRelativeError))
                    CMELE.ElementRelativeError = 3;

                //start new prefab of ErrorListEntry absolute
                GameObject gER = GameObject.Instantiate(errorPrefab) as GameObject;
                //add the relative value and submit to the relative list
                gER.transform.SetParent(relErrElementScrollView.transform, false);
                ELE = gER.GetComponent<ErrorListEntry>();
                ELE.SetInputFieldValue(CMELE.ElementRelativeError.ToString());
                ELE.errorType = 0; //0 = elementRelativeError, 1 = elementAbsoluteError, 2 = mineralRelativeError, 3 = mineralAbsoluteError
                ELE.label.text = CMELE.SampleID;
                ELE.CMELE = CMELE;



                if (double.IsNegativeInfinity(CMELE.ElementAbsoluteError))
                    CMELE.ElementAbsoluteError = 0.02;

                //start new prefab of ErrorListEntry absolute
                GameObject gEA = GameObject.Instantiate(errorPrefab) as GameObject;
                //add the absolute value and submit to the absolute list
                gEA.transform.SetParent(absErrElementScrollView.transform, false);
                ELE = gEA.GetComponent<ErrorListEntry>();
                ELE.SetInputFieldValue(CMELE.ElementAbsoluteError.ToString());
                ELE.errorType = 1; //0 = elementRelativeError, 1 = elementAbsoluteError, 2 = mineralRelativeError, 3 = mineralAbsoluteError
                ELE.label.text = CMELE.SampleID;
                ELE.CMELE = CMELE;
                
            }
            else if(CMELE.MineralToggle.isOn)
            {
                if (double.IsNegativeInfinity(CMELE.MineralRelativeError))
                    CMELE.MineralRelativeError = 6;

                //start new prefab of ErrorListEntry absolute
                GameObject gER = GameObject.Instantiate(errorPrefab) as GameObject;
                //add the relative value and submit to the relative list
                gER.transform.SetParent(relErrMineralScrollView.transform, false);
                ELE = gER.GetComponent<ErrorListEntry>();
                ELE.SetInputFieldValue(CMELE.MineralRelativeError.ToString());
                ELE.errorType = 0; //0 = elementRelativeError, 1 = elementAbsoluteError, 2 = mineralRelativeError, 3 = mineralAbsoluteError
                ELE.label.text = CMELE.SampleID;
                ELE.CMELE = CMELE;
                 


                if (double.IsNegativeInfinity(CMELE.MineralAbsoluteError))
                    CMELE.MineralAbsoluteError = 0.50;

                //start new prefab of ErrorListEntry absolute
                GameObject gEA = GameObject.Instantiate(errorPrefab) as GameObject;
                //add the absolute value and submit to the absolute list
                gEA.transform.SetParent(absErrMineralScrollView.transform, false);
                ELE = gEA.GetComponent<ErrorListEntry>();
                ELE.SetInputFieldValue(CMELE.MineralAbsoluteError.ToString());
                ELE.errorType = 1; //0 = elementRelativeError, 1 = elementAbsoluteError, 2 = mineralRelativeError, 3 = mineralAbsoluteError
                ELE.label.text = CMELE.SampleID;
                ELE.CMELE = CMELE;

            }
        }
        bothController.errorMenu.SetActive(true);
        bothController.sharedMenu.SetActive(false);

    }

    public void DepopulateErrorScrollviews()
    {
        foreach (Transform error in relErrElementScrollView.transform)
        {
            ErrorListEntry ELE = error.GetComponent<ErrorListEntry>();
            if(ELE.inputField.text != "" && ELE.inputField.text != null)
            {
                if (ELE.errorType == 0)
                    ELE.CMELE.ElementRelativeError = Double.Parse(ELE.inputField.text);
                if (ELE.errorType == 1)
                    ELE.CMELE.ElementAbsoluteError = Double.Parse(ELE.inputField.text);
                if (ELE.errorType == 2)
                    ELE.CMELE.MineralRelativeError = Double.Parse(ELE.inputField.text);
                if (ELE.errorType == 3)
                    ELE.CMELE.MineralAbsoluteError = Double.Parse(ELE.inputField.text);
            }
            ELE.DestroySelf();
        }
        bothController.errorMenu.SetActive(false);
        bothController.sharedMenu.SetActive(true);

    }


    public void SetAllErrorWeights(int errorType)//0 = elementRelativeError, 1 = elementAbsoluteError, 2 = mineralRelativeError, 3 = mineralAbsoluteError
    {
        double result;
        switch (errorType)
        {
            case 0:
                if (double.TryParse(setAllRelativeAssayErrorInputField.text, out result))
                {
                    foreach (InputField inpF in relErrElementScrollView.transform.GetComponentsInChildren<InputField>())
                    {
                        inpF.text = result.ToString();
                    }
                }
                break;
            case 1:
                if (double.TryParse(setAllAbsoluteAssayErrorInputField.text, out result))
                {
                    foreach (InputField inpF in absErrElementScrollView.transform.GetComponentsInChildren<InputField>())
                    {
                        inpF.text = result.ToString();
                    }
                }
                break;
            case 2:
                if (double.TryParse(setAllRelativeQXRDErrorInputField.text, out result))
                {
                    foreach (InputField inpF in relErrMineralScrollView.transform.GetComponentsInChildren<InputField>())
                    {
                        inpF.text = result.ToString();
                    }
                }
                break;
            case 3:
                if (double.TryParse(setAllAbsoluteQXRDErrorInputField.text, out result))
                {
                    foreach (InputField inpF in absErrMineralScrollView.transform.GetComponentsInChildren<InputField>())
                    {
                        inpF.text = result.ToString();
                    }
                }
                break;
            default:
                Debug.Log("defaut case reached for SetAllErrorWEights()");
                break;

        }
    }

    public void PopulateQXRDScrollview()
    {
        if (!QXRDListIsFull)
        {
            QXRDListEntry QLE;
            List<string> mineralList = new List<string>();
            foreach (Transform column in bothController.combiColumnScrollView.transform)
            {
                CombiMineralElementListEntry CMELE = column.gameObject.GetComponent<CombiMineralElementListEntry>();
                if (CMELE.MineralToggle.isOn)
                {
                    mineralList.Add(CMELE.SampleID);
                    /*
                    if (double.IsNegativeInfinity(CMELE.MineralRelativeError))
                        CMELE.MineralRelativeError = 6;

                    //start new prefab of ErrorListEntry absolute
                    GameObject gER = GameObject.Instantiate(errorPrefab) as GameObject;
                    //add the relative value and submit to the relative list
                    gER.transform.SetParent(relErrMineralScrollView.transform, false);
                    ELE = gER.GetComponent<ErrorListEntry>();
                    ELE.SetInputFieldValue(CMELE.MineralRelativeError.ToString());
                    ELE.errorType = 0; //0 = elementRelativeError, 1 = elementAbsoluteError, 2 = mineralRelativeError, 3 = mineralAbsoluteError
                    ELE.label.text = CMELE.SampleID;
                    ELE.CMELE = CMELE;



                    if (double.IsNegativeInfinity(CMELE.MineralAbsoluteError))
                        CMELE.MineralAbsoluteError = 0.50;

                    //start new prefab of ErrorListEntry absolute
                    GameObject gEA = GameObject.Instantiate(errorPrefab) as GameObject;
                    //add the absolute value and submit to the absolute list
                    gEA.transform.SetParent(absErrMineralScrollView.transform, false);
                    ELE = gEA.GetComponent<ErrorListEntry>();
                    ELE.SetInputFieldValue(CMELE.MineralAbsoluteError.ToString());
                    ELE.errorType = 1; //0 = elementRelativeError, 1 = elementAbsoluteError, 2 = mineralRelativeError, 3 = mineralAbsoluteError
                    ELE.label.text = CMELE.SampleID;
                    ELE.CMELE = CMELE;*/

                }
            }

            int i = 0;
            foreach (Toggle tog in bothController.mineralCompScrollView.transform.GetComponentsInChildren<Toggle>())
            {
                if (tog.isOn)
                {
                    GameObject g = GameObject.Instantiate(QXRDPrefab) as GameObject;
                    g.transform.SetParent(QXRDScrollview.transform, false);
                    QLE = g.GetComponent<QXRDListEntry>();
                    QLE.index = i;
                    QLE.SetLabel(tog.gameObject.GetComponent<MineralCompositionListEntry>().MineralComp);
                    QLE.AddToDropdown(mineralList);
                    int optionVal = 0;
                    foreach(Dropdown.OptionData option in QLE.dropdown.options)
                    {
                        if (ContainsNoCase(option.text, QLE.MineralComp.Substring(0, 3)))
                        {
                            //Debug.Log(option.text + " has first three letters of " + QLE.MineralComp.Substring(0, 3));
                            QLE.dropdown.value = optionVal;
                        }
                        optionVal++;
                    }
                }
            }
            QXRDListIsFull = true;
        }

        bothController.QXRDMenu.SetActive(true);
        bothController.sharedMenu.SetActive(false);

    }

    public void ExitQXRDMenu()
    {
        bothController.QXRDMenu.SetActive(false);
        bothController.sharedMenu.SetActive(true);

    }

    public void DepopulateQXRDScrollview()
    {
        foreach (Transform QXRD in QXRDScrollview.transform)
        {
            QXRDListEntry QLE = QXRD.GetComponent<QXRDListEntry>();
            QLE.DestroySelf();
        }
        QXRDListIsFull = false;
        ExitQXRDMenu();
    }


    public void CalculateCombi2(bool toCSV)
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


    public void CalculateCombi(bool toCSV)
    {
        Debug.Log("CalculateCombi");
        StartCoroutine("EnumerateCombi", toCSV);

    }

    public IEnumerator EnumerateCombi(bool toCSV)
    {
        Debug.Log("EnumerateCombi");
        bothController.loadingMenu.SetActive(true);
        bothController.sharedMenu.SetActive(false);
        bothController.QXRDMenu.SetActive(false);

        string fullExportString = "";
        string columnNames = "Sample No.,Score";


        List<string> minList = new List<string>();
        List<double> weightList = new List<double>();

        List<string> assayHeaderList = new List<string>();
        List<string> QXRDHeaderList = new List<string>();
        List<int> assayIndexList = new List<int>();
        List<int> QXRDIndexList = new List<int>();
        List<double> assayRelativeErrorList = new List<double>();
        List<double> QXRDRelativeErrorList = new List<double>();
        List<double> assayAbsoluteErrorList = new List<double>();
        List<double> QXRDAbsoluteErrorList = new List<double>();
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
        foreach (CombiMineralElementListEntry CMELE in bothController.combiColumnScrollView.transform.GetComponentsInChildren<CombiMineralElementListEntry>())
        {
            if (CMELE.ElementToggle.isOn)
            {
                string elem = CMELE.SampleID;
                if (double.IsNegativeInfinity(CMELE.ElementRelativeError))
                    CMELE.ElementRelativeError = 3;
                if (double.IsNegativeInfinity(CMELE.ElementAbsoluteError))
                    CMELE.ElementAbsoluteError = 0.02;

                //AssayDataList contains all the _ppm data so that we can use them with the CMC dictionaries later
                assayHeaderList.Add(elem.Substring(0, elem.IndexOf('_')));//, elem.Length-elem.IndexOf('_')));
                assayIndexList.Add(CMELE.index);
                assayRelativeErrorList.Add(CMELE.ElementRelativeError);
                assayAbsoluteErrorList.Add(CMELE.ElementAbsoluteError);
                //Debug.Log("it = " + elem.Substring(0, elem.IndexOf('_')));//, elem.Length - elem.IndexOf('_')));
            }
            else if(CMELE.MineralToggle.isOn)
            {
                string elem = CMELE.SampleID;
                if (double.IsNegativeInfinity(CMELE.ElementRelativeError))
                    CMELE.ElementRelativeError = 6;
                if (double.IsNegativeInfinity(CMELE.ElementAbsoluteError))
                    CMELE.ElementAbsoluteError = 0.5;

                QXRDHeaderList.Add(elem.Substring(0, elem.IndexOf('_')));//, elem.Length-elem.IndexOf('_')));
                QXRDIndexList.Add(CMELE.index);
                QXRDRelativeErrorList.Add(CMELE.ElementRelativeError);
                QXRDAbsoluteErrorList.Add(CMELE.ElementAbsoluteError);
                //Debug.Log("it = " + elem.Substring(0, elem.IndexOf('_')));//, elem.Length - elem.IndexOf('_')));
            }
        }
        if (toCSV)
            export.WriteStringToStringBuilder(columnNames);
        else
        {
            export.WriteStringToStringBuilder(System.DateTime.Now.ToString("dd-MM-yyyy") + "_" + System.DateTime.Now.ToString("hh-mmtt"));
            export.WriteStringToStringBuilder("==========");
        }

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

        //populate compQXRDIndexList with all of the dropdown values
        foreach (QXRDListEntry QLE in QXRDScrollview.transform.GetComponentsInChildren<QXRDListEntry>())
        {
            Debug.Log("adding " + (QLE.dropdown.value-1));
            compQXRDIndexList.Add(QLE.dropdown.value-1);
        }



        //Initialise the Simplex Array with elementList (the columns from the CSV) and the minList (the list of minerals from the AMC)
        //double[,] simplexArray = new double[elementList.Count + 1, minList.Count];

        mineral2DArray = new double[assayHeaderList.Count + 1 + QXRDHeaderList.Count + 1][];


        //for each of the CSV columns j...
        //Debug.Log("For i <= " + (simplexArray2.GetUpperBound(1) - 1));
        for (int j = 0; j < mineral2DArray.Length; j++)
        {
            //Debug.Log("gt here " + j);
            mineral2DArray[j] = new double[minList.Count];
            //Debug.Log("mineral2DArray[j].Length = " + mineral2DArray[j].Length);
        }


        //and for each of the selected mineral compositions l...
        //Debug.Log("For j <= " + simplexArray2.GetUpperBound(0));
        for (int l = 0; l < minList.Count; l++)//i <= simplexArray.GetUpperBound(0); i++)
        {

            double other = 100;

            int row = 0;
            for (int j = 0; j < assayHeaderList.Count; j++)
            {

                //EVERYTYHING BELOW HERE IS OLD===============================================
                CombiMineralComposition CMC;
                double val = 0;
                //Debug.Log("minlist[" + i + "] = " + minList[i]);
                //Debug.Log("elementList[" + j + "] = " + elementList[j]);

                //Get the AMC for the Mineral Composition or singular Element
                if (combiMineralDict.TryGetValue(minList[l], out CMC))
                {
                    //Find the element value for the particular element within the AMC
                    CMC.elementDictionary.TryGetValue(assayHeaderList[j], out val); //=================<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                    mineral2DArray[j][l] = val;
                    //Debug.Log("m[" + j + ", " + i + "] = " + val + "(" + minList[i] + "/" + elementList[j] + ")");
                }
                else if (combiElementDict.TryGetValue(minList[l], out CMC))
                {
                    CMC.elementDictionary.TryGetValue(assayHeaderList[l], out val);
                    mineral2DArray[j][l] = val;
                    //Debug.Log("e[" + j + ", " + i + "] = " + val + "(" + minList[i] + "/" + elementList[j] + ")");
                }
                else
                    Debug.Log("failed");
                other -= val;
                row++;
            }

            mineral2DArray[row][l] = other;
            row++;
            int QXRDstartingPosition = row;
            for (int j = QXRDstartingPosition; j < QXRDHeaderList.Count + QXRDstartingPosition; j++)
            {
                //Debug.Log("if(" + QXRDHeaderList[j - QXRDstartingPosition] + " == " + minList[l]);
                Debug.Log("if(" + compQXRDIndexList[row - QXRDstartingPosition] + " == " + (j - QXRDstartingPosition));
                if (compQXRDIndexList[row - QXRDstartingPosition] == j - QXRDstartingPosition)
                {
                    Debug.Log("min2DArray[" + row + "][" + l + "] = 100 because " + (row - QXRDstartingPosition) + " = " + (j - QXRDstartingPosition));
                    mineral2DArray[row][l] = 69;
                }/*
                        if(QXRDHeaderList[j-QXRDstartingPosition] == minList[l])
                        {
                            mineral2DArray[row][l] = 100;

                        }*/
                row++;
            }

        }

        //========================DEBUG ENTIRE GRID===================================

        //Debug.Log("o[" + simplexArray2.GetUpperBound(0) + ", " + i + "] = " + other);
        int h = 0;
        for (int l = 0; l < minList.Count; l++)//i <= simplexArray.GetUpperBound(0); i++)
        {
            string outputLine = "";
            for (int u = 0; u < mineral2DArray[0].Length; u++)
            {
                outputLine += mineral2DArray[l][u].ToString() + ", ";
            }
            Debug.Log("<color=green>" + h + ": </color>" + outputLine);
            h++;
        }



        foreach (Toggle tog in bothController.sampleToggles)
        {
            if (tog.isOn)
            {
                //calculate for sampleID
                //this involves combiSampleValues 

                int s = tog.gameObject.GetComponent<SampleListEntry>().index;
                elementDoubleList = new double[csvController.elementPositions.Count + 1];

                string id = csvController.grid[0, s];

                if (!toCSV)
                {
                    export.WriteStringToStringBuilder("");
                    export.WriteStringToStringBuilder("ID: " + id);
                    export.WriteStringToStringBuilder("");
                }


                int i = 0;
                foreach (int ep in csvController.elementPositions)
                {
                    //Debug.Log("element: " + ep + "," + s + " = " + csvController.grid[ep, s]);
                    if (!double.TryParse(csvController.grid[ep, s], out elementDoubleList[i]))
                        Debug.Log("<color=red>error: could not parse double</color>");
                    //Debug.Log("elementDoubleList[" + i + "] = " + elementDoubleList[i]);
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
                    //Debug.Log("chemicalDoubleList[" + i + "] = " + chemicalDoubleList[i]);
                    i++;
                }

                combinedDoubleList = new double[elementDoubleList.Length + chemicalDoubleList.Length + 1];
                Array.Copy(elementDoubleList, combinedDoubleList, elementDoubleList.Length);
                Array.Copy(chemicalDoubleList, 0, combinedDoubleList, elementDoubleList.Length, chemicalDoubleList.Length);
                combinedDoubleList[combinedDoubleList.Length - 1] = 100;


                //double[] x = new double[chemicalDoubleList.Length]; //{ 15, 52, 23, 0, 0, 0, 0, 0, 0.5, 0, 0, 0, 0, 0, 1.0, 0, 0, 0, 0, 0, 0, 0, 0, 1.2, 0, 0.1, 1, 5, 0.5, 0, 0, 4, 0, 0, 0, 0 };//{ 1, 1, 1 };//
                //Array.Copy(chemicalDoubleList, x, chemicalDoubleList.Length);

                //Calculation starting point
                double[] x = new double[weightList.Count];
                double[] bndl = new double[x.Length]; //{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//{ -1, +1, -1 }; //lower = 0
                double[] bndu = new double[x.Length]; //{ 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };//{ +1, +3, +1 }; //upper = 100

                for (int w = 0; w < weightList.Count; w++)
                {
                    //Debug.Log(w + " adding " + weightList[w]);
                    x[w] = weightList[w];
                    bndl[w] = 0;
                    bndu[w] = 100;
                }
                //double[] x = new double[] { 15, 52, 23, 0, 0, 0, 0, 0, 0.5, 0, 0, 0, 0, 0, 1.0, 0, 0, 0, 0, 0, 0, 0, 0, 1.2, 0, 0.1, 1, 5, 0.5, 0, 0, 4, 0, 0, 0, 0 };

                //Bounds


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

                //10 for major, 1 for minor
                //make this the leftmost scrollview
                /*x[0] = chemicalDoubleList[0];
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
                x[35] = chemicalDoubleList[12];*/

                string secondArray = "";
                /*foreach (double d in x)
                {
                    secondArray += d.ToString() + ", ";
                }
                Debug.Log("x: " + secondArray);*/

                relativeError = new double[assayHeaderList.Count + 1 + QXRDHeaderList.Count + 1];
                absoluteError = new double[assayHeaderList.Count + 1 + QXRDHeaderList.Count + 1];

                int k = 0;
                foreach (double error in assayRelativeErrorList)
                {
                    relativeError[k] = error;
                    k++;
                }
                relativeError[k] = -1;
                foreach (double error in QXRDRelativeErrorList)
                {
                    relativeError[k] = error;
                    k++;
                }
                relativeError[k] = -1;

                k = 0;
                foreach (double error in assayAbsoluteErrorList)
                {
                    absoluteError[k] = error;
                    k++;
                }
                absoluteError[k] = -1;
                foreach (double error in QXRDAbsoluteErrorList)
                {
                    absoluteError[k] = error;
                    k++;
                }
                absoluteError[k] = -1;




                //assaydataother and such
                //double AssayDataOther = GetOtherValue(AssayDataList);

                alglib.minlmcreatev(combinedDoubleList.Length, x, 0.0001, out state);
                alglib.minlmsetbc(state, bndl, bndu);
                alglib.minlmsetcond(state, epsx, maxits);
                alglib.minlmsetxrep(state, true);
                alglib.minlmoptimize(state, function1_fvec, function1_rep, null);
                alglib.minlmresults(state, out x, out rep);

                //Debug.Log("thething: " + alglib.ap.format(x, 2));// EXPECTED: [-3,+3]
                exportResult = alglib.ap.format(x, 2);

                int degreesOfFreedom = combinedDoubleList.Length - x.Length;

                double chiSquare = ChiSquarePval(Math.Sqrt(exportFunc), degreesOfFreedom);

                string truncatedExportResult = exportResult.Substring(1, exportResult.Length - 2);
                if (toCSV)
                {

                    export.WriteStringToStringBuilder(id + "," + truncatedExportResult + "," + exportFunc + "," + degreesOfFreedom + "," + chiSquare);
                }
                if (!toCSV)
                {
                    export.WriteStringToStringBuilder("");
                    export.WriteStringToStringBuilder("Result: " + truncatedExportResult);
                    export.WriteStringToStringBuilder("");
                    export.WriteStringToStringBuilder("ExportFunc: " + exportFunc);
                    export.WriteStringToStringBuilder("Degrees of Freedom: " + degreesOfFreedom);
                    export.WriteStringToStringBuilder("Chi-Square Statistic: " + chiSquare);
                    export.WriteStringToStringBuilder("");
                    switch (rep.terminationtype)
                    {
                        case -8:
                            export.WriteStringToStringBuilder("optimizer detected NAN/INF values either in the function itself, or in its Jacobian");
                            Debug.Log("optimizer detected NAN/INF values either in the function itself, or in its Jacobian");
                            break;
                        case -7:
                            export.WriteStringToStringBuilder("derivative correctness check failed");
                            Debug.Log("derivative correctness check failed");
                            break;
                        case -5:
                            export.WriteStringToStringBuilder("inappropriate solver was used: solver created with minlmcreatefgh() used  on  problem  with general linear constraints(set with minlmsetlc() call).");
                            Debug.Log("inappropriate solver was used: solver created with minlmcreatefgh() used  on  problem  with general linear constraints(set with minlmsetlc() call).");
                            break;
                        case -3:
                            export.WriteStringToStringBuilder("constraints are inconsistent");
                            Debug.Log("constraints are inconsistent");
                            break;
                        case 2:
                            export.WriteStringToStringBuilder("relative step is no more than EpsX.");
                            Debug.Log("relative step is no more than EpsX.");
                            break;
                        case 5:
                            export.WriteStringToStringBuilder("MaxIts steps was taken");
                            Debug.Log("MaxIts steps was taken");
                            break;
                        case 7:
                            export.WriteStringToStringBuilder("stopping conditions are too stringent, further improvement is impossible");
                            Debug.Log("stopping conditions are too stringent, further improvement is impossible");
                            break;
                        case 8:
                            export.WriteStringToStringBuilder("terminated by user who called  MinLMRequestTermination(). X contains point which was 'current accepted' when termination request was submitted.");
                            Debug.Log("terminated by user who called  MinLMRequestTermination(). X contains point which was 'current accepted' when termination request was submitted.");
                            break;
                        default:
                            export.WriteStringToStringBuilder("Something went wrong! Error code: " + rep.terminationtype);
                            Debug.Log("Something went wrong! Error code: " + rep.terminationtype);
                            break;
                    }
                    export.WriteStringToStringBuilder("");
                    export.WriteStringToStringBuilder("Iterations Count: " + rep.iterationscount);
                    export.WriteStringToStringBuilder("");
                    export.WriteStringToStringBuilder("No. of function calculations: " + rep.nfunc);
                    export.WriteStringToStringBuilder("No. of Jacobi matrix calculations: " + rep.njac);
                    export.WriteStringToStringBuilder("No. of gradient calculations: " + rep.ngrad);
                    export.WriteStringToStringBuilder("No. of Hessian calculations: " + rep.nhess);
                    export.WriteStringToStringBuilder("No. of Cholesky decomposition calculations: " + rep.ncholesky);
                    export.WriteStringToStringBuilder("funcidx: " + rep.funcidx);
                    export.WriteStringToStringBuilder("varidx: " + rep.varidx);
                    export.WriteStringToStringBuilder("");
                    export.WriteStringToStringBuilder("");
                    export.WriteStringToStringBuilder("---------------------------------");
                    export.WriteStringToStringBuilder("");
                }


                bothController.progressBar.value++;
                bothController.progressText.text = bothController.progressBar.value + "/" + maxSamples;

                yield return null;
            }


        }
        using (StreamWriter sw = File.CreateText(export.tempFilename))
        {
            sw.Write(export.sb.ToString());
        }
        bothController.loadingMenu.SetActive(false);
        bothController.sharedMenu.SetActive(true);
        yield return null;
    }

    public void SetSampleValuesList(double[] sampleArray)
    {
        combiSampleValues = sampleArray;
    }

    public double GetOtherValue(List<double> list)
    {
        double other = 100;
        foreach (double d in list)
            other -= d;
        return other;
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

        //mineral2DArray = new double[2][]; //minList.length 

        //relativeError = new double[]{ 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, 3.00, -1, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, -1 };
        //absoluteError = new double[] { 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, -1, 0.50, 0.50, 0.50, 0.50, 0.10, 0.10, 0.10, 0.50, 0.50, 0.50, 0.10, 0.50, 0.10, 0.10, 0.50, 0.50, 0.50, 0.50, 0.10, 0.50, 0.10, 0.50, 0.50, 0.50, 0.10, 0.10, 0.10, 0.10, 0.10, -1 };
        

        //across length = mineral compositions + element compositions
        //down length = all selected columns plus 2 for the others
        /*double[][] minerals = {
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
        };*/

        //crucial misunderstanding - the lowest sum of squares needs to relate to B8

        //fi[0] = 0;//(B6-0.01*SUMPRODUCT(B16:B51,[the column]))/(B6*B1/100+B2) where B6 = arbitrary number from csv (B1 and B2 are both constants)   (((remove (B6*B1/100+B2) for 'Other')))
        //Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        /*foreach (double d in x)
        {
            Debug.Log("x/" + d);
        }*/

        for (int acrossCounter = 0; acrossCounter < fi.Length; acrossCounter++)
        {
            //Debug.Log("x.Length = " + x.Length);
            //Debug.Log("mineral2DArray[" + acrossCounter + "].Length = " + mineral2DArray[acrossCounter].Length);
            double product = SumProduct(x, mineral2DArray[acrossCounter]);
            if (relativeError[acrossCounter] != -1)
            {   //combinedDoubleList = the list of values provided by each line of the CSV
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
