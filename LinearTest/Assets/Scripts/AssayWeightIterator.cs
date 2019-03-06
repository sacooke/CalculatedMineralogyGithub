using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class AssayWeightIterator : MonoBehaviour {

    public alglib alg;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }



    public IEnumerator EnumerateAssayWeightIterator(bool toCSV)
    {

        string fullExportString = "";
        string columnNames = "Sample No.,Score";


        List<string> minList = new List<string>();
        List<double> weightList = new List<double>();

        List<string> elementList = new List<string>();
        List<int> columnIndexList = new List<int>();
        double result;

        //Compile list of mineral compositions to be used in the calculations from the scrollviews of minerals and elements
        /*foreach (Toggle tog in bothController.mineralCompScrollView.transform.GetComponentsInChildren<Toggle>())
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

                /*double[] cn = new double[columnIndexList.Count + 1];//csvController.chemTest.ToArray();
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
                /*alglib.minlpsetlc2(state, simplexArray, cn, cn); //al, au
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

                /*double endProduct = SumProduct(c, resultArray);

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
        bothController.sharedMenu.SetActive(true);*/
        yield return null;
    }


}
