using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MenuManager))]
public class GameManager : MonoBehaviour
{
    PlayerMovement player;
    MenuManager menus;
    Reticle ret;
    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
        menus = GetComponent<MenuManager>();
        ret = FindObjectOfType<Reticle>();
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
    }
    public void Reset()
    {
        player.Reset();
        menus.Reset();
        ret.Show();
    }
}
