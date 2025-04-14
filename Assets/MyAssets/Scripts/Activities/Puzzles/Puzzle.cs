using System.IO;
using UnityEngine;
public abstract class Puzzle : MonoBehaviour
{
    private PuzzleData puzzleData;

    private Vector3 bounds = Vector3.zero;

    public PuzzleData PuzzleData { get => puzzleData; set => puzzleData = value; }
    public Vector3 Bounds { get => bounds; set => bounds = value; }

    protected Puzzle(float pieceScale, string titleStr, int nCols, int nRows, int nDepth)
    {
        PuzzleData=new PuzzleData(pieceScale, titleStr, nCols, nRows, nDepth);
    }
    protected Puzzle(float pieceScale, string titleStr, int nCols, int nRows)
    {
        PuzzleData = new PuzzleData(pieceScale, titleStr, nCols, nRows);
    }
    protected float CalculateOffsetForSocketCenter(int tot, int i)
    {
        float pos = i < tot / 2 ? -(tot / 2 - i) : (i - tot / 2);
        if (tot % 2 == 0)
            pos = i >= tot / 2 ? _ = pos * 2 + 1 : _ = (pos + 1) * 2 - 1;
        return tot % 2 != 0 ? pos / tot : pos / (tot * 2);
    }
    protected Sprite LoadFromPath(string path, string name)
    {
        if (File.Exists(path))
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D tex = new(2, 2);
            tex.LoadImage(fileData);
            Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            s.name = name;
            return s;
        }
        else
        {
            Debug.Log("File not found!");
            return null;
        }
    }
    protected abstract void CalculateBounds();
}