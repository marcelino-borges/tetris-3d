using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [HideInInspector]
    public static MusicManager instance;
    [HideInInspector]
    public AudioSource audioSource;

    public float currentVolume = .15f;
    public float CurrentVolume { get => isMuted ? 0 : currentVolume; set => currentVolume = value; }

    public float maxVolume = .15f;
    public bool isMuted = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = CurrentVolume;
    }

    public void Mute(bool mute)
    {
        audioSource.mute = mute;
        instance.isMuted = mute;
    }
}
