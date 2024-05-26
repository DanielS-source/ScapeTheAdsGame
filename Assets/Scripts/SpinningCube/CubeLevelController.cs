using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeLevelController : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject xPrefab;
    public int numberOfCubes = 9;
    private int cubesRemaining;
    private Camera mainCamera;
    private List<Vector3> cubePositions = new List<Vector3>();

    void Start()
    {
        mainCamera = Camera.main;
        SpawnCubes();
        StartCoroutine(CheckAndAddX());
    }

    private void SpawnCubes()
    {
        cubesRemaining = numberOfCubes;
        GameObject[] cubes = new GameObject[numberOfCubes];

        for (int i = 0; i < numberOfCubes; i++)
        {
            Vector3 newPosition;
            bool positionIsValid;

            // Find a valid position for the new cube
            do
            {
                newPosition = GetRandomPositionWithinCameraBounds();
                positionIsValid = true;

                foreach (Vector3 position in cubePositions)
                {
                    if (Vector3.Distance(newPosition, position) < 1.0f) // Ensure there is a 1 unit distance between cubes
                    {
                        positionIsValid = false;
                        break;
                    }
                }
            } while (!positionIsValid);

            GameObject cube = Instantiate(cubePrefab, newPosition, Quaternion.identity);
            cube.tag = "Cube"; // Tag the cube as "Cube"
            CubeController cubeController = cube.GetComponent<CubeController>();
            cubeController.gameController = this;
            cubes[i] = cube;

            cubePositions.Add(newPosition);
        }

        // Ensure that at least one cube has the 'X' attached
        if (cubes.Length > 0)
        {
            int randomCubeIndex = Random.Range(0, cubes.Length);
            CubeController randomCubeController = cubes[randomCubeIndex].GetComponent<CubeController>();
            randomCubeController.AddXToRandomFace(xPrefab);
        }
    }

    private Vector3 GetRandomPositionWithinCameraBounds()
    {
        // Get the camera bounds
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * screenAspect;

        // Define the margin
        float margin = 1f;

        // Calculate the bounds within which the cubes should spawn
        float minX = mainCamera.transform.position.x - cameraWidth / 2 + margin;
        float maxX = mainCamera.transform.position.x + cameraWidth / 2 - margin;
        float minY = mainCamera.transform.position.y - cameraHeight / 2 + margin;
        float maxY = mainCamera.transform.position.y + cameraHeight / 2 - margin;

        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        float z = 0; // Assuming a 2D game, so Z can be 0

        return new Vector3(x, y, z);
    }

    public void CubeRemoved()
    {
        cubesRemaining--;
        if (cubesRemaining <= 0)
        {
            Debug.Log("You win!");
            GameHandler.instance.Win(150);
        }
        else
        {
            Debug.Log("Added face!" + cubesRemaining);
            int randomCubeIndex = Random.Range(0, cubesRemaining);
            GameObject randomCube = GameObject.FindGameObjectsWithTag("Cube")[randomCubeIndex];
            CubeController randomCubeController = randomCube.GetComponent<CubeController>();
            randomCubeController.AddXToRandomFace(xPrefab);
        }
    }

    private IEnumerator CheckAndAddX()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
            bool xExists = false;

            foreach (GameObject cube in cubes)
            {
                if (cube.transform.childCount > 0)
                {
                    xExists = true;
                    break;
                }
            }

            if (!xExists && cubes.Length > 0)
            {
                int randomCubeIndex = Random.Range(0, cubes.Length);
                CubeController randomCubeController = cubes[randomCubeIndex].GetComponent<CubeController>();
                randomCubeController.AddXToRandomFace(xPrefab);
            }
        }
    }

}
