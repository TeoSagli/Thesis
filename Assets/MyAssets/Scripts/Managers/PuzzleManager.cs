using System;
using System.Collections;
using System.Collections.Generic;
using DilmerGames.Core.Singletons;
using UnityEngine;
using static Enums;

public class PuzzleManager : Singleton<PuzzleManager>
{

    [SerializeField]
    [Header("Tot puzzles to solve")]

    private int totPuzzle = 4;
    [Header("Audio")]
    [SerializeField]
    private AudioClip audioClipWin;
    [SerializeField]
    private AudioClip audioClipFail;
    private int totSolvedPuzzle = 0;
    [SerializeField]
    private AudioSource audioSource;
    public Action<GameState> onPuzzleSolved;

    private void Start()
    {

    }
    /*
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == playerTag && !puzzleSolvedDetected)
            {
                onPuzzleSolved?.Invoke(GameState.PuzzleSolved);
                puzzleSolvedDetected = true;
                   ActivatePortal();
            }
        }*/
    /* public void ActivatePortal()
     {
         PortalFeature script = GameObject.FindGameObjectWithTag("Portal").GetComponent<PortalFeature>();
         script.ToggleParticle(script.GetParticleSystemIn());
     }*/
    public void PuzzleAdvancement()
    {
        PlayAdvancementAudio();
        totSolvedPuzzle++;
        ProgressBarFeature.Instance.UpdateBar(totSolvedPuzzle * 100 / totPuzzle);
        if (totSolvedPuzzle == totPuzzle)
            onPuzzleSolved?.Invoke(GameState.PuzzleSolved);
    }
    public int GetTotPuzzle()
    {
        return totPuzzle;
    }
    private void PlayAdvancementAudio()
    {
        audioSource.clip = audioClipWin;
        audioSource.Play();
    }
    public void PlayFailAudio()
    {
        audioSource.clip = audioClipFail;
        audioSource.Play();
    }
}
