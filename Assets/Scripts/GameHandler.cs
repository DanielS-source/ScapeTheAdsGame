using System;
using System.Collections;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;

    public string debugText;

    public static bool isInfiniteMode = true;

    public GameObject tapText;
    public TextMeshProUGUI scoreText;
    //public TextMeshProUGUI debugText;

    //Array of game scene names, in order of their appearance in build settings, starting from 1
    public static string[] gameSceneNames = new string[] {"Game1","Game2", "Game3", "Game4"};
    public static int? levelBeingPlayed = null;

    public static int score = 0;


    public string hostname;
    public int port;

    private TcpClient client;
    private static NetworkStream stream;

    private byte[] buffer = new byte[1024];

    private void Awake()
    {

        //Singleton and adding this gameObject to DontDestroyOnLoad
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.LoadScene("BombTimer", LoadSceneMode.Additive);

    }

    // Start is called before the first frame update
    void Start()
    {
        client = new TcpClient(hostname, port);
        stream = client.GetStream();
        StartCoroutine(ReceiveData());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator ReceiveData()
    {
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        if (!string.IsNullOrEmpty(receivedData))
        {
            HandleMessage(receivedData);
        }
        yield return null;
    }

    [Serializable]
    public class GameMode
    {
        public String gamemode; // infinite, level
        public int level; // 1, 2, 3...
    }

    private void HandleMessage(String receivedData)
    {
        Debug.Log($"Received from server: {receivedData}");

        GameMode gameMode = JsonUtility.FromJson<GameMode>(receivedData);

        debugText = receivedData;

        if (gameMode.gamemode.Equals("level"))
        {
            isInfiniteMode = false;
            levelBeingPlayed = gameMode.level;

            SceneManager.LoadScene(gameMode.level + 1, LoadSceneMode.Additive);
        } else if (gameMode.gamemode.Equals("infinite"))
        {
            isInfiniteMode = true;
            loadNextSceneExcept(null);
        }
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
        
    public static void SendMessage(InfiniteData replyData)
    {
        string replyJson = JsonUtility.ToJson(replyData);
        byte[] replyBytes = Encoding.UTF8.GetBytes(replyJson);
        Debug.Log($"Sending to server: {replyJson}");
        stream.Write(replyBytes, 0, replyBytes.Length);
    }

    public static void SendMessage(LevelData replyData)
    {
        string replyJson = JsonUtility.ToJson(replyData);
        byte[] replyBytes = Encoding.UTF8.GetBytes(replyJson);
        Debug.Log($"Sending to server: {replyJson}");
        stream.Write(replyBytes, 0, replyBytes.Length);
    }


    //this is expected to be called from a game scene whose game is over,
    //with the calling game scene name as parameter.
    public static void loadNextSceneExcept(string callingScene)
    {
        if (callingScene == null)
        {
            loadRandomGameScene(gameSceneNames);
        } else
        {
            string[] finallyCallableScenes = new string[] {};
            var tempList = finallyCallableScenes.ToList();
            foreach (string callableScene in gameSceneNames)
            {
                if (!callableScene.Equals(callingScene)) {
                    tempList.Add(callableScene);
                }
            }
            finallyCallableScenes = tempList.ToArray();
            if (finallyCallableScenes.Length > 0)
            {
                loadRandomGameScene(finallyCallableScenes);
            }
        }
    }

    private static void loadRandomGameScene(string[] callableScenes)
    {
        int sceneIndex = new Random().Next(0, callableScenes.Length);
        //if (levelBeingPlayed > 0 && SceneManager.GetActiveScene() != null && SceneManager.GetActiveScene().name.Equals(gameSceneNames[levelBeingPlayed - 1]))
        //{
        if (levelBeingPlayed.HasValue)
        {
            SceneManager.UnloadSceneAsync(levelBeingPlayed.Value + 1, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        }
        //}
        levelBeingPlayed = Array.IndexOf(gameSceneNames, Array.Find<string>(gameSceneNames, cs => cs.Equals(callableScenes[sceneIndex]))) + 1;
        SceneManager.LoadScene(gameSceneNames[levelBeingPlayed.Value - 1], LoadSceneMode.Additive);
    }

    private void Continue()
    {
        loadNextSceneExcept(gameSceneNames[levelBeingPlayed.Value - 1]);
    }

    public void TimeOver()
    {
        InfiniteData infiniteData = new InfiniteData();
        infiniteData.type = "InfiniteMode";
        infiniteData.stage = levelBeingPlayed.Value;
        infiniteData.clearTime = (int)Time.time;
        infiniteData.score = score;

        SendMessage(infiniteData);
        Application.Quit();
    }

    public void Win(int accScore)
    {
        if (isInfiniteMode)
        {
            score = score + accScore;
            TimerScript.instance.ExtendTimer();
            Continue();
        }
        else
        {
            LevelData levelData = new LevelData();
            levelData.type = "LevelClear";
            levelData.level = levelBeingPlayed.Value;

            SendMessage(levelData);
            Application.Quit();
        }
    }
}
