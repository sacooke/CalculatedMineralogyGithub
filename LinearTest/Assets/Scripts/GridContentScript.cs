using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GridContentScript : MonoBehaviour {

    public int rows;
    public int cols;
    public GameObject inputFieldPrefab;

    void Start()
    {
        RectTransform parentRect = gameObject.GetComponent<RectTransform>();
        GridLayoutGroup gridLayout = gameObject.GetComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(parentRect.rect.width / cols, parentRect.rect.height / rows);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                GameObject inputField = Instantiate(inputFieldPrefab);
                inputField.transform.SetParent(gameObject.transform, false);
            }
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
