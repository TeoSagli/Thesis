
using Meta.XR.MRUtilityKit;
using UnityEngine;
public class Puzzle : MonoBehaviour
{
    [SerializeField, Header("MRUK object")]
    private MRUK mruk;
    [SerializeField, Header("Scale of the image pieces")]
    protected float pieceScale = 0.05f;
    [SerializeField, Header("Title")]
    protected string title;
    [SerializeField, Header("Grid dimensions")]
    protected int nCols = 1;
    [SerializeField]
    protected int nRows = 1;
    // RANDOMLY PLACE THE PIECES
    protected void SpawnRndPuzzlePieces(GameObject[] puzzlePiecesArr)
    {
        SpawnPuzzlePieces script = GetComponent<SpawnPuzzlePieces>();
        script.SetObjectsArr(puzzlePiecesArr);
        script.StartSpawn();
    }
    public int GetNRows()
    {
        return nRows;
    }
    public int GetNCols()
    {
        return nCols;
    }
    public float GetPieceScale()
    {
        return pieceScale;
    }
    public string GetTitle()
    {
        return title;
    }
}