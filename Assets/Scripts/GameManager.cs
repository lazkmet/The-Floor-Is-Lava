using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MenuManager))]
public class GameManager : MonoBehaviour
{
    PlayerMovement player;
    MenuManager menus;
    Reticle ret;
    Lava lava;
    Timer[] timers = { };
    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
        menus = GetComponent<MenuManager>();
        ret = FindObjectOfType<Reticle>();
        lava = FindObjectOfType<Lava>();
        timers = FindObjectsOfType<Timer>();
    }
    private void Start()
    {
        Reset();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            menus.TogglePause();
            if (menus.isPaused)
            {
                ret.Hide();
                player.enabled = false;
            }
            else {
                ret.Show();
                player.enabled = true;
            }
        }
    }
    public void GameOver() {
        Time.timeScale = 0;
        ret.Hide();
        menus.SetActiveScreen(1);
        lava.StopRise();
    }
    public void Reset()
    {
        player.Reset();
        menus.Reset();
        lava.Reset();
        ret.Show();
        foreach (Timer t in timers) {
            t.Reset();
        }
    }
    public void TimeUp() {
        lava.StartRise();
    }
}
