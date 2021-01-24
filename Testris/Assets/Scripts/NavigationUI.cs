using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationUI : MonoBehaviour
{
    [HideInInspector]
    public AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void LoadMainMenu()
    {
        PlayClickSfx();
        SceneManager.LoadScene(0);
    }

    public void LoadPlayground()
    {
        PlayClickSfx();
        SceneManager.LoadScene(1);
    }

    public void Restart()
    {
        PlayClickSfx();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayClickSfx()
    {
        SoundManager.instance.PlayButtonSfx();
    }
}
