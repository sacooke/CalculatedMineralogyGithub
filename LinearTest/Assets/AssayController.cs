using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class AssayController : MonoBehaviour
{

    public FileChooser fileChooser;
    private string fileContentString;
    public alglib alg;
    public Export export;
    public List<string> samples = new List<string>();
    public List<string> columns = new List<string>();
    public List<Toggle> sampleToggles = new List<Toggle>();
    public List<Toggle> columnToggles = new List<Toggle>();
    public static double[] combiSampleValues = { 8.67, 0.0021, 0.068, 1.22, 0.0149, 2.66, 2.92, 0.64, 0.0477, 0.0008, 3.97, 0.073, 0.01, 0.3, 0.00051, 79.40299, 15, 52, 23, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 5, 0, 0, 4, 0, 0, 0, 100 };

    public GameObject mainMenu;
    public GameObject assayMenu;
    public BothController bothController;
    public CSVReader csvController;
    public GameObject samplePrefab;
    public GameObject sampleScrollView;
    public GameObject columnScrollView;


    public double[] elementDoubleList;
    public double[] chemicalDoubleList;
    public static double[] combinedDoubleList;

    public static double exportFunc = -1;
    public static string exportResult = "N/A";

    public Dictionary<string, AssayMineralComposition> assayMineralDict;
    public Dictionary<string, AssayMineralComposition> assayElementDict;

    // Use this for initialization
    void Start()
    {
        //alglib.testminlpunit.testminlp(false, null);
        //Debug.Log("The calculation here is wrong. Need to compare it with the original version to check what the problem is. Seems to lie in the sumproduct of x and [THING], but there's also an issue where the final value (100) isn't being accounted for.");

        /*double[] x = new double[] { };

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
        /*alglib.minlpsetlc2(state, simplexArray, al, au);
        alglib.minlpoptimize(state);
        alglib.minlpresults(state, out x, out rep);

        Debug.Log("type = " + rep.terminationtype);



        Debug.Log("Values: " + alglib.ap.format(x, 2));// EXPECTED: [-3,+3]
        string exportResult = alglib.ap.format(x, 2);

        string truncatedExportResult = exportResult.Substring(1, exportResult.Length - 2);

        double[] resultArray = Array.ConvertAll(truncatedExportResult.Split(','), new Converter<string, double>(Double.Parse));

        double result = SumProduct(c, resultArray);
        Debug.Log("Result: " + result);*/

        //AddAllAMCs();




    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddAllAMCs()
    {


        assayMineralDict = new Dictionary<string, AssayMineralComposition>();
        assayElementDict = new Dictionary<string, AssayMineralComposition>();

        string mineralComp;
        AssayMineralComposition AMC;

        mineralComp = "Other(Quartz)";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = 7;

        mineralComp = "Feld_Alb_Ca-Na";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 10.41, 0, 0, 0, 0.38, 0, 0.05, 0.11, 0.01, 0, 0, 7.8, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = 5;

        mineralComp = "Feld_Alb_Na";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 10.4, 0, 0, 0, 0, 0, 0.04, 0.24, 0.02, 0, 0, 8.21, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = 5;

        mineralComp = "FeldsparK";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 9.817163594, 0, 0, 0, 0.12149786, 0, 0.0979097, 13.33707643, 0, 0, 0, 0.126114876, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = 7;

        mineralComp = "FeldsKBa";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 9.85, 0, 0, 0.25, 0, 0, 0.08, 13.5, 0, 0.03, 0, 0.31, 0, 0, 0, 0, 0.03, 0, 0, 0 });
        AMC.weight = 7;

        mineralComp = "Feld_Plag";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 15.3732, 0, 0, 0, 7.8908, 0, 0.199, 0.1423, -0.0012, -0.0065, 0, 3.8507, 0, 0, 0, 0, 0.002, 0, 0, 0 });
        AMC.weight = 3;

        mineralComp = "Carb_Calc";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 40.04426534, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = 0.3;

        mineralComp = "Carb_Ank";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 20.0185592, 0, 12.55342226, 0, 6.071985112, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = 5.7;

        mineralComp = "Carb_Dol";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 21.73, 0, 0, 0, 13.18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = 3;

        mineralComp = "Rhodonite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 47.8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = 3;

        mineralComp = "Siderite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 53.4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = 3;

        mineralComp = "Anhydrite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 29.44, 0, 0, 0, 0, 0, 0, 0, 0, 0, 23.55, 0, 0, 0, 0, 0 });
        AMC.weight = 1.3;

        mineralComp = "ChloriteFe";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 9.6754, 0, 0, 0, 0.1066, 0, 20.3305, 0.0161, 9.3554, 0.0787, 0, 0.0455, 0, 0, 0, 0, 0.0163, 0, 0, 0 });
        AMC.weight = 10;

        mineralComp = "ChloriteMg";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 10.0939, 0, 0, 0, 0.0377, 0, 10.5241, 0.0335, 14.3654, 0.0387, 0, 0.0293, 0, 0, 0, 0, 0.0267, 0, 0, 0 });
        AMC.weight = 10;

        mineralComp = "Muscovite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 19.5, 0, 0, 0, 0, 0, 0.08, 8.37, 0.05, 0.093, 0, 0.47, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = 7;

        mineralComp = "Musc_Phengite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 16.46, 0, 0, 0, 0, 0, 3.19, 9.14, 0.81, 0, 0, 0.08, 0, 0, 0, 0, 0.1, 0, 0, 0 });
        AMC.weight = 4;

        mineralComp = "BiotMg";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 7.5391, 0, 0, 0, 0.0156, 0, 7.9141, 7.8631, 11.8337, 0.052, 0, 0.0718, 0, 0, 0, 0, 1.0092, 0, 0, 0 });
        AMC.weight = 9;

        mineralComp = "BiotFe";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 7.3702, 0, 0, 0, 0.1426, 0, 13.0428, 7.3473, 8.2094, 0.0686, 0, 0.0769, 0, 0, 0, 0, 0.9201, 0, 0, 0 });
        AMC.weight = 9;

        mineralComp = "Amph_tscher";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 7.33, 0, 0, 0, 7.7, 0, 13.72, 0.62, 5.76, 0.33, 0, 1.43, 0, 0, 0, 0, 0.74, 0, 0, 0 });
        AMC.weight = 5;

        mineralComp = "Amph_Act";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 1.5633, 0, 0, 0, 8.7168, 0, 12.7119, 0.1038, 8.065, 0.0905, 0, 0.204, 0, 0, 0, 0, 0.0574, 0, 0, 0 });
        AMC.weight = 8;

        mineralComp = "Amph_HorMg";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 3.42, 0, 0, 0, 8.63, 0, 11.26, 0.43, 8.03, 0.48, 0, 0.78, 0, 0, 0, 0, 0.53, 0, 0, 0 });
        AMC.weight = 7.5;

        mineralComp = "Amph_Trem";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0.9295, 0, 0, 0, 9.3243, 0, 6.3783, 0.0668, 11.4574, 0.1231, 0, 0.2079, 0, 0, 0, 0, 0.0767, 0, 0, 0 });
        AMC.weight = 6;

        mineralComp = "Epid_LC";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 12.5294, 0, 0, 0, 16.5257, 0, 9.494, 0, 0.0194, 0.1798, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = 4;

        mineralComp = "Epid_Clzt";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 17.8137858, 0, 0, 0, 17.63863053, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = 3;

        mineralComp = "Magnetite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 72.34827478, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = 7;

        mineralComp = "Apatite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 39.05798859, 0, 0, 0, 0, 0, 0, 0, 18.17529238, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = 10;

        mineralComp = "Chalcopyrite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 34.5, 30.49887156, 0, 0, 0, 0, 0, 0, 0, 35, 0, 0, 0, 0, 0 });
        AMC.weight = 8;

        mineralComp = "Pyrite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 46.59103012, 0, 0, 0, 0, 0, 0, 0, 53.4, 0, 0, 0, 0, 0 });
        AMC.weight = 5;

        mineralComp = "Spalerite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 2.86, 0, 0, 0, 0, 0, 0, 0, 33.06, 0, 0, 0, 64.07, 0 });
        AMC.weight = 5;

        mineralComp = "galena";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 86.6, 13.4, 0, 0, 0, 0, 0 });
        AMC.weight = 5;

        mineralComp = "Uraninite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 88.15, 0, 0 });
        AMC.weight = 10;

        mineralComp = "Arsenopyrite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 46, 0, 0, 0, 0, 34.3, 0, 0, 0, 0, 0, 0, 0, 19.7, 0, 0, 0, 0, 0 });
        AMC.weight = 10;

        mineralComp = "Molybdenite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 59.9, 0, 0, 0, 40.1, 0, 0, 0, 0, 0 });
        AMC.weight = 10;

        mineralComp = "Chalcocite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 79.85, 0, 0, 0, 0, 0, 0, 0, 0, 20.15, 0, 0, 0, 0, 0 });
        AMC.weight = 6;

        mineralComp = "Sphene/titanite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 20.44022825, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 24.46569855, 0, 0, 0 });
        AMC.weight = 2;

        mineralComp = "Rutile";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 59.96494742, 0, 0, 0 });
        AMC.weight = -1;

        mineralComp = "Barite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 58.84, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 13.74, 0, 0, 0, 0, 0 });
        AMC.weight = -1;

        mineralComp = "Kaolinite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 20.75, 0, 0, 0, 0.04, 0, 0.16, 0.17, 0.04, 0, 0, 0.07, 0, 0, 0, 0, 0.05, 0, 0, 0 });
        AMC.weight = 7.1;

        mineralComp = "Smectite/Montmorillonite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 9.44, 0, 0, 0, 0.49, 0, 2.23, 0.37, 1.87, 0.023, 0, 1.61, 0, 0, 0, 0, 0.04, 0, 0, 0 });
        AMC.weight = 7.2;

        mineralComp = "Jarosite";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 33.45, 7.81, 0, 0, 0, 0, 0, 0, 12.81, 0, 0, 0, 0, 0 });
        AMC.weight = 5.0;

        mineralComp = "Tour_Fe";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 13.07, 0, 0, 0, 1.43, 0, 9.21, 0.04, 5.23, 0, 0, 1.26, 0, 0, 0, 0, 1.51, 0, 0, 0 });
        AMC.weight = 7.5;

        mineralComp = "Tour_Mg";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 18.03, 0, 0, 0, 0.45, 0, 2.38, 0.01, 5.69, 0, 0, 1.46, 0, 0, 0, 0, 0.24, 0, 0, 0 });
        AMC.weight = 6;

        mineralComp = "Chrysocolla";
        AMC = new AssayMineralComposition(mineralComp);
        assayMineralDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 2.05, 0, 0, 0, 0, 33.86, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = 0;

        mineralComp = "Ag";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "Al";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "As";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "Au";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "Ba";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "Ca";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "Cu";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "Fe";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "K";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "Mg";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "Mn";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "Mo";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "Na";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "P";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "Pb";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "S";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "Te";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "Ti";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0 });
        AMC.weight = -100;

        mineralComp = "U";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0 });
        AMC.weight = -100;

        mineralComp = "Zn";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0 });
        AMC.weight = -100;

        mineralComp = "Zr";
        AMC = new AssayMineralComposition(mineralComp);
        assayElementDict.Add(mineralComp, AMC);
        FillAMCDatabase(AMC, new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100 });
        AMC.weight = -100;
    }

    public void FillAMCDatabase(AssayMineralComposition AMC, double[] val)
    {
        AMC.elementDictionary.Add("Ag", val[0]);
        AMC.elementDictionary.Add("Al", val[1]);
        AMC.elementDictionary.Add("As", val[2]);
        AMC.elementDictionary.Add("Au", val[3]);
        AMC.elementDictionary.Add("Ba", val[4]);
        AMC.elementDictionary.Add("Ca", val[5]);
        AMC.elementDictionary.Add("Cu", val[6]);
        AMC.elementDictionary.Add("Fe", val[7]);
        AMC.elementDictionary.Add("K", val[8]);
        AMC.elementDictionary.Add("Mg", val[9]);
        AMC.elementDictionary.Add("Mn", val[10]);
        AMC.elementDictionary.Add("Mo", val[11]);
        AMC.elementDictionary.Add("Na", val[12]);
        AMC.elementDictionary.Add("P", val[13]);
        AMC.elementDictionary.Add("Pb", val[14]);
        AMC.elementDictionary.Add("S", val[15]);
        AMC.elementDictionary.Add("Te", val[16]);
        AMC.elementDictionary.Add("Ti", val[17]);
        AMC.elementDictionary.Add("U", val[18]);
        AMC.elementDictionary.Add("Zn", val[19]);
        AMC.elementDictionary.Add("Zr", val[20]);


        //Ag	Al	As	Au	Ba	Ca	Cu	Fe	K	Mg	Mn	Mo	Na	P	Pb	S	Te	Ti	U	Zn	Zr


        //string[] ArrayOfElements = new string[] { "Al", "As", "Ba", "Ca", "Cu", "Fe", "K", "Mg", "Mn", "Mo", "Na", "P", "S", "Ti", "W" };
    }

    public class AssayMineralComposition
    {
        public string mineral { get; set; }
        public double weight { get; set; }

        public AssayMineralComposition(string mineral, double weight = 1)
        {
            this.mineral = mineral;
            this.weight = weight;
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

    public void InitialiseAssay()
    {
        Debug.Log("init assay");
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

        for (int i = 1; i < csvController.grid.GetUpperBound(0); i++)
        {
            string columnHeader = csvController.grid[i,0];

            if(columnHeader != "" && columnHeader != null)
            {
                columns.Add(columnHeader);

                GameObject g = GameObject.Instantiate(samplePrefab) as GameObject;
                g.transform.SetParent(columnScrollView.transform, false);
                g.GetComponentInChildren<Text>().text = columnHeader;
                g.GetComponent<SampleListEntry>().SampleID = columnHeader;
                g.GetComponent<SampleListEntry>().index = i;
                //if (!ContainsNoCase(columnHeader, "_pc"))
                //    g.GetComponent<Toggle>().isOn = false;

            }
            //int samplePos = csvController.samplePositions[i];

            //sampleToggles.Add(g.GetComponent<Toggle>());
        }
    }

    /*
    public List<double> GetChemTestList(string[,] grid, int row)
    {
        chemTest.Clear();
        for (int x = 0; x <= grid.GetUpperBound(0); x++)
        {
            //Debug.Log("[" + x + "," + (row ) + "] = " + grid[x, row]);
            if (ContainsNoCase(grid[x, 0], "wt%") || ContainsNoCase(grid[x, 0], "_pc"))
            {
                Debug.Log("<color=blue>CHEMTEST: </color>" + grid[x, row]);
                chemTest.Add((Double.Parse(grid[x, row])) * 100);
                //Debug.Log((Double.Parse(grid[x, row ])) * 100);
            }
        }
        double totalChem = 0;

        foreach (double d in chemTest)
        {
            totalChem += d;
        }
        chemTest.Add(10000.0 - totalChem);
        foreach (double d in chemTest)
        {
            Debug.Log("d - " + d);
        }
        return chemTest;

    }*/

    public void CalculateAssay(bool toCSV)
    {
        StartCoroutine("EnumerateAssay",toCSV);
    }

    public void CalculateAssay2(bool toCSV)
    {
        //Prepare export strings
        string fullExportString = "";
        string columnNames = "Sample No.,Score";


        List<string> minList = new List<string>();
        List<double> weightList = new List<double>();

        List<string> elementList = new List<string>();
        List<int> columnIndexList = new List<int>();
        double result;

        //Compile list of mineral compositions to be used in the calculations from the scrollviews of minerals and elements
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

        //compile list of elements to be used in the calculations from the scrollview of column names
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
        //If we're exporting to a CSV, we have to write the column headers. Otherwise, we start with the date and time that the file was created
        if (toCSV)
            export.WriteStringToStringBuilder(columnNames);
        else
        {
            export.WriteStringToStringBuilder(System.DateTime.Now.ToString("dd-MM-yyyy") + "_" + System.DateTime.Now.ToString("hh-mmtt"));
            export.WriteStringToStringBuilder("==========");
        }

        foreach (Toggle tog in bothController.sampleToggles)
        {
            if (tog.isOn)
            {
                
                int s = tog.gameObject.GetComponent<SampleListEntry>().index;

                csvController.GetChemTestList(csvController.grid, s);
                

                string id = csvController.grid[0, s];

                if (!toCSV)
                {
                    export.WriteStringToStringBuilder("");
                    export.WriteStringToStringBuilder("ID: " + id);
                    export.WriteStringToStringBuilder("");
                }

                

                double[] x = new double[] { };
                
                
                //Initialise the Simplex Array with elementList (the columns from the CSV) and the minList (the list of minerals from the AMC)
                double[,] simplexArray = new double[elementList.Count+1, minList.Count];

                //The cost array is represented by the minList's weight values
                double[] c = new double[simplexArray.GetUpperBound(1)+1];

                //for each of the selected mineral compositions i...
                //Debug.Log("For i <= " + (simplexArray2.GetUpperBound(1) - 1));
                for (int i = 0; i <= simplexArray.GetUpperBound(1); i++)
                {

                    double other = 100;

                    //and for each of the CSV columns j...
                    //Debug.Log("For j <= " + simplexArray2.GetUpperBound(0));
                    for (int j = 0; j <= simplexArray.GetUpperBound(0) - 1; j++)//i <= simplexArray.GetUpperBound(0); i++)
                    {
                        BothController.MineralComposition MC;
                        double val = 0;
                        //Debug.Log("minlist[" + i + "] = " + minList[i]);
                        //Debug.Log("elementList[" + j + "] = " + elementList[j]);

                        //Get the AMC for the Mineral Composition or singular Element
                        if (bothController.currentMineralDict.TryGetValue(minList[i], out MC))
                        {
                            //Find the element value for the particular element within the AMC
                            MC.elementDictionary.TryGetValue(elementList[j], out val);
                            simplexArray[j, i] = val;
                            //Debug.Log("m[" + j + ", " + i + "] = " + val + "(" + minList[i] + "/" + elementList[j] + ")");
                        }
                        else if (bothController.elementDict.TryGetValue(minList[i], out MC))
                        {
                            MC.elementDictionary.TryGetValue(elementList[j], out val);
                            simplexArray[j, i] = val;
                            //Debug.Log("e[" + j + ", " + i + "] = " + val + "(" + minList[i] + "/" + elementList[j] + ")");
                        }
                        else
                            Debug.Log("failed");
                        other -= val;
                    }

                    simplexArray[simplexArray.GetUpperBound(0), i] = other;
                    //Debug.Log("o[" + simplexArray2.GetUpperBound(0) + ", " + i + "] = " + other);

                    c[i] = -weightList[i];
                    //Debug.Log("c[" + i + "] = " + c[i]);
                }

                //CSVReader.DebugOutputGrid(simplexArray);
                CSVReader.DebugOutputGrid(simplexArray);
                /*double[,] simplexArray = new double[,]
                                                    {
                                                    { 0, -10.6, -9.69, -16.95, -20.3, -17.82, -20.9, -100, 0, 0, 0},
                                                    { 0, 0, 0, -25.18, 0, -17.64, 0, 0, -100, 0, 0},
                                                    { 0, 0, -14.05, 0, -9.8, 0, 0, 0, 0, -100, 0},
                                                    { 0, -8.5, 0, 0, 0, 0, 0, 0, 0, 0, -100},
                                                    { -100, -80.6, -76.26, -57.87, -69.8, -64.54, -79.1, 0, 0, 0, 0}
                                                    };*/

                CSVReader.DebugOutputGrid(csvController.grid);
                double[] cn = new double[columnIndexList.Count+1];//csvController.chemTest.ToArray();
                double cnOther = 10000;
                //Debug.Log("[0,s] = " + csvController.grid[0,s]);
                for (int i = 0; i < columnIndexList.Count; i++)
                {
                    //Debug.Log("[columnIndexList[i],0] = " + csvController.grid[columnIndexList[i], 0]);
                    double.TryParse(csvController.grid[columnIndexList[i], s], out cn[i]);
                    cn[i] *= 100;
                    cnOther -= cn[i];
                }
                cn[cn.Length-1] = cnOther;

                /*foreach (double d in cn)
                    Debug.Log("cn = " + d);

                foreach (double d in c)
                    Debug.Log("c = " + d);*/



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


                //double[] bndl = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};//{ -1, +1, -1 }; //lower = 0
                //double[] bndu = new double[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100};//{ +1, +3, +1 }; //upper = 100

                //double[] bndl = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//{ -1, +1, -1 }; //lower = 0
                //double[] bndu = new double[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };//, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };//{ +1, +3, +1 }; //upper = 100

                //double[] bndu = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//{ -1, +1, -1 }; //lower = 0
                //double[] bndl = new double[] { -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100 };//, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };//{ +1, +3, +1 }; //upper = 100

                //double[] al = new double[] { cn[0], cn[1], cn[2], cn[3], cn[4], cn[5], cn[6], cn[7], cn[8], cn[9], cn[10], cn[11], cn[12], cn[13], cn[14], cn[15] };//{ -1, +1, -1 }; //lower = 0
                //double[] au = new double[] { cn[0], cn[1], cn[2], cn[3], cn[4], cn[5], cn[6], cn[7], cn[8], cn[9], cn[10], cn[11], cn[12], cn[13], cn[14], cn[15] };//{ +1, +3, +1 }; //upper = 100
                //double[] al = new double[] { cn[0], cn[1], cn[2], cn[3], cn[4] };
                //double[] au = new double[] { cn[0], cn[1], cn[2], cn[3], cn[4] };
                //double[] al = new double[] { -cn[0], -cn[1], -cn[2], -cn[3], -cn[4] };
                //double[] au = new double[] { -cn[0], -cn[1], -cn[2], -cn[3], -cn[4] };

                //double[] c = { -7, -5, -5, -7, -7, -3, -0.3, -5.4, -3, -0.7, -10, -10, -7.1, -5, -9, -9.5, -8, -7.5, -6, -4, -3, -7, -10, -8, -5, -10,-10,-10, -6, -2, 1, 1, -7.2, -7.2, -5.0, -7.5, -6, 100, 100, 100, 100, 100, 100, 100, 0, 100 };
                //double[] c = { 5, 5, 5, 5, 5, 5, 5, -100, -100, -100, -100 };
                //double[] c = { -5, -5, -5, -5, -5, -5.1, -5, 100, 100, 100, 100 };
                //double[] c = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

                /*double[,] simplexArrayLC1 = new double[,]
                                                    {
                                                    { 0, 10.6, 9.69, 16.95, 20.3, 17.82, 20.9, 100, 0, 0, 0, cn[0]},
                                                    { 0, 0, 0, 25.18, 0, 17.64, 0, 0, 100, 0, 0, cn[1]},
                                                    { 0, 0, 014.05, 0, 9.8, 0, 0, 0, 0, 100, 0, cn[2]},
                                                    { 0, 8.5, 0, 0, 0, 0, 0, 0, 0, 0, 100, cn[3]},
                                                    { 100, 80.6, 76.26, 57.87, 69.8, 64.54, 79.1, 0, 0, 0, 0, cn[4]}
                                                    };*/

                //int[] ct = { 0, 0, 0, 0, 0 };
                //int[] ct = { -1, -1, -1, -1, -1 };
                //fullExportString += "\nct: ";
                //foreach (double d in ct)
                //    fullExportString += d + ", ";
                //double[] scale = { 5, 5, 5, 5, 5, 5, 5, -100, -100, -100, -100 };
                //double[] scale = { 5, 5, 5, 5, 5, 5, 5, -1, 1, 1, 1 };
                //fullExportString += "\nscale: ";
                //foreach (double d in scale)
                //    fullExportString += d + ", ";
                

                /*for (int i = 0; i < simplexArrayLC1.GetUpperBound(0); i++)
                {
                    for(int j = 0; j < scale.Length; j++)
                        simplexArray[i, j] *= scale[j];
                }*/

                /*Debug.Log("Simp " + simplexArray2.GetUpperBound(0) + ", " + simplexArray2.GetUpperBound(1));
                Debug.Log("Simp orig " + simplexArray.GetUpperBound(0) + ", " + simplexArray.GetUpperBound(1));
                Debug.Log("Bndl " + bndl.Length);
                Debug.Log("Cn " + cn.Length);*/
                
                alglib.minlpstate state;
                alglib.minlpreport rep;
                alglib.minlpcreate(simplexArray.GetUpperBound(1)+1, out state);
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


                    Debug.Log("primal feasibility error: " + rep.primalerror);
                    export.WriteStringToStringBuilder("primal feasibility error: " + rep.primalerror);
                    Debug.Log("iteration count: " + rep.iterationscount);
                    fullExportString += "\niteration count: " + rep.iterationscount;

                    string statsLine = "stats: ";

                    foreach (int i in rep.stats)
                    {
                        statsLine += i;
                        statsLine += ", ";
                    }

                    Debug.Log("array[N+M], statuses of box (N) and linear (M) constraints: " + statsLine);
                    export.WriteStringToStringBuilder("array[N+M], statuses of box (N) and linear (M) constraints: " + statsLine);
                    export.WriteStringToStringBuilder("");
                    export.WriteStringToStringBuilder("---------------------------------");
                    export.WriteStringToStringBuilder("");
                }

                Debug.Log("target function value: " + rep.f);
                Debug.Log("Values: " + alglib.ap.format(x, 2));
                




                string exportResult =  alglib.ap.format(x, 2);
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

                Debug.Log("<color=green>--------------------------</color>");

                //Debug.Log("'Result': " + result);
            }


        }

        //string dateTime = System.DateTime.Now.ToString("dd-MM-yyyy") + "_" + System.DateTime.Now.ToString("hh-mmtt");

        //System.IO.File.WriteAllText(@"F:\LinearTest\CalculatedMineralogyGithub\CalculatedMineralogyGithub\WriteText" + dateTime + "_" + f + ".txt", fullExportString);

        Debug.Log("<color=red>=====================================================================================</color>");

        //oldCalculate();
    }   //CalculateCombi()

    

    public IEnumerator EnumerateAssay(bool toCSV)
    {

        bothController.loadingMenu.SetActive(true);
        bothController.sharedMenu.SetActive(false);

        string fullExportString = "";
        string columnNames = "Sample No.,Score";


        List<string> minList = new List<string>();
        List<double> weightList = new List<double>();

        List<string> elementList = new List<string>();
        List<int> columnIndexList = new List<int>();
        double result;

        //Compile list of mineral compositions to be used in the calculations from the scrollviews of minerals and elements
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
                tog.isOn = true;
                minList.Add(elemString);
                weightList.Add(double.TryParse(tog.GetComponentInChildren<InputField>().text, out result) ? result : -100);
                columnNames += "," + elemString;
                
            }
        }

        //compile list of elements to be used in the calculations from the scrollview of column names
        foreach (Toggle tog in bothController.columnScrollView.transform.GetComponentsInChildren<Toggle>())
        {
            if (tog.isOn)
            {
                string elem = tog.gameObject.GetComponent<SampleListEntry>().SampleID;
                elementList.Add(elem.Substring(0, elem.IndexOf('_')));//, elem.Length-elem.IndexOf('_')));
                columnIndexList.Add(tog.gameObject.GetComponent<SampleListEntry>().index);
            }
        }
        //If we're exporting to a CSV, we have to write the column headers. Otherwise, we start with the date and time that the file was created
        if (toCSV)
            export.WriteStringToStringBuilder(columnNames);
        else
        {
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

        foreach (Toggle tog in bothController.sampleToggles)
        {
            if (tog.isOn)
            {
                int s = tog.gameObject.GetComponent<SampleListEntry>().index;

                csvController.GetChemTestList(csvController.grid, s);


                string id = csvController.grid[0, s];

                if (!toCSV)
                {
                    export.WriteStringToStringBuilder("");
                    export.WriteStringToStringBuilder("ID: " + id);
                    export.WriteStringToStringBuilder("");
                }



                double[] x = new double[] { };



                double[,] simplexArray = new double[elementList.Count + 1, minList.Count];

                double[] c = new double[simplexArray.GetUpperBound(1) + 1];
                
                for (int i = 0; i <= simplexArray.GetUpperBound(1); i++)
                {

                    double other = 100;
                    
                    for (int j = 0; j <= simplexArray.GetUpperBound(0) - 1; j++)//i <= simplexArray.GetUpperBound(0); i++)
                    {
                        BothController.MineralComposition MC;
                        double val = 0;
                        if (i < 3 && j < 3)
                        {
                            Debug.Log(i + ": checking if " + minList[i] + " is in currentMineralDict or elementDict");
                        }
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

                Debug.Log("Simplex array:");
                CSVReader.DebugOutputGrid(simplexArray);

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
                string resultString = "";
                foreach (double d in resultArray)
                    resultString += d.ToString("F3") + ",";
                Debug.Log("results: " + resultString);

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



                /*string resultLine = "resultArray: ";

                foreach (double i in resultArray)
                {
                    resultLine += i;
                    resultLine += ", ";
                }
                Debug.Log(resultLine);*/

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
        bothController.sharedMenu.SetActive(true);
        yield return null;
    }





    public void oldCalculate()
    {

        string fullExportString = "";
        Debug.Log("testing");
        foreach (Toggle tog in bothController.sampleToggles)
        {
            if (tog.isOn)
            {
                bool biggerCalc = false;

                Debug.Log("got this far");
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
                fullExportString += "\nID: " + id + "\n";

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

                double[,] simplexArray;

                if (biggerCalc)
                {
                    simplexArray = new double[,] {
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
                }
                else
                {

                    simplexArray = new double[,]
                                                        {
                                                        { 0, 10.6, 9.69, 16.95, 20.3, 17.82, 20.9, 100, 0, 0, 0},
                                                        { 0, 0, 0, 25.18, 0, 17.64, 0, 0, 100, 0, 0},
                                                        { 0, 0, 014.05, 0, 9.8, 0, 0, 0, 0, 100, 0},
                                                        { 0, 8.5, 0, 0, 0, 0, 0, 0, 0, 0, 100},
                                                        { 100, 80.6, 76.26, 57.87, 69.8, 64.54, 79.1, 0, 0, 0, 0}
                                                        };
                }


                CSVReader.DebugOutputGrid(simplexArray);
                /*double[,] simplexArray = new double[,]
                                                    {
                                                    { 0, -10.6, -9.69, -16.95, -20.3, -17.82, -20.9, -100, 0, 0, 0},
                                                    { 0, 0, 0, -25.18, 0, -17.64, 0, 0, -100, 0, 0},
                                                    { 0, 0, -14.05, 0, -9.8, 0, 0, 0, 0, -100, 0},
                                                    { 0, -8.5, 0, 0, 0, 0, 0, 0, 0, 0, -100},
                                                    { -100, -80.6, -76.26, -57.87, -69.8, -64.54, -79.1, 0, 0, 0, 0}
                                                    };*/

                double[] cn = csvController.chemTest.ToArray();
                fullExportString += "\ncn: ";
                foreach (double d in cn)
                    fullExportString += d + ", ";

                double[] bndl;
                double[] bndu;
                double[] al;
                double[] au;
                double[] c;

                if (biggerCalc)
                {
                    bndl = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//{ -1, +1, -1 }; //lower = 0
                    bndu = new double[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };//{ +1, +3, +1 }; //upper = 100
                    al = new double[] { cn[0], cn[1], cn[2], cn[3], cn[4], cn[5], cn[6], cn[7], cn[8], cn[9], cn[10], cn[11], cn[12], cn[13], cn[14], cn[15] };//{ -1, +1, -1 }; //lower = 0
                    au = new double[] { cn[0], cn[1], cn[2], cn[3], cn[4], cn[5], cn[6], cn[7], cn[8], cn[9], cn[10], cn[11], cn[12], cn[13], cn[14], cn[15] };//{ +1, +3, +1 }; //upper = 100
                    c = new double[] { -7, -5, -5, -7, -7, -3, -0.3, -5.4, -3, -0.7, -10, -10, -7.1, -5, -9, -9.5, -8, -7.5, -6, -4, -3, -7, -10, -8, -5, -10, -10, -10, -6, -2, 1, 1, -7.2, -7.2, -5.0, -7.5, -6, 100, 100, 100, 100, 100, 100, 100, 0, 100 };
                }
                else
                {
                    bndl = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//{ -1, +1, -1 }; //lower = 0
                    bndu = new double[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };//, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };//{ +1, +3, +1 }; //upper = 100
                    al = new double[] { cn[0], cn[1], cn[2], cn[3], cn[4] };
                    au = new double[] { cn[0], cn[1], cn[2], cn[3], cn[4] };
                    c = new double[] { -5, -5, -5, -5, -5, -5.1, -5, 100, 100, 100, 100 };

                }
                //double[] bndu = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//{ -1, +1, -1 }; //lower = 0
                //double[] bndl = new double[] { -100, -100, -100, -100, -100, -100, -100, -100, -100, -100, -100 };//, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };//{ +1, +3, +1 }; //upper = 100

                //double[] al = new double[] { -cn[0], -cn[1], -cn[2], -cn[3], -cn[4] };
                //double[] au = new double[] { -cn[0], -cn[1], -cn[2], -cn[3], -cn[4] };

                //double[] c = { 5, 5, 5, 5, 5, 5, 5, -100, -100, -100, -100 };
                fullExportString += "\nc: ";
                foreach (double d in c)
                    fullExportString += d + ", ";
                //double[] c = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

                double[,] simplexArrayLC1 = new double[,]
                                                    {
                                                    { 0, 10.6, 9.69, 16.95, 20.3, 17.82, 20.9, 100, 0, 0, 0, cn[0]},
                                                    { 0, 0, 0, 25.18, 0, 17.64, 0, 0, 100, 0, 0, cn[1]},
                                                    { 0, 0, 014.05, 0, 9.8, 0, 0, 0, 0, 100, 0, cn[2]},
                                                    { 0, 8.5, 0, 0, 0, 0, 0, 0, 0, 0, 100, cn[3]},
                                                    { 100, 80.6, 76.26, 57.87, 69.8, 64.54, 79.1, 0, 0, 0, 0, cn[4]}
                                                    };

                //int[] ct = { 0, 0, 0, 0, 0 };
                //int[] ct = { -1, -1, -1, -1, -1 };
                //fullExportString += "\nct: ";
                //foreach (double d in ct)
                //    fullExportString += d + ", ";
                //double[] scale = { 5, 5, 5, 5, 5, 5, 5, -100, -100, -100, -100 };
                //double[] scale = { 5, 5, 5, 5, 5, 5, 5, -1, 1, 1, 1 };
                //fullExportString += "\nscale: ";
                //foreach (double d in scale)
                //    fullExportString += d + ", ";

                fullExportString += "\n\n";

                /*for (int i = 0; i < simplexArrayLC1.GetUpperBound(0); i++)
                {
                    for(int j = 0; j < scale.Length; j++)
                        simplexArray[i, j] *= scale[j];
                }*/
                
                alglib.minlpstate state;
                alglib.minlpreport rep;
                alglib.minlpcreate(bndl.Length, out state);
                alglib.minlpsetcost(state, c);
                alglib.minlpsetbc(state, bndl, bndu);
                //alglib.minlpsetscale(state, scale);
                //alglib.minlpsetcost(state, bndl); //replace bndl with something
                //alglib.minlpsetbc(state, bndl, bndu);
                /*alglib.minlpsetlc1(state, true); //a, ct is constraint types so >0 is >= =0 is = and <0 is <=, k*/
                /*
        CT      -   constraint types, array[K]:
                    * if CT[i]>0, then I-th constraint is A[i,*]*x >= A[i,n]
                    * if CT[i]=0, then I-th constraint is A[i,*]*x  = A[i,n]
                    * if CT[i]<0, then I-th constraint is A[i,*]*x <= A[i,n]*/
                alglib.minlpsetlc2(state, simplexArray, al, au);
                //int[] ct = { };
                //alglib.minlpsetlc1(state, simplexArrayLC1, ct);
                alglib.minlpoptimize(state);
                alglib.minlpresults(state, out x, out rep);


                switch (rep.terminationtype)
                {
                    case -4:
                        fullExportString += "\nLP problem is primal unbounded(dual infeasible)";
                        Debug.Log("LP problem is primal unbounded(dual infeasible)");
                        break;
                    case -3:
                        fullExportString += "\nLP problem is primal infeasible(dual unbounded)";
                        Debug.Log("LP problem is primal infeasible(dual unbounded)");
                        break;
                    case 1:
                        fullExportString += "\nsuccessful completion 1";
                        Debug.Log("successful completion 1");
                        break;
                    case 2:
                        fullExportString += "\nsuccessful completion 2";
                        Debug.Log("successful completion 2");
                        break;
                    case 3:
                        fullExportString += "\nsuccessful completion 3";
                        Debug.Log("successful completion 3");
                        break;
                    case 4:
                        fullExportString += "\nsuccessful completion 4";
                        Debug.Log("successful completion 4");
                        break;
                    case 5:
                        fullExportString += "\nMaxIts steps was taken";
                        Debug.Log("MaxIts steps was taken");
                        break;
                    case 7:
                        fullExportString += "\nstopping conditions are too stringent, further improvement is impossible, X contains best point found so far.";
                        Debug.Log("stopping conditions are too stringent, further improvement is impossible, X contains best point found so far.");
                        break;
                }

                Debug.Log("target function value: " + rep.f);
                Debug.Log("Values: " + alglib.ap.format(x, 2));

                fullExportString += "\n\ntarget function value: " + rep.f;
                fullExportString += "\nValues: " + alglib.ap.format(x, 2);

                string dualLine = "dual variables: ";

                foreach (double d in rep.y)
                {
                    dualLine += d;
                    dualLine += ", ";
                }

                Debug.Log(dualLine);

                fullExportString += "\n" + dualLine;

                Debug.Log("primal feasibility error: " + rep.primalerror);
                fullExportString += "\nprimal feasibility error: " + rep.primalerror;
                Debug.Log("iteration count: " + rep.iterationscount);
                fullExportString += "\niteration count: " + rep.iterationscount;

                string statsLine = "stats: ";

                foreach (int i in rep.stats)
                {
                    statsLine += i;
                    statsLine += ", ";
                }

                Debug.Log("array[N+M], statuses of box (N) and linear (M) constraints: " + statsLine);
                fullExportString += "\narray[N+M], statuses of box (N) and linear (M) constraints: " + statsLine + "\n\n---------------------------------\n\n";

                string exportResult = alglib.ap.format(x, 2);

                string truncatedExportResult = exportResult.Substring(1, exportResult.Length - 2);

                double[] resultArray = Array.ConvertAll(truncatedExportResult.Split(','), new Converter<string, double>(Double.Parse));

                int d2 = bndl.Length;
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

                double result = SumProduct(c, resultArray);

                Debug.Log("c length = " + c.Length);
                Debug.Log("resultArray length = " + resultArray.Length);
                Debug.Log("Simp " + simplexArray.GetUpperBound(0) + ", " + simplexArray.GetUpperBound(1));
                Debug.Log("Cn " + cn.Length);

                string cLine = "c: ";

                foreach (double i in c)
                {
                    cLine += i;
                    cLine += ", ";
                }
                Debug.Log(cLine);


                string resultLine = "resultArray: ";

                foreach (double i in resultArray)
                {
                    resultLine += i;
                    resultLine += ", ";
                }
                Debug.Log(resultLine);

                double endProduct = SumProduct(c, resultArray);

                Debug.Log("<color=yellow>--------------------------</color>");

            }

        }   //CalculateCombi()
        string dateTime = System.DateTime.Now.ToString("dd-MM-yyyy") + "_" + System.DateTime.Now.ToString("hh-mmtt");

        System.IO.File.WriteAllText(@"F:\LinearTest\CalculatedMineralogyGithub\CalculatedMineralogyGithub\OTHER_WriteText" + dateTime + ".txt", fullExportString);
        
    }

    


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

    
}
