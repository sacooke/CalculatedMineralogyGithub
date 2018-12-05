using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CombiMineralElementListEntry : MonoBehaviour {

    public string SampleID;
    public int index;
    public Toggle ElementToggle;
    public Toggle MineralToggle;
    public Text label;

    public double ElementAbsoluteError;
    public double ElementRelativeError;

    public double MineralAbsoluteError;
    public double MineralRelativeError;

    // Use this for initialization
    void Start ()
    {
        ElementAbsoluteError = double.NegativeInfinity;
        ElementRelativeError = double.NegativeInfinity;

        MineralAbsoluteError = double.NegativeInfinity;
        MineralRelativeError = double.NegativeInfinity;

}
	
	// Update is called once per frame
	void Update () {

    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
