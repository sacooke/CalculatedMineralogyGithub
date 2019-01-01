using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QXRDListSizeAdjuster : MonoBehaviour {

    public GridLayoutGroup glg;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnGUI()
    {
        Debug.Log("setting to " + (this.GetComponent<RectTransform>().sizeDelta.x / 2));
        glg.cellSize.Set(this.GetComponent<RectTransform>().sizeDelta.x / 2, 20);
    }
}
