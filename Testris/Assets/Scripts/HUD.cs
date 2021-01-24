using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI scoreTxt;
    public GameObject pausePanel;

    public void SetScore(int score)
    {
        if (scoreTxt != null)
            scoreTxt.text = score > 9 ? score.ToString() : "0" + score;
    }

    public void ShowPausePanel(bool show)
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(show);
            LevelManager.instance.isGamePaused = show;
        }
        
    }
}
