using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class MineralTableController : MonoBehaviour {

    public FileChooser fileChooser;
    public GameObject TableEntryPrefab;
    public Transform Content;
    public List<MineralTableScript> mineralTableList;
    public InputField nameField;
    public Export export;
    public GameObject CSVOverwritePanel;
    public GameObject CSVFilenamePanel;
    public GameObject tableMenu;
    public GameObject tableErrorPanel;
    public Text tableErrorText;
    public GameObject mainMenu;

    public InputField CSVFilenameInputField;

    public Text pathText;

    string tempFilename;
    string path;
    private string fileContentString;
    string[,] importedDatasetGrid = { { "null" } };

    // Use this for initialization
    void Start () {

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
        Debug.Log("path = " + path);

        pathText.text = path;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PressAddMTSButton()
    {
        CreateTableEntry();
    }

    public MineralTableScript CreateTableEntry()
    {
        GameObject g = GameObject.Instantiate(TableEntryPrefab);
        g.transform.SetParent(Content);
        MineralTableScript mts = g.GetComponent<MineralTableScript>();
        mineralTableList.Add(mts);
        g.transform.localScale = new Vector3(1, 1, 1);
        mts.controller = this;
        return mts;
    }

    public void DestroyTableEntry(MineralTableScript mts)
    {
        mineralTableList.Remove(mts);
    }

    public void DestroyAllListEntries()
    {
        
        for (int i = mineralTableList.Count - 1; i >= 0; i--)
        {
            MineralTableScript mts = mineralTableList[i];
            DestroyTableEntry(mineralTableList[i]);

            mts.DeleteThis();
        }
        nameField.text = "";
        CSVFilenameInputField.text = "";

    }

    public void ReturnToMainMenu()
    {
        DestroyAllListEntries();
        tableMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OpenTableMenu()
    {

        tableMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void StartCSV()
    {
        if (nameField.text == "")
        {
            Debug.Log("Please enter a name");
            CSVFilenamePanel.SetActive(true);
            tableMenu.SetActive(false);
        }
        else
        {
            string filename = path + nameField.text + ".csv";
            if (!File.Exists(filename))
            {
                CreateCSV(filename);
            }
            else
            {
                tempFilename = filename;
                CSVOverwritePanel.SetActive(true);
                tableMenu.SetActive(false);
                Debug.Log("overwrite?");
            }
        }

    }

    public void CSVSaveAsFilename(bool saveAs)
    {
        if (saveAs)
        {
            if (CSVFilenameInputField.text != "")
            {

                CSVFilenamePanel.SetActive(false);

                string filename = path + CSVFilenameInputField.text + ".csv";
                if (!File.Exists(filename))
                {
                    CreateCSV(filename);
                    tableMenu.SetActive(true);
                }
                else
                {
                    tempFilename = filename;
                    CSVOverwritePanel.SetActive(true);
                    Debug.Log("overwrite?");
                }

            }
        }
        else
        {
            CSVFilenamePanel.SetActive(false);
            tableMenu.SetActive(true);
        }
    }

    public void CSVOverwrite(bool overwrite)
    {
        CSVOverwritePanel.SetActive(false);
        tableMenu.SetActive(true);
        if (overwrite)
        {
            CreateCSV(tempFilename);
            /*
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(tempFilename))
            {
                sw.Write(sb.ToString());
            }*/
        }

    }

    public void CreateCSV(string filename)
    {

        export.StartNewStringbuilder();
        export.WriteStringToStringBuilder("Mineral Composition,Ag,Al,As,Au,Ba,Ca,Cu,Fe,K,Mg,Mn,Mo,Na,P,Pb,S,Te,Ti,U,Zn,Zr,Assay Weight,WLS Starting Point");
        foreach(MineralTableScript mts in mineralTableList)
        {
            string outputLine = "";
            outputLine += (mts.MineralCompField.text + ",");
            if (mts.AgField.text != "")
                outputLine += (mts.AgField.text + ",");
            else
                outputLine += "0,";
            if (mts.AlField.text != "")
                outputLine += (mts.AlField.text + ",");
            else
                outputLine += "0,";
            if (mts.AsField.text != "")
                outputLine += (mts.AsField.text + ",");
            else
                outputLine += "0,";
            if (mts.AuField.text != "")
                outputLine += (mts.AuField.text + ",");
            else
                outputLine += "0,";
            if (mts.BaField.text != "")
                outputLine += (mts.BaField.text + ",");
            else
                outputLine += "0,";
            if (mts.CaField.text != "")
                outputLine += (mts.CaField.text + ",");
            else
                outputLine += "0,";
            if (mts.CuField.text != "")
                outputLine += (mts.CuField.text + ",");
            else
                outputLine += "0,";
            if (mts.FeField.text != "")
                outputLine += (mts.FeField.text + ",");
            else
                outputLine += "0,";
            if (mts.KField.text != "")
                outputLine += (mts.KField.text + ",");
            else
                outputLine += "0,";
            if (mts.MgField.text != "")
                outputLine += (mts.MgField.text + ",");
            else
                outputLine += "0,";
            if (mts.MnField.text != "")
                outputLine += (mts.MnField.text + ",");
            else
                outputLine += "0,";
            if (mts.MoField.text != "")
                outputLine += (mts.MoField.text + ",");
            else
                outputLine += "0,";
            if (mts.NaField.text != "")
                outputLine += (mts.NaField.text + ",");
            else
                outputLine += "0,";
            if (mts.PField.text != "")
                outputLine += (mts.PField.text + ",");
            else
                outputLine += "0,";
            if (mts.PbField.text != "")
                outputLine += (mts.PbField.text + ",");
            else
                outputLine += "0,";
            if (mts.SField.text != "")
                outputLine += (mts.SField.text + ",");
            else
                outputLine += "0,";
            if (mts.TeField.text != "")
                outputLine += (mts.TeField.text + ",");
            else
                outputLine += "0,";
            if (mts.TiField.text != "")
                outputLine += (mts.TiField.text + ",");
            else
                outputLine += "0,";
            if (mts.UField.text != "")
                outputLine += (mts.UField.text + ",");
            else
                outputLine += "0,";
            if (mts.ZnField.text != "")
                outputLine += (mts.ZnField.text + ",");
            else
                outputLine += "0,";
            if (mts.ZrField.text != "")
                outputLine += (mts.ZrField.text + ",");
            else
                outputLine += "0,";
            if (mts.AssayWeightField.text != "")
                outputLine += (mts.AssayWeightField.text + ",");
            else
                outputLine += "0,";
            if (mts.WLSStartField.text != "")
                outputLine += (mts.WLSStartField.text);
            else
                outputLine += "0";
            Debug.Log(outputLine);
            export.WriteStringToStringBuilder(outputLine);
        }
        Debug.Log("filename: " + filename);
        export.WriteToFile(filename);
        /*using (StreamWriter sw = File.CreateText(filename))
        {
            sw.Write(export.sb.ToString());
        }*/

    }


    public void OpenFileDialog()
    {
        fileChooser.setup(FileChooser.OPENSAVE.OPEN, "csv");
        fileChooser.callbackYes = delegate (string filename) {
            fileChooser.gameObject.SetActive(false);
            if (File.Exists(filename))
            {
                LoadDatasetCSV(filename);

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

    public void LoadDatasetCSV(string filename)
    {
        StartCoroutine(ImportSamples(filename));

    }


    public void VerifyCSV(string filename)
    {
        //export.SetFilenameString(filename);
        Debug.Log("upper bound length = " + importedDatasetGrid.GetUpperBound(0));
        if(importedDatasetGrid.GetUpperBound(0) != 24)
        {
            ShowTableError("csv file does not contain the correct amount of columns");
            return;
        }

        if (importedDatasetGrid[0, 0] != "Mineral Composition")
        {
            ShowTableError("csv file does not contain a 'Mineral Composition' column");
            return;
        }
        if (importedDatasetGrid[1, 0] != "Ag")
        {
            ShowTableError("csv file does not contain a 'Ag' column");
            return;
        }
        if (importedDatasetGrid[2, 0] != "Al")
        {
            ShowTableError("csv file does not contain a 'Al' column");
            return;
        }
        if (importedDatasetGrid[3, 0] != "As")
        {
            ShowTableError("csv file does not contain a 'As' column");
            return;
        }
        if (importedDatasetGrid[4, 0] != "Au")
        {
            ShowTableError("csv file does not contain a 'Au' column");
            return;
        }
        if (importedDatasetGrid[5, 0] != "Ba")
        {
            ShowTableError("csv file does not contain a 'Ba' column");
            return;
        }
        if (importedDatasetGrid[6, 0] != "Ca")
        {
            ShowTableError("csv file does not contain a 'Ca' column");
            return;
        }
        if (importedDatasetGrid[7, 0] != "Cu")
        {
            ShowTableError("csv file does not contain a 'Cu' column");
            return;
        }
        if (importedDatasetGrid[8, 0] != "Fe")
        {
            ShowTableError("csv file does not contain a 'Fe' column");
            return;
        }
        if (importedDatasetGrid[9, 0] != "K")
        {
            ShowTableError("csv file does not contain a 'K' column");
            return;
        }
        if (importedDatasetGrid[10, 0] != "Mg")
        {
            ShowTableError("csv file does not contain a 'Mg' column");
            return;
        }
        if (importedDatasetGrid[11, 0] != "Mn")
        {
            ShowTableError("csv file does not contain a 'Mn' column");
            return;
        }
        if (importedDatasetGrid[12, 0] != "Mo")
        {
            ShowTableError("csv file does not contain a 'Mo' column");
            return;
        }
        if (importedDatasetGrid[13, 0] != "Na")
        {
            ShowTableError("csv file does not contain a 'Na' column");
            return;
        }
        if (importedDatasetGrid[14, 0] != "P")
        {
            ShowTableError("csv file does not contain a 'P' column");
            return;
        }
        if (importedDatasetGrid[15, 0] != "Pb")
        {
            ShowTableError("csv file does not contain a 'Pb' column");
            return;
        }
        if (importedDatasetGrid[16, 0] != "S")
        {
            ShowTableError("csv file does not contain a 'S' column");
            return;
        }
        if (importedDatasetGrid[17, 0] != "Te")
        {
            ShowTableError("csv file does not contain a 'Te' column");
            return;
        }
        if (importedDatasetGrid[18, 0] != "Ti")
        {
            ShowTableError("csv file does not contain a 'Ti' column");
            return;
        }
        if (importedDatasetGrid[19, 0] != "U")
        {
            ShowTableError("csv file does not contain a 'U' column");
            return;
        }
        if (importedDatasetGrid[20, 0] != "Zn")
        {
            ShowTableError("csv file does not contain a 'Zn' column");
            return;
        }
        if (importedDatasetGrid[21, 0] != "Zr")
        {
            ShowTableError("csv file does not contain a 'Zr' column");
            return;
        }
        if (importedDatasetGrid[22, 0] != "Assay Weight")
        {
            ShowTableError("csv file does not contain a 'Assay Weight' column");
            return;
        }
        if (importedDatasetGrid[23, 0] != "WLS Starting Point")
        {
            ShowTableError("csv file does not contain a 'WLS Starting Point' column");
            return;
        }
        /*
        for (int i = 0; i < importedDatasetGrid.GetUpperBound(0); i++)
        {
            Debug.Log("upperbound " + i + " = " + importedDatasetGrid[i, 0]);

        }*/
        DestroyAllListEntries();
        string csvName = filename.Substring(filename.LastIndexOf('\\') + 1, filename.Length - filename.LastIndexOf('\\') - 1 - 4);
        Debug.Log("CSVNAME = " + csvName);
        nameField.text = csvName;
        for (int i = 1; i <= importedDatasetGrid.GetUpperBound(1); i++)
        {
            MineralTableScript mts = CreateTableEntry();
            mts.AddAllFields();
            mts.MineralCompField.text = importedDatasetGrid[0, i];
            int j = 1;
            foreach(InputField field in mts.allFields)
            {
                field.text = importedDatasetGrid[j, i];
                j++;
            }
            mts.AssayWeightField.text = importedDatasetGrid[j, i];
            mts.WLSStartField.text = importedDatasetGrid[j+1, i];
            mts.calcOther();
        }

    }
    private IEnumerator ImportSamples(string url)
    {
        //FilenameText.text = url.Substring(url.LastIndexOf("\\") + 1);
        yield return StartCoroutine(FastDownload(url, fileContents => fileContentString = fileContents));
        if (fileContentString != null && fileContentString.Length > 0 && fileContentString != "null")
        {
            importedDatasetGrid = CSVReader.SplitCsvGrid(fileContentString);
        }
        else
        {
            Debug.Log("Error: File Content is null");
            //DisplayErrorMessage(errorMessage);
        }

        yield return null;
        VerifyCSV(url);

    }



    private IEnumerator FastDownload(string url, System.Action<string> result)
    {
        string s = "null";
        try
        {
            //s = File.ReadAllText(url).TrimEnd('\r', '\n');
            foreach (string b in File.ReadAllLines(url))
                if (Regex.IsMatch(b, "[^,]"))
                    s += b + "\n";

        }
        catch (IOException e)
        {
            Debug.Log("<color=red>Error: " + e.GetType().Name + ": " + e.Message + "</color>");
            //errorMessage = "Error! " + e.GetType().Name + ": " + e.Message;
        }
        yield return null;
        result(s);
    }

    public void ShowTableError(string error)
    {
        tableErrorPanel.SetActive(true);
        tableMenu.SetActive(false);
        tableErrorText.text = error;
    }

    public void CloseTableError()
    {
        tableErrorPanel.SetActive(false);
        tableMenu.SetActive(true);

        tableErrorText.text = "";
    }
}
