/*
	original CSVReader code by Dock. (24/8/11)
	http://starfruitgames.com
    http://wiki.unity3d.com/index.php?title=CSVReader

	usage: 
	CSVReader.SplitCsvGrid(textString)
 
	returns a 2D string array. 
 
	Drag onto a gameobject for a demo of CSV parsing.


    Updated by Stephen Cooke (2017) to perform functions specific to ore spreadsheets
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CSVReader : MonoBehaviour
{
    public string csvFile;

    //public float[] NorthValues;
    //public float[] EastValues;
    //public float[] ElevValues;

    public List<string> idValues = new List<string>();
    public List<int> idPositions = new List<int>();

    public List<string> elementList = new List<string>();
    public List<int> elementPositions = new List<int>();
    public int elementCount = 0;

    public List<string> chemicalList = new List<string>();
    public List<int> chemicalPositions = new List<int>();
    public int chemicalCount = 0;

    public List<string> sampleList = new List<string>();
    public List<int> samplePositions = new List<int>();
    public int sampleCount = 0;

    //public List<string> materialList = new List<string>();
    //public List<string> depositList = new List<string>();

    public List<int> RowsToUse = new List<int>();

    //public string[,] idCollection; //the averaged values of the samples
    //public string[,] idCollectionUnmodified; //The original table, used when resetting
    //public int[,] idPPMTally; //the number of ppm values used for each sample
    //public int[,] idPPMTallyUnmodified; //the original number of ppm values, used when resetting
    public int uniqueVals = 0;

    public string[,] grid; //The full grid

    bool UTMCheckFailed = false;
    bool ElementCheckFailed = false;
    bool SampleCheckFailed = false;
    bool ChemicalCheckFailed = false;

    public List<double> chemTest = new List<double>();

    //Create a dictionary for tracking the number of times an id appears
    public Dictionary<string, int> idDictionary = new Dictionary<string, int>();
    public Dictionary<string, int> idDictionaryUnmodified;


    int fieldPos = -1;
    int materialPos = -1;
    int depositPos = -1;
    string material;

    public bool assayMode;

    public CombiController combi;
    public AssayController assay;

    public void Start()
    {
    }

    //Converts the csv file into a 2d array
    public void InitialiseGrid(bool isAssay)
    {
        Debug.Log("initing");

        //ASSAY is just _pc and Other, and is used with simplex

        grid = SplitCsvGrid(csvFile);

        int gridHeight = 1 + grid.GetUpperBound(1);

        GetElements(grid); //ppm
        GetChemicals(grid); //pc or wt%
        GetSamples(grid);
        
        Debug.Log("elements/chemicals/samples: " + ElementCheckFailed + "/" + ChemicalCheckFailed + "/" + SampleCheckFailed);

        assayMode = isAssay;

        if (!assayMode)
            combi.InitialiseCombi();
        else
            assay.InitialiseAssay();

        //Debug.Log("size = " + (1 + grid.GetUpperBound(0)) + "," + (gridHeight));

        //NorthValues = new float[gridHeight - 1];
        //EastValues = new float[gridHeight - 1];
        //ElevValues = new float[gridHeight - 1];

        /*for (int y = 0; y <= grid.GetUpperBound(1); y++)
        {
            Debug.Log("line " + y + ": ");
            DebugRow(grid, y);
        }*/
        //DebugOutputGrid(grid);
        //GetUTMCoords(grid);
        //if (UTMCheckFailed)
        //    return;
        //GetElements(grid);
        //if (ElementCheckFailed)
        //    return;
        //GetListOfMaterials(grid);
        //GetListOfDeposits(grid);
        //GetUniqueIDs(grid);
    }

    // outputs the content of a 2D array, useful for checking the importer
    static public void DebugOutputGrid(string[,] grid)
    {
        string textOutput = "";
        for (int y = 0; y <= grid.GetUpperBound(1); y++)
        {
            textOutput += y;
            textOutput += ": ";
            for (int x = 0; x <= grid.GetUpperBound(0); x++)
            {

                textOutput += grid[x, y];
                textOutput += "|";
            }
            textOutput += "\n";
        }
        Debug.Log(textOutput);
    }
    static public void DebugOutputGrid(int[,] grid)
    {
        string textOutput = "";
        for (int y = 0; y <= grid.GetUpperBound(1); y++)
        {
            textOutput += y;
            textOutput += ": ";
            for (int x = 0; x <= grid.GetUpperBound(0); x++)
            {

                textOutput += grid[x, y].ToString();
                textOutput += "|";
            }
            textOutput += "\n";
        }
        Debug.Log(textOutput);
    }
    static public void DebugOutputGrid(float[,] grid)
    {
        string textOutput = "";
        for (int y = 0; y <= grid.GetUpperBound(1); y++)
        {
            textOutput += y;
            textOutput += ": ";
            for (int x = 0; x <= grid.GetUpperBound(0); x++)
            {

                textOutput += grid[x, y].ToString("F2");
                textOutput += "|";
            }
            textOutput += "\n";
        }
        Debug.Log(textOutput);
    }

    //outputs the first column of a 2d array. Just used for a quick debugging
    static public void DebugFirstValues(string[,] grid)
    {
        string textOutput = "";
        for (int y = 0; y < grid.GetUpperBound(1); y++)
        {
            textOutput += y;
            textOutput += ": ";
            textOutput += grid[0, y];
            textOutput += "\n";
        }
        Debug.Log(textOutput);
    }

    //outputs a specific row of a 2d array
    static public void DebugRow(string[,] grid, int row)
    {
        string textOutput = "";
        for (int x = 0; x <= grid.GetUpperBound(0); x++)
        {
            textOutput += grid[x, row];
            textOutput += ", ";
        }
        Debug.Log(textOutput);
    }

    static public void DebugRow(int[,] grid, int row)
    {
        string textOutput = "";
        for (int x = 0; x <= grid.GetUpperBound(0); x++)
        {
            textOutput += grid[x, row].ToString();
            textOutput += ", ";
        }
        Debug.Log(textOutput);
    }

    static public void DebugRow(float[,] grid, int row)
    {
        string textOutput = "";
        for (int x = 0; x <= grid.GetUpperBound(0); x++)
        {
            textOutput += grid[x, row].ToString();
            textOutput += ", ";
        }
        Debug.Log(textOutput);
    }

    // splits a CSV file into a 2D string array
    static public string[,] SplitCsvGrid(string csvText)
    {
        csvText = csvText.TrimEnd('\r', '\n');
        string[] lines = csvText.Split("\n"[0]);

        // finds the max width of row
        int width = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = SplitCsvLine(lines[i]);
            width = Mathf.Max(width, row.Length);
        }

        // creates new 2D string grid to output to
        string[,] outputGrid = new string[width, lines.Length];
        for (int y = 0; y < lines.Length; y++)
        {
            string[] row = SplitCsvLine(lines[y]);
            for (int x = 0; x < row.Length; x++)
            {
                outputGrid[x, y] = row[x];

                // This line was to replace "" with " in my output. 
                // Include or edit it as you wish.
                outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
            }
        }

        return outputGrid;
    }

    // splits a CSV row 
    static public string[] SplitCsvLine(string line)
    {
        return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
        @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
        System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                select m.Groups[1].Value).ToArray();
    }







    //======================================================================================================================================================================================================================================================================


    public void GetSamples(string[,] grid)
    {
        for (int y = 1; y <= grid.GetUpperBound(1); y++)
        {
            //Debug.Log("testing for " + y);
            if (grid[0, y] != "" && grid[0,y] != null)
            {
                //Debug.Log("adding " + grid[0, y]);
                sampleList.Add(grid[0, y]);
                samplePositions.Add(y);
                sampleCount++;
            }
        }
        if (sampleCount == 0)
        {
            SampleCheckFailed = true;
            //fileImport.DisplayErrorMessage("Count not find element columns\n\nElement column headings must end with '_ppm'");
        }
    }
    public void GetElements(string[,] grid)
    {
        for (int x = 0; x <= grid.GetUpperBound(0); x++)
        {
            if (ContainsNoCase(grid[x, 0], "_ppm"))
            {
                elementList.Add(grid[x, 0]);
                elementPositions.Add(x);
                elementCount++;
            }
        }
        if (elementCount == 0)
        {
            ElementCheckFailed = true;
            //fileImport.DisplayErrorMessage("Count not find element columns\n\nElement column headings must end with '_ppm'");
        }
    }
    public void GetChemicals(string[,] grid)
    {
        for (int x = 0; x <= grid.GetUpperBound(0); x++)
        {
            if (ContainsNoCase(grid[x, 0], "wt%") || ContainsNoCase(grid[x, 0], "_pc"))
            {
                chemicalList.Add(grid[x, 0]);
                chemicalPositions.Add(x);
                chemicalCount++;
                //chemTest.Add(Double.Parse(grid[x, 1]));
            }
        }
        if (chemicalCount == 0)
        {
            ChemicalCheckFailed = true;
            //fileImport.DisplayErrorMessage("Count not find element columns\n\nElement column headings must end with '_ppm'");
        }
    }
    public List<double> GetChemTestList(string[,] grid, int row)
    {
        chemTest.Clear();
        for (int x = 0; x <= grid.GetUpperBound(0); x++)
        {
            //Debug.Log("[" + x + "," + (row ) + "] = " + grid[x, row]);
            if (ContainsNoCase(grid[x, 0], "wt%") || ContainsNoCase(grid[x, 0], "_pc"))
            {
                chemTest.Add((Double.Parse(grid[x, row]))*100);
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
            //Debug.Log(d);
        }
        return chemTest;

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