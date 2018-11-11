using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MineralCompositionListEntry : MonoBehaviour {

    public string MineralComp;
    public Text label;
    public int index;

	// Use this for initialization
	void Start () {
        MineralComp = label.text;
        index = this.transform.GetSiblingIndex();
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
