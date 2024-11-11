using System;
using System.Collections;
using System.Collections.Generic;
using DilmerGames.Core.Singletons;
using UnityEngine;

public class PuzzleSolverFeature : Singleton<PuzzleSolverFeature>
{
    [SerializeField]
    private string playerTag;

    public Action<GameState> onPuzzleSolved;

    public bool puzzleSolvedDetected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == playerTag && !puzzleSolvedDetected)
        {
            onPuzzleSolved?.Invoke(GameState.PuzzleSolved);
            puzzleSolvedDetected = true;
            ActivatePortal();
        }
    }
    private void ActivatePortal()
    {
        PortalFeature script = GameObject.Find("Portal").GetComponent<PortalFeature>();
        script.ToggleParticle(script.GetParticleSystemIn());
    }
}
