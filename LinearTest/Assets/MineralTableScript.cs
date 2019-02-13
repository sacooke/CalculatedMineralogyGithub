using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class MineralTableScript : MonoBehaviour {

    public MineralTableController controller;

    public InputField AgField;
    public InputField AlField;
    public InputField AsField;
    public InputField AuField;
    public InputField BaField;
    public InputField CaField;
    public InputField CuField;
    public InputField FeField;
    public InputField KField;
    public InputField MgField;
    public InputField MnField;
    public InputField MoField;
    public InputField NaField;
    public InputField PField;
    public InputField PbField;
    public InputField SField;
    public InputField TeField;
    public InputField TiField;
    public InputField UField;
    public InputField ZnField;
    public InputField ZrField;
    public InputField AssayWeightField;
    public InputField WLSStartField;

    public Text OtherText;
    public InputField MineralCompField;

    public List<InputField> allFields;

    Color darkGreen = new Color(0f, 171 / 255f, 44 / 255f);

    // Use this for initialization
    void Start ()
    {
        allFields.Add(AgField);
        allFields.Add(AlField);
        allFields.Add(AsField);
        allFields.Add(AuField);
        allFields.Add(BaField);
        allFields.Add(CaField);
        allFields.Add(CuField);
        allFields.Add(FeField);
        allFields.Add(KField);
        allFields.Add(MgField);
        allFields.Add(MnField);
        allFields.Add(MoField);
        allFields.Add(NaField);
        allFields.Add(PField);
        allFields.Add(PbField);
        allFields.Add(SField);
        allFields.Add(TeField);
        allFields.Add(TiField);
        allFields.Add(UField);
        allFields.Add(ZnField);
        allFields.Add(ZrField);

        foreach(InputField inputField in allFields)
        {
            inputField.onEndEdit.AddListener(delegate { calcOther(); });
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void calcOther()
    {
        double otherVal = 100;
        bool isValid = true;
        double temp;
        foreach(InputField inputField in allFields)
        {
            //Debug.Log(inputField.gameObject.name + ": " + inputField.text);
            if (double.TryParse(inputField.text, out temp))
            {
                if (temp < 0)
                {
                    inputField.text = "0.0";
                    temp = 0;
                }
                if (temp > 100)
                {
                    inputField.text = "100.0";
                    temp = 100;
                }
                otherVal -= temp;
            }
            else if (inputField.text == "")
                otherVal -= 0;
            else
            {
                isValid = false;
                break;
            }
        }
        if(isValid)
        {
            OtherText.text = otherVal.ToString("F2");
            if (otherVal >= 0)
                OtherText.color = darkGreen;
            else
                OtherText.color = Color.red;
        }
        else
        {
            OtherText.text = "???";
            OtherText.color = Color.red;

        }
    }

    public void DeleteThis()
    {
        controller.DestroyTableEntry(this);
        Destroy(this.gameObject);
    }
}
