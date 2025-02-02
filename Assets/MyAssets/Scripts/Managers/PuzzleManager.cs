using System;
using System.Collections;
using System.Collections.Generic;
using DilmerGames.Core.Singletons;
using UnityEngine;
using static Enums;

public class PuzzleManager : Singleton<PuzzleManager>
{

    [SerializeField]
    private int totPuzzle = 4;
    private int totSolvedPuzzle = 0;

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
        totSolvedPuzzle++;
        if (totSolvedPuzzle == totPuzzle)
            onPuzzleSolved?.Invoke(GameState.PuzzleSolved);
    }
}
