using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.IO;


/*  Code by Alex Grace (2015), Jennifer Ralph (2016), and Stephen Cooke (2017)
    Export.cs allows the user to create a .csv file containing the data relating to the measurements in the sample list,
	as well as create a .dxf file with the basic geometry of points, planes, and lineations within the scene.
    It also contains location data about the original .kmz file, including latitude, longitude and UTM coordinates
*/

public class Export : MonoBehaviour
{
    public GameObject CombiOverwritePanel;
    public GameObject AssayOverwritePanel;
    public GameObject TrainingOverwritePanel;

    public FileChooser fileChooser;
    public static string fileName;

    public string csvName = "null";
    public StringBuilder sb;

    public string tempFilename;

    public CombiController combiController;
    public AssayController assayController;
    public BothController bothController;
    public TrainingController trainingController;


    bool tempToCsv;
    int tempOption;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ExportFile(string exportType)
    {
        OpenFileDialog(exportType);
    }

    //From filechooser asset
    public void OpenFileDialog(string format)
    {
        fileChooser.setup(FileChooser.OPENSAVE.SAVE, format);
        fileChooser.callbackYes = delegate (string filename) {
            fileChooser.gameObject.SetActive(false);
            if (format == "txt")
                ExportDataToTXTCombi(filename);
            else if (format == "csv")
                ExportDataToCSVCombi(filename);
        };
        fileChooser.callbackNo = delegate () {
            fileChooser.gameObject.SetActive(false);
        };
    }

    public void OpenFileDialogAssay(string format)
    {
        fileChooser.setup(FileChooser.OPENSAVE.SAVE, format);
        fileChooser.callbackYes = delegate (string filename) {
            fileChooser.gameObject.SetActive(false);
            if (format == "txt")
                ExportDataToTXTAssay(filename);
            else if (format == "csv")
                ExportDataToCSVAssay(filename);
        };
        fileChooser.callbackNo = delegate () {
            fileChooser.gameObject.SetActive(false);
        };
    }

    public void OpenFileDialogTraining(int option)
    {
        //0 = weights
        //1 = just assay
        //2 = assay and combi
        fileChooser.setup(FileChooser.OPENSAVE.SAVE, "csv");
        fileChooser.callbackYes = delegate (string filename) {
            fileChooser.gameObject.SetActive(false);
            ExportDataToCSVTraining(filename, option);
        };
        fileChooser.callbackNo = delegate () {
            fileChooser.gameObject.SetActive(false);
        };
    }

    public void ExportDataToTXTCombi(string filename)
    {
        if (!File.Exists(filename))
        {
            tempFilename = filename;
            StartNewStringbuilder();
            sb.AppendLine("Export from \"" + csvName + "\" created on " + System.DateTime.Now.ToString("dd/MM/yyyy") + " at " + System.DateTime.Now.ToString("hh:mmtt"));
            sb.AppendLine("");
            combiController.CalculateCombi(false);
            /*
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(filename))
            {
                sw.Write(sb.ToString());
            }*/
        }
        else
        {
            tempFilename = filename;
            tempToCsv = false;
            CombiOverwritePanel.SetActive(true);
            bothController.QXRDMenu.SetActive(false);
            Debug.Log("overwrite?");
        }
    }

    public void ExportDataToTXTAssay(string filename)
    {
        if (!File.Exists(filename))
        {
            tempFilename = filename;
            StartNewStringbuilder();
            sb.AppendLine("Export from \"" + csvName + "\" created on " + System.DateTime.Now.ToString("dd/MM/yyyy") + " at " + System.DateTime.Now.ToString("hh:mmtt"));
            sb.AppendLine("");
            assayController.CalculateAssay(false);
            /*
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(filename))
            {
                sw.Write(sb.ToString());
            }*/
        }
        else
        {
            tempFilename = filename;
            tempToCsv = false;
            AssayOverwritePanel.SetActive(true);
            bothController.sharedMenu.SetActive(false);
            Debug.Log("overwrite?");
        }
    }

    public void CombiOverwrite(bool overwrite)
    {
        CombiOverwritePanel.SetActive(false);
        bothController.QXRDMenu.SetActive(true);
        if (overwrite)
        {
            StartNewStringbuilder();
            combiController.CalculateCombi(tempToCsv);
            /*
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(tempFilename))
            {
                sw.Write(sb.ToString());
            }*/
        }

    }
    public void AssayOverwrite(bool overwrite)
    {
        AssayOverwritePanel.SetActive(false);
        bothController.sharedMenu.SetActive(true);
        if (overwrite)
        {
            StartNewStringbuilder();
            assayController.CalculateAssay(tempToCsv);
            /*
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(tempFilename))
            {
                sw.Write(sb.ToString());
            }*/
        }

    }
    public void TrainingOverwrite(bool overwrite)
    {
        TrainingOverwritePanel.SetActive(false);
        trainingController.trainingSubmenu3.SetActive(true);
        if (overwrite)
        {
            trainingController.ExportTrainingData(tempFilename, tempOption);
        }

    }

    public void ExportDataToCSVCombi(string filename)
    {
        if (!File.Exists(filename))
        {
            tempFilename = filename;
            StartNewStringbuilder();
            combiController.CalculateCombi(true);
            /*
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(filename))
            {
                sw.Write(sb.ToString());
            }*/
        }
        else
        {
            tempFilename = filename;
            tempToCsv = true;
            CombiOverwritePanel.SetActive(true);
            bothController.QXRDMenu.SetActive(false);
            Debug.Log("overwrite?");
        }
    }

    public void ExportDataToCSVAssay(string filename)
    {
        if (!File.Exists(filename))
        {
            tempFilename = filename;
            StartNewStringbuilder();
            assayController.CalculateAssay(true);

            // Create a file to write to.
            /*using (StreamWriter sw = File.CreateText(filename))
            {
                sw.Write(sb.ToString());
            }*/
        }
        else
        {
            tempFilename = filename;
            tempToCsv = true;
            AssayOverwritePanel.SetActive(true);
            bothController.sharedMenu.SetActive(false);
            Debug.Log("overwrite?");
        }
    }
    public void ExportDataToCSVTraining(string filename, int option)
    {
        if (!File.Exists(filename))
        {
            Debug.Log("got this far");
            trainingController.ExportTrainingData(filename, option);
        }
        else
        {
            tempFilename = filename;
            tempOption = option;
            TrainingOverwritePanel.SetActive(true);
            trainingController.trainingMenu.SetActive(false);
            Debug.Log("overwrite?");
        }
    }

    public void SetFilenameString(string file)
    {
        csvName = file;
    }

    public void StartNewStringbuilder()
    {
        sb = new StringBuilder();
    }

    public void WriteStringToStringBuilder(string line)
    {
        sb.AppendLine(line);
    }

    /*void ExportDataToTXT(string filename)
    {
        //this method converts relevant scene information into the 'csv' file format
        
        sb.AppendLine("Export from \"" + csvName + "\" created on " + System.DateTime.Now.ToString("dd/MM/yyyy")
            + " at " + System.DateTime.Now.ToString("hh:mmtt"));
        sb.AppendLine("");
        
        if (elementDenom != "null")
        {
            sb.AppendLine("Ratio: " + elementNum + "/" + elementDenom);
        }
        else
        {
            sb.AppendLine("Element: " + elementNum);
        }

        sb.AppendLine("Scalar constant (a): " + scalarConstant);
        sb.AppendLine("Exponent constant (b): " + exponentConstant);
        sb.AppendLine("");
        sb.AppendLine("");


        foreach (SampleButton sample in dataPoints)
        {
            sb.AppendLine("Field Sample ID: " + sample.idText.text);
            sb.AppendLine("Radius: " + sample.radius + "m");
            sb.AppendLine("");

        }

    }*/

    public void WriteToFile(string filename)
    {
        if (true)//!File.Exists(filename))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(filename))
            {
                sw.Write(sb.ToString());
            }

            //statusText.text = "Successfully exported!";
        }
        else
        {
            //statusText.text = "Error: File already exists.";
        }

    }

    /*void ExportDataToCSV(string filename)
    {
        //this method converts relevant scene information into the 'csv' file format

        SampleButton[] dataPoints = ListOfSamples.GetComponentsInChildren<SampleButton>();
        StringBuilder sb = new StringBuilder();
        sb.Append("Export from \"" + fileName + "\" created on " + System.DateTime.Now.ToString("dd/MM/yyyy")
            + " at " + System.DateTime.Now.ToString("hh:mmtt") + "\n");
        sb.Append("longitude,latitude,altitude\n"); //altitudeMode(https://support.google.com/earth/answer/148078?hl=en)\n");
        sb.Append(gps.longitude + "," + gps.latitude + "," + gps.altitude + "\n"); //+ gps.altitudeMode + "\n");
        sb.Append("UTM coordinates\neasting,northing,zone\n");
        double north;
        double east;
        string zone;
        LatLongToUTM.ConvertLatLongToUTM(gps.longitude, gps.latitude, out north, out east, out zone);
        sb.Append(east + "," + north + "," + zone + "\n");

        sb.Append("dip/plunge,dipDirection/trend,featureType,sampleType,text,easting,northing, altitudeOffset(m)\n");

        foreach (SampleButton sample in dataPoints)
        {
            Vector3 samplePos = sample.rayCastPoint.transform.position;
            samplePos += gps.modelOffset;
            samplePos = gps.modelScale * samplePos;
            string p = sample.probeType.ToString();
            if (p == "MULTILINE")
            {
                sb.Append("" +
                "," + "" +
                "," + "" +
                "," + sample.probeType.ToString() +
                "," + "" +
                "," + ((-1) * samplePos.x + east) +
                "," + ((-1) * samplePos.z + north) +
                "," + samplePos.y +
                "\n");
            }
            //if sample is a label, ruler, or hotspot, it should not be included in the export file 
            else if (p == "TEXTLABEL" || p == "RULER" || p == "HOTSPOT")
            {
                //do nothing
            }

            else
            {
                sb.Append(Mathf.RoundToInt(sample.GetDip()) +
                "," + Mathf.RoundToInt(sample.GetDipDirection()) +
                "," + sample.GetType() +
                "," + sample.probeType.ToString() + "," +
                "," + ((-1) * samplePos.x + east) +
                "," + ((-1) * samplePos.z + north) +
                "," + samplePos.y +
                "\n");
            }
        }

        if (!File.Exists(filename))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(filename))
            {
                sw.Write(sb.ToString());
            }

            statusText.text = "Successfully exported!";
        }
        else
        {
            statusText.text = "Error: File already exists.";
        }

    }*/

}
