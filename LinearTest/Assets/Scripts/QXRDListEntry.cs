using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class QXRDListEntry : MonoBehaviour
{

    public string MineralComp;
    public Text label;
    public InputField inputField;
    public int index;
    public CombiMineralElementListEntry CMELE;
    public Dropdown dropdown;


    // Use this for initialization
    void Start()
    {
        MineralComp = label.text;
        index = this.transform.GetSiblingIndex();

        //dropdown.OnSelect();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        //text = GUI.TextField(inputField, text);
        //inputField.text = Regex.Replace(inputField.text, @"[^0-9.]", "");
    }

    public void SetLabel(string text)
    {
        label.text = text;
        MineralComp = text;
    }

    public void AddToDropdown(List<string> m_DropOptions)
    {
        dropdown.AddOptions(m_DropOptions);
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
