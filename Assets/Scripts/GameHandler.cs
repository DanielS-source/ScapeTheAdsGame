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

    public bool isInfiniteMode
    {
        get { return isInfiniteMode; }
        set { isInfiniteMode = value; }
    }

    public GameObject tapText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI debugText;

    public static string[] gameSceneNames = new string[] {"Game"};

    int score = 0;


    public string hostname;
    public int port;

    private TcpClient client;
    private NetworkStream stream;

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
        //client = new TcpClient(hostname, port);
        //stream = client.GetStream();

    }

    // Update is called once per frame
    void Update()
    {

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
        
    public void SendMessage(InfiniteData replyData)
    {
        string replyJson = JsonUtility.ToJson(replyData);
        byte[] replyBytes = Encoding.UTF8.GetBytes(replyJson);
        Debug.Log($"Sending to server: {replyJson}");
        //stream.Write(replyBytes, 0, replyBytes.Length);
    }

    public void SendMessage(LevelData replyData)
    {
        string replyJson = JsonUtility.ToJson(replyData);
        byte[] replyBytes = Encoding.UTF8.GetBytes(replyJson);
        Debug.Log($"Sending to server: {replyJson}");
        //stream.Write(replyBytes, 0, replyBytes.Length);
    }


    //this is expected to be called from a game scene whose game is over,
    //with the calling game scene as parameter.
    public static void loadNextSceneExcept(string callingScene)
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
            SceneManager.LoadScene((string) finallyCallableScenes[new Random().Next(0, finallyCallableScenes.Length)], LoadSceneMode.Additive);
            SceneManager.UnloadScene(callingScene);
        }
    }
}
