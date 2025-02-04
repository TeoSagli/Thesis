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
    [field: SerializeField]
    [Header("Audio clip for win")]
    public AudioClip audioClip;
    private int totSolvedPuzzle = 0;
    private readonly AudioSource audioSource;
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
    public void PlayAdvancementAudio()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
