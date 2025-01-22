using System.Collections;
using System.Collections.Generic;
using LearnXR.Core.Utilities;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public void printToLogger(string s)
    {
        SpatialLogger.Instance.LogInfo($"{GetType().Name} > " + s);
    }
}
