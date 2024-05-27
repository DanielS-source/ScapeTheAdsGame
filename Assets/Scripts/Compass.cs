using System;
using UnityEngine;

public class CompassController : MonoBehaviour
{
    public float compassSmooth = 0.5f;
    private float m_lastMagneticHeading = 0f;
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

        if (m_lastMagneticHeading < currentMagneticHeading - compassSmooth || m_lastMagneticHeading > currentMagneticHeading + compassSmooth)
        {
            m_lastMagneticHeading = currentMagneticHeading;
            if (m_lastMagneticHeading < 10 || m_lastMagneticHeading > 350)
            {
                GameHandler.instance.Win(200);
            }
        }
    }
}