using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{

    [SerializeField]
    private GameObject[] crosshair = new GameObject[4];

    private RectTransform[] crosshairLines = new RectTransform[4];

    [SerializeField] private int crosshairLength;
    [SerializeField] private int crosshairWidth;
    // [SerializeField] private int centerGap;
    [SerializeField] private Color crosshairColor;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++) {
            crosshairLines[i] = crosshair[i].GetComponent<RectTransform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            crosshairLines[i].sizeDelta = new Vector2(crosshairWidth, crosshairLength);
            crosshair[i].GetComponent<Image>().color = crosshairColor;
        }

    }
}
