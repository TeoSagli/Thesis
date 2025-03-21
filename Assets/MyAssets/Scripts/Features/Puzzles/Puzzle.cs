using Meta.XR.MRUtilityKit;
using UnityEngine;
public abstract class Puzzle : BaseFeature
{
    [SerializeField, Header("Scale of the pieces")]
    protected float pieceScale = 0.05f;
    [SerializeField, Header("Title")]
    protected string titleStr;
    [SerializeField, Header("Grid dimensions")]
    protected int nCols = 1;
    [SerializeField]
    protected int nRows = 1;
    protected Vector3 bounds;
    protected float CalculateOffsetForSocketCenter(int tot, int i)
    {
        float pos = i < tot / 2 ? -(tot / 2 - i) : (i - tot / 2);
        if (tot % 2 == 0)
            pos = i >= tot / 2 ? _ = pos * 2 + 1 : _ = (pos + 1) * 2 - 1;
        return tot % 2 != 0 ? pos / tot : pos / (tot * 2);
    }
    public float GetPieceScale()
    {
        return pieceScale;
    }
    public string GetTitle()
    {
        return titleStr;
    }
    public int GetNCols()
    {
        return nCols;
    }
    public int GetNRows()
    {
        return nRows;
    }
    protected abstract void CalculateBounds();
}