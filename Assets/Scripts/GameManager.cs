using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    public GameObject block;
    public float maxX;
    public Transform spawnPoint;
    public float spawnRate;

    bool gameStarted = false;

    public GameObject tapText;
    public TextMeshProUGUI scoreText;

    public int winScore = 10;
    public int decreaseScore = 2;
    int score = 0;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {

    }

    void Update()
    {
        
        if (Input.GetMouseButtonDown(0) && !gameStarted)
        {
            StartSpawning();
            gameStarted = true;
            tapText.SetActive(false);
        }

    }

    void FixedUpdate() {
        if (Application.platform == RuntimePlatform.Android) {
            if (Input.GetKey(KeyCode.Escape)) {
                QuitGame();
            }
        }
    }


    private void StartSpawning()
    {
        InvokeRepeating("SpawnBlock", 0.5f, spawnRate);
    }

    private void SpawnBlock()
    {
        Vector3 spawnPosition = spawnPoint.position;

        spawnPosition.x = Random.Range(-maxX, maxX);

        Instantiate(block, spawnPosition, Quaternion.identity);

        score++;

        scoreText.text = score.ToString();

        if (score >= winScore) {
            GameHandler.instance.Win(200);
        }

    }

    public void DecreaseScore()
    {
        if (score <= decreaseScore)
        {
            score = 0;
        }
        else {
            score -= decreaseScore;
        }
        scoreText.text = score.ToString();

    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
