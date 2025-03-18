
using Meta.XR.MRUtilityKit;
using UnityEngine;
public abstract class PuzzlePiece : Puzzle
{
    protected GameObject[] puzzlePiecesArr;
    // RANDOMLY PLACE THE PIECES
    protected void SpawnRndPuzzlePieces()
    {
        SpawnPuzzlePieces script = GetComponent<SpawnPuzzlePieces>();
        script.SetObjectsArr(puzzlePiecesArr);
        script.StartSpawn();
    }

    public GameObject[] GetPuzzleArr()
    {
        return puzzlePiecesArr;
    }
}