using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    public float startTimer = 20;

    private float timer = 0;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI debugText;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        
        timerText.text = "" + (startTimer - timer).ToString("f2");
        debugText.text = GameHandler.instance.debugText;

        if ((startTimer - timer) < 0)
        {
            GameHandler.instance.GameOver();
        }
    }
}
