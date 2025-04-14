using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PuzzleData3D : MonoBehaviour
{
    private string meshPath;
    private string meshName;

    public PuzzleData3D(string meshPath, string meshName)
    {
      MeshPath = meshPath;
      MeshName = meshName;
    }
    public string MeshPath { get => meshPath; set => meshPath = value; }
    public string MeshName { get => meshName; set => meshName = value; }
    
}
