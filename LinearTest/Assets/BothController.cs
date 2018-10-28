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

    public FileChooser fileChooser;
    private string fileContentString;
    public alglib alg;
    public Export export;

    public GameObject samplePrefab;
    public GameObject sampleScrollView;
    public GameObject columnScrollView;

    public List<string> samples = new List<string>();
    public List<string> columns = new List<string>();
    public List<Toggle> sampleToggles = new List<Toggle>();
    public List<Toggle> columnToggles = new List<Toggle>();

    // Use this for initialization
    void Start () {
	
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

            }
            //int samplePos = csvController.samplePositions[i];

            //sampleToggles.Add(g.GetComponent<Toggle>());
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
