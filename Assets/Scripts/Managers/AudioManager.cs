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
    private IEnumerator ShuffleWhenItStopsPlaying()
    {
        while (true)
        {
            yield return new WaitUntil(() => !audioSource.isPlaying);
            onCurrentTrackEnded?.Invoke();
            ShuffleAndPlay();
        }
    }
    private void ShuffleAndPlay(GameState gameState = GameState.Playing)
    {
        if (tracks.Length > 0)
        {
            audioSource.clip = tracks[UnityEngine.Random.Range(0, tracks.Length - 1)];
            adjustvolume(0.25f);
            audioSource.Play();
        }
    }
    private void adjustvolume(float value)
    {
        audioSource.volume = value;
    }
}
