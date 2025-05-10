
using Meta.XR.MRUtilityKit;
using UnityEngine;
[System.Serializable]
public abstract class PuzzlePiece : Puzzle
{
    protected GameObject[] puzzlePiecesArr;
    protected void SpawnRndPuzzlePieces()
    {
        GameObject g = new ("PuzzlePiecesSpawner");
        var s=g.AddComponent<SpawnPuzzlePieces>();
        s.ObjectArr = puzzlePiecesArr;
        s.StartSpawn();
    }
    public GameObject[] GetPuzzleArr()
    {
        return puzzlePiecesArr;
    }
}