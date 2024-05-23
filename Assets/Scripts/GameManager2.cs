using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager2 : MonoBehaviour
{
    public GameObject[] wallPrefabs;
    public float[] difficulty; // Probability distribution of each wall type
    public Transform spawnPoint;

    public GameObject tapText;
    public GameObject shakeText;
    public GameObject pressText;
    public TextMeshProUGUI scoreText;

    private int score = 0;
    private bool gameStarted = false;
    private bool gameEnded = false; // Flag to indicate the game has ended
    private int wallsSpawned = 0;
    private int maxWalls = 5;

    private Button buttonScript;

    void Start()
    {
        buttonScript = FindObjectOfType<Button>(); // Find the button script in the scene
        Wall.OnWallDestroyed += HandleWallDestroyed;
    }

    void OnDestroy()
    {
        Wall.OnWallDestroyed -= HandleWallDestroyed;
    }

    // Spawns a random wall at the spawn point based on the difficulty distribution
    void SpawnWall()
    {
        if (wallsSpawned >= maxWalls)
        {
            gameStarted = false;
            gameEnded = true; // Set the flag indicating the game has ended
            pressText.SetActive(true);
            return;
        }

        Vector3 spawnPos = spawnPoint.position;

        // Randomly select a wall type based on difficulty
        GameObject wall = GetRandomWallType();

        Instantiate(wall, spawnPos, Quaternion.identity);

        score++;
        scoreText.text = score.ToString();

        wallsSpawned++;
    }

    // Returns a randomly selected wall prefab based on the difficulty distribution
    GameObject GetRandomWallType()
    {
        float rand = Random.value;
        float cumulativeProbability = 0f;
        for (int i = 0; i < wallPrefabs.Length; i++)
        {
            cumulativeProbability += difficulty[i];
            if (rand <= cumulativeProbability)
            {
                return wallPrefabs[i];
            }
        }
        // If no wall type is selected, return the last wall prefab
        return wallPrefabs[wallPrefabs.Length - 1];
    }

    void HandleWallDestroyed()
    {
        if (gameStarted && !gameEnded)
        {
            shakeText.SetActive(false);
            SpawnWall();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !gameStarted && !gameEnded)
        {
            gameStarted = true;
            wallsSpawned = 0;
            tapText.SetActive(false);
            shakeText.SetActive(true);
            SpawnWall();
        }

        // Check if the button is pressed to exit the game and no walls are left
        if (buttonScript.isPressed && AreAllWallsDestroyed())
        {
            ExitGame();
        }
    }

    // Checks if all walls are destroyed
    bool AreAllWallsDestroyed()
    {
        return GameObject.FindObjectsOfType<Wall>().Length == 0;
    }

    // Exits the game or application
    void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
