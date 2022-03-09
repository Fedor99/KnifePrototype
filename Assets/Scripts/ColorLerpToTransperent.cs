using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ColorLerpToTransperent : MonoBehaviour
{
    private Image image;
    public Color targetColor = Color.clear;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        image.color = Color.Lerp(image.color, targetColor, 2f * Time.deltaTime);
    }
}
