using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class PuzzleData2D
{
    private string spritePath;
    private int spriteId;
    private string spriteName;
    [JsonConstructor]
    public PuzzleData2D(string spritePath, string spriteName, int spriteId)
    {
        SpritePath = spritePath;
        SpriteName = spriteName;
        SpriteId = spriteId;
    }
    public string SpritePath { get => spritePath; set => spritePath = value; }
    public string SpriteName { get => spriteName; set => spriteName = value; }
    public int SpriteId { get => spriteId; set => spriteId = value; }
}