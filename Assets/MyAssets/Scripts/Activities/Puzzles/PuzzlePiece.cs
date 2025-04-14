
using Meta.XR.MRUtilityKit;
using UnityEngine;
[System.Serializable]
public abstract class PuzzlePiece : Puzzle
{
    protected GameObject[] puzzlePiecesArr;

    protected PuzzlePiece(float pieceScale, string titleStr, int nCols, int nRows, int nDepth) : base(pieceScale, titleStr, nCols, nRows, nDepth) { }

    protected PuzzlePiece(float pieceScale, string titleStr, int nCols, int nRows) : base(pieceScale, titleStr, nCols, nRows) { }
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