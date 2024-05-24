using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public int health;
    public float shakeThreshold = 2.0f; // Adjust this value to set the sensitivity
    private Vector3 lastAcceleration;
    private bool isFirstUpdate = true;

    public delegate void WallDestroyedHandler();
    public static event WallDestroyedHandler OnWallDestroyed;

    // Update is called once per frame
    void Update()
    {
        // Get the current acceleration
        Vector3 currentAcceleration = Input.acceleration;

        if (isFirstUpdate)
        {
            // Skip the first update to initialize the lastAcceleration
            lastAcceleration = currentAcceleration;
            isFirstUpdate = false;
            return;
        }

        // Calculate the difference in acceleration
        Vector3 deltaAcceleration = currentAcceleration - lastAcceleration;

        // Calculate the magnitude of the difference
        float shakeMagnitude = deltaAcceleration.magnitude;

        // Check if the shake magnitude exceeds the threshold
        if (shakeMagnitude > shakeThreshold)
        {
            // Reduce health
            health--;

            //log the shake for debugging purposes
            Debug.Log("Shake detected! Health reduced to: " + health);
        }

        // Update the lastAcceleration for the next frame
        lastAcceleration = currentAcceleration;

        // Destroy the wall if health is below zero
        if (health < 0)
        {
            Destroy(gameObject);
            OnWallDestroyed?.Invoke();
        }
    }
}
