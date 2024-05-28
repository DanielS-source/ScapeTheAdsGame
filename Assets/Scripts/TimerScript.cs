using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerScript : MonoBehaviour
{
    public static TimerScript instance;

    public float startTimer = 20;

    private float timer = 0;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI debugText;
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

    }

    // Update is called once per frame
    void Update()
    {
        bool finished = false;
        timer += Time.deltaTime;
        
        timerText.text = "" + (startTimer - timer).ToString("f2");
        debugText.text = GameHandler.instance.debugText;

        if (!finished && (startTimer - timer) < 0)
        {
            finished = true;
            GameHandler.instance.TimeOver();
        }
    }

    public void ExtendTimer()
    {
        timer -= 5;
    }
}
