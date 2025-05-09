using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PuzzleData3D : MonoBehaviour
{
    private string meshPath;
    private int meshId;
    private string meshName;
    [JsonConstructor]
    public PuzzleData3D(string meshPath, string meshName, int meshId)
    {
        MeshPath = meshPath;
        MeshName = meshName;
        MeshId = meshId;
    }
    public string MeshPath { get => meshPath; set => meshPath = value; }
    public string MeshName { get => meshName; set => meshName = value; }
    public int MeshId { get => meshId; set => meshId = value; }
    
}
