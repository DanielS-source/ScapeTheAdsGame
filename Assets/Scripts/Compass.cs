using System;
using UnityEngine;

public class CompassController : MonoBehaviour
{
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
        float currentMagneticHeading = Input.compass.magneticHeading;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0, (((float)Math.Cos(Math.PI / 180 * currentMagneticHeading)) + 1)/2, 0);

        if (currentMagneticHeading < 10 || currentMagneticHeading > 350)
        {
            GameHandler.instance.Win(200);
        }
    }
}