using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MineralTableController : MonoBehaviour {

    public GameObject TableEntryPrefab;
    public Transform Content;
    public List<MineralTableScript> mineralTableList;
    public InputField nameField;
    public Export export;
    public GameObject CSVOverwritePanel;
    public GameObject CSVFilenamePanel;
    public GameObject tableMenu;
    public GameObject mainMenu;

    public InputField CSVFilenameInputField;

    public Text pathText;

    string tempFilename;
    string path;

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

    public void CreateTableEntry()
    {
        GameObject g = GameObject.Instantiate(TableEntryPrefab);
        g.transform.SetParent(Content);
        mineralTableList.Add(g.GetComponent<MineralTableScript>());
        g.transform.localScale = new Vector3(1, 1, 1);
        g.GetComponent<MineralTableScript>().controller = this;
    }

    public void DestroyTableEntry(MineralTableScript mts)
    {
        mineralTableList.Remove(mts);
    }

    public void ReturnToMainMenu()
    {
        foreach(MineralTableScript mts in mineralTableList)
        {
            DestroyTableEntry(mts);
            nameField.text = "";
            CSVFilenameInputField.text = "";
        }
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
                outputLine += (mts.ZrField.text);
            else
                outputLine += "0";
            if (mts.AssayWeightField.text != "")
                outputLine += (mts.AssayWeightField.text);
            else
                outputLine += "0";
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
}
