using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    public float startTimer = 20;

    private float timer = 0;

    public TextMeshProUGUI timerText;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        
        timerText.text = "" + (startTimer - timer).ToString("f2");

        if ((startTimer - timer) < 0)
        {

            GameHandler.InfiniteData dataToBeSent = new GameHandler.InfiniteData();
            dataToBeSent.clearTime = (int)(timer * 1000);
            dataToBeSent.type = "InfiniteMode";
            dataToBeSent.stage = 1;
            dataToBeSent.score = dataToBeSent.clearTime;
            GameHandler.instance.SendMessage(dataToBeSent);
        }
    }
}
