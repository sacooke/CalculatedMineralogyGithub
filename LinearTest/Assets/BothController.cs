using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class BothController : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject sharedMenu;
    public GameObject assayMenu;
    public GameObject combiMenu;
    public CSVReader csvController;
    
    public AssayController assayController;

    public FileChooser fileChooser;
    private string fileContentString;
    public alglib alg;
    public Export export;

    public GameObject samplePrefab;
    public GameObject sampleScrollView;
    public GameObject columnScrollView;
    public GameObject mineralCompScrollView;
    public GameObject elementCompScrollView;

    public GameObject mineralCompListPrefab;

    public List<string> samples = new List<string>();
    public List<string> columns = new List<string>();
    public List<Toggle> sampleToggles = new List<Toggle>();
    public List<Toggle> columnToggles = new List<Toggle>();
    public List<Toggle> mineralToggles = new List<Toggle>();
    public List<Toggle> elementToggles = new List<Toggle>();

    public InputField setAllMineralWeightsInputField;
    public InputField setAllElementWeightsInputField;

    // Use this for initialization
    void Start()
    {
        
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
