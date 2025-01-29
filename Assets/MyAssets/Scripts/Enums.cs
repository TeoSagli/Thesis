using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{
    public enum FeatureUsage
    {
        Once,
        Toggle
    }
    public enum FeatureDirection
    {
        Forward,
        Backward
    }
    public enum GameState
    {
        Playing,
        Paused,
        PuzzleSolved
    }
}
