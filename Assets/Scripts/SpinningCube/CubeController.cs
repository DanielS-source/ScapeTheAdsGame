using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public CubeLevelController gameController;
    private Vector3 previousMousePosition;
    private bool isDragging;
    private float spinSpeed;
    private bool useGyro;

    void Start()
    {
        spinSpeed = Random.Range(0.5f, 5.0f);

        useGyro = SystemInfo.supportsGyroscope;

        if (useGyro)
        {
            spinSpeed = Random.Range(1.5f, 8.0f);
            Input.gyro.enabled = true;
        }
    }

    void Update()
    {
        if (useGyro)
        {
            HandleGyroInput();
        }
        else
        {
            HandleMouseInput();
        }
    }

    private void HandleGyroInput()
    {
        Quaternion gyroInput = Input.gyro.attitude;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f) * new Quaternion(-gyroInput.x * spinSpeed, -gyroInput.y * spinSpeed, gyroInput.z * spinSpeed, gyroInput.w);

        // Apply spin speed to the gyroscope input
        //transform.rotation = Quaternion.Slerp(transform.rotation, adjustedGyroInput, spinSpeed * Time.deltaTime);
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            previousMousePosition = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 deltaMousePosition = currentMousePosition - previousMousePosition;

            float rotationX = deltaMousePosition.y * 0.2f * spinSpeed; // Adjust sensitivity as needed
            float rotationY = -deltaMousePosition.x * 0.2f * spinSpeed; // Adjust sensitivity as needed

            transform.Rotate(Vector3.up, rotationY, Space.World); // Y-axis rotation
            transform.Rotate(Vector3.right, rotationX, Space.World); // X-axis rotation

            previousMousePosition = currentMousePosition;
        }
    }

    public void AddXToRandomFace(GameObject xPrefab)
    {
        Debug.Log("X!");
        // Define the six possible face positions and rotations
        Vector3[] facePositions = {
            new Vector3(0, 0, 0.5f),
            new Vector3(0, 0, -0.5f),
            new Vector3(0.5f, 0, 0),
            new Vector3(-0.5f, 0, 0),
            new Vector3(0, 0.5f, 0),
            new Vector3(0, -0.5f, 0)
        };

        Quaternion[] faceRotations = {
            Quaternion.identity,
            Quaternion.Euler(0, 180, 0),
            Quaternion.Euler(0, 90, 0),
            Quaternion.Euler(0, -90, 0),
            Quaternion.Euler(-90, 0, 0),
            Quaternion.Euler(90, 0, 0)
        };

        // Select a random face
        int randomIndex = Random.Range(0, facePositions.Length);
        Vector3 randomPosition = facePositions[randomIndex];
        Quaternion randomRotation = faceRotations[randomIndex];

        // Instantiate the 'X' on the selected face
        GameObject xInstance = Instantiate(xPrefab, transform);
        xInstance.transform.localPosition = randomPosition;
        xInstance.transform.localRotation = randomRotation;

        // Move the 'X' slightly forward along its local Z-axis to ensure it is rendered above the cube
        xInstance.transform.Translate(Vector3.forward * 0.001f, Space.Self);

        // Ensure the 'X' is visible and correctly sorted
        SpriteRenderer xRenderer = xInstance.GetComponent<SpriteRenderer>();
        xRenderer.sortingOrder = 10; // Ensure it's on top of the cube
    }
}
