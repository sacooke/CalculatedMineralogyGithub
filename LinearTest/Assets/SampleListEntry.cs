using UnityEngine;
using System.Collections;

public class SampleListEntry : MonoBehaviour {

    public string SampleID;
    public int index;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
