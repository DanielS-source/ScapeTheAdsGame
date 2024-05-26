using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CompassController : MonoBehaviour
{
    public float compassSmooth = 0.5f;
    public TextMeshProUGUI debugText;
    // Use this for initialization
    void Start()
    {
        // If you need an accurate heading to true north, 
        // start the location service so Unity can correct for local deviations:
        Input.location.Start();
        // Start the compass.
        Input.compass.enabled = true;
    }

    // Update is called once per frame
    private void Update()
    {
        //do color change based on compass
        float currentMagneticHeading = (float)System.Math.Round(Input.compass.magneticHeading, 2);
        gameObject.GetComponent<SpriteRenderer>().material.color = new Color(0f, ((float)System.Math.Cos(currentMagneticHeading)) * 255, 0f, 1f);

        debugText.text = "" + currentMagneticHeading + "   " + System.Math.Cos(currentMagneticHeading)* 255;
        if (currentMagneticHeading < compassSmooth || currentMagneticHeading > 360 - compassSmooth)
        {
            debugText.text = "WIN!";
        }
    }
}