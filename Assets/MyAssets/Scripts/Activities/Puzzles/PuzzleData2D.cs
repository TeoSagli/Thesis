using UnityEngine;

[System.Serializable]
public class PuzzleData2D
{
    private string spritePath;
    private string spriteName;

    public PuzzleData2D(string spritePath, string spriteName)
    {
        SpritePath = spritePath;
        SpriteName = spriteName;
    }

    public string SpritePath { get => spritePath; set => spritePath = value; }
    public string SpriteName { get => spriteName; set => spriteName = value; }
}