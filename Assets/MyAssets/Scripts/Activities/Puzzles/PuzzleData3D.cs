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
    private string ext;
    [JsonConstructor]
    public PuzzleData3D(string meshPath, string meshName, int meshId, string ext)
    {
        MeshPath = meshPath;
        MeshName = meshName;
        MeshId = meshId;
        Ext = ext;
    }
    public string MeshPath { get => meshPath; set => meshPath = value; }
    public string MeshName { get => meshName; set => meshName = value; }
    public string Ext { get => ext; set => ext = value; }
    public int MeshId { get => meshId; set => meshId = value; }
    
}
