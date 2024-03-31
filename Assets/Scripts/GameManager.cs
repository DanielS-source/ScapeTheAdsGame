using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
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
    public TextMeshProUGUI debugText;

    int score = 0;


    public string hostname;
    public int port;

    private TcpClient client;
    private NetworkStream stream;
    private byte[] buffer = new byte[1024];
    void Start()
    {
        client = new TcpClient(hostname, port);
        stream = client.GetStream();
        StartCoroutine(ReceiveData());
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
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator ReceiveData()
    {
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        if (receivedData != null && receivedData != "")
        {
            HandleMessage(receivedData);
        }
        yield return null;
    }

    private void HandleMessage(String receivedData)
    {
        debugText.text = receivedData;
        Debug.Log($"Received from server: {receivedData}");

        SendMessage();
    }

    [Serializable]
    public class InfiniteData
    {
        public string type; // InfiniteMode
        public int stage; // How many levels where cleared
        public int clearTime; // Time as Instant
        public int score; // Score of the playthough
    }

    [Serializable]
    public class LevelData
    {
        public string type; // LevelClear, LevelFailed, LevelUnlocked
        public int level; // 1,2,3,4,5,6,...
    }

    private void SendMessage()
    {
        var replyData = new InfiniteData
        {
            type = "InfiniteMode",
            stage = 10,
            clearTime = 600000,
            score = 500
        };

        string replyJson = JsonUtility.ToJson(replyData);
        byte[] replyBytes = Encoding.UTF8.GetBytes(replyJson);
        stream.Write(replyBytes, 0, replyBytes.Length);
    }

}
