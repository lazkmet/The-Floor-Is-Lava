using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI[] timerDisplays = { };
    public float countdownSeconds = 0;
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
        if (Time.timeScale > 0 && !paused && enabled) {
            seconds -= Time.deltaTime;
            if (seconds < 0) {
                if (minutes > 0)
                {
                    seconds += 60;
                    minutes--;
                }
                else if (hours > 0) {
                    seconds += 60;
                    minutes = 59;
                    hours--;
                }
                else { 
                    FindObjectOfType<GameManager>().TimeUp();
                    enabled = false;
                    seconds = 0;
                }
            }
            UpdatePreString();
        }
        UpdateDisplay();
    }
    public void Reset()
    {
        hours = Mathf.FloorToInt(countdownSeconds / 3600);
        minutes = Mathf.FloorToInt((countdownSeconds - (hours * 3600)) / 60);
        seconds = countdownSeconds % 60;
        paused = false;
        enabled = true;
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
        timer += seconds.ToString("F2");
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
