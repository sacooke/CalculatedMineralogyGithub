using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

public class ErrorListEntry : MonoBehaviour
{
    
    public string MineralComp;
    public Text label;
    public InputField inputField;
    public int index;
    public CombiMineralElementListEntry CMELE;
    public int errorType = -1; //0 = elementRelativeError, 1 = elementAbsoluteError, 2 = mineralRelativeError, 3 = mineralAbsoluteError


    // Use this for initialization
    void Start()
    {
        MineralComp = label.text;
        index = this.transform.GetSiblingIndex();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        //text = GUI.TextField(inputField, text);
        inputField.text = Regex.Replace(inputField.text, @"[^0-9.]", "");
    }

    public void SetInputFieldValue(string newText)
    {
        inputField.text = newText;
    }
    public string GetInputFieldValue()
    {
        return inputField.text;
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
