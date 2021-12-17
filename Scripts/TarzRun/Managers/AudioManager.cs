using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSource;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if(GameObject.FindGameObjectsWithTag("AudioManager").Length > 1)
        {
            StopMusic();
        }
        else
        DontDestroyOnLoad(transform.gameObject);
    }
    public void PlayMusic()
    {
        if (_audioSource.isPlaying) 
            return;
        _audioSource.Play();
    }
    public void StopMusic()
    {
        _audioSource.Stop();
    }
}
