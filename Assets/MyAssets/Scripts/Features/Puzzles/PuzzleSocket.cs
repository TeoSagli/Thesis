using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public abstract class PuzzleSocket : BaseFeature
{
    [SerializeField, Header("Title")]
    protected TextMeshProUGUI title;
    [SerializeField, Header("Socket Background")]
    protected GameObject socketBack;
    [SerializeField, Header("Puzzle Object Reference")]
    protected GameObject puzzleObject;
    protected int nRows = 1;
    protected int nCols = 1;
    protected string titleToSet;
    protected Vector3 bounds;
    protected float pieceScale;
    protected void SetTitle(string titleToSet)
    {
        title.text = titleToSet;
    }
    protected void PositionTitle(Vector3 vector)
    {
        title.transform.Translate(vector);
    }
    protected void SetPuzzleSize(Vector3 s)
    {
        transform.localScale = s;
    }
    protected float CalculateOffsetForSocketCenter(int tot, int i)
    {
        float pos = i < tot / 2 ? -(tot / 2 - i) : (i - tot / 2);
        if (tot % 2 == 0)
            pos = i >= tot / 2 ? _ = pos * 2 + 1 : _ = (pos + 1) * 2 - 1;
        return tot % 2 != 0 ? pos / tot : pos / (tot * 2);
    }

    protected abstract void ImportParameters();
    protected abstract void CalculateBounds();
    public abstract void GenerateAndPlaceSockets();
    protected abstract GameObject GeneratePuzzleSocket(Sprite puzzlePiece, int index);
    protected abstract GameObject GeneratePuzzleSocket(GameObject puzzlePiece, int index);
    protected abstract void PlaceSocketAt(ref GameObject socket, Vector3 offsetVec);
    protected abstract Vector3 CalculateOffsetVec(int i, int j, int k);
}
