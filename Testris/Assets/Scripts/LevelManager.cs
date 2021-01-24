using System;
using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public int score;
    public float gameSpeed = 2f;
    public bool isGameOver = false;
    public bool isGamePaused = false;
    public GameObject gameOverPanel;
    public HUD hud;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SetGameOver()
    {
        isGameOver = true;
        ShowGameOverPanel();
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void IncreaseScore()
    {
        score++;
        if(hud != null)
            hud.SetScore(score);
    }
}
