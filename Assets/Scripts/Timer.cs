using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI[] timerDisplays = { };
    private float seconds = 0;
    private int hours;
    private int minutes;
    private string preString = "";
    private bool paused;
    private void Awake()
    {
        Reset();
    }
    private void Update()
    {
        if (Time.timeScale > 0 && !paused) {
            seconds += Time.deltaTime;
            if (seconds >= 60) {
                seconds -= 60;
                minutes++;
                if (minutes >= 60)
                {
                    minutes = 0;
                    hours++;
                    UpdatePreString();
                }
            }          
        }
        UpdateDisplay();
    }
    public void Reset()
    {
        hours = 0;
        minutes = 0;
        seconds = 0;
        paused = false;
        UpdatePreString();
        UpdateDisplay();
    }
    private void UpdatePreString() {
        if (hours > 0)
        {
            preString = hours.ToString("D2") + ":";
        }
        else {
            preString = "";
        }
    }
    private void UpdateDisplay() {
        string timer = preString;
        timer += minutes.ToString("D2") + ":";   
        timer += Mathf.FloorToInt(seconds).ToString("D2");
        foreach (TextMeshProUGUI t in timerDisplays) {
            t.text = timer;
        }
    }
    public void Pause()
    {
        paused = true;
    }
    public void Unpause() {
        paused = false;
    }
}
