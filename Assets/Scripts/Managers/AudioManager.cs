using System;
using System.Collections;
using System.Collections.Generic;
using DilmerGames.Core.Singletons;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager>
{

    [Header("Background Music Tracks")]
    [SerializeField]
    private AudioClip[] tracks;
    private AudioSource audioSource;

    [Header("Events")]
    public Action onCurrentTrackEnded;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(ShuffleWhenItStopsPlaying());
        ShuffleAndPlay();
    }
    /// <summary>
    /// Shuffle the tracks and play the audio source if game is playing.
    /// </summary>
    /// 
    private IEnumerator ShuffleWhenItStopsPlaying()
    {
        while (true)
        {
            yield return new WaitUntil(() => !audioSource.isPlaying && IsGamePlaying());
            onCurrentTrackEnded?.Invoke();
            ShuffleAndPlay();
        }
    }
    /// <summary>
    /// Shuffle the tracks and play the audio source.
    /// </summary>
    /// 
    private void ShuffleAndPlay()
    {
        if (tracks.Length > 0 && IsGamePlaying())
        {
            audioSource.clip = tracks[UnityEngine.Random.Range(0, tracks.Length - 1)];
            Adjustvolume(0.25f);
            audioSource.Play();
        }
    }
    /// <summary>
    /// Adjust audio source volume.
    /// </summary>
    /// <param name="value">Volume level.</param>
    /// 
    public void Adjustvolume(float value)
    {
        audioSource.volume = value;
    }

    /// <summary>
    /// Pause audio source.
    /// </summary>
    /// 
    public void PauseAudioSource()
    {
        audioSource.Pause();
    }

    /// <summary>
    /// Returns if the game is playing.
    /// </summary>
    /// 
    bool IsGamePlaying()
    {
        return GameManager.Instance.GetGameState() == GameState.Playing;
    }
}
