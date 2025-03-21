using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;

public class LoadScenePuzzle : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor[] cachedRayInteractors;
    public void LoadPuzzle2D()
    {
        SwitchScene("OpenDayPuzzleMap");
    }
    public void LoadPuzzle3D()
    {
        SwitchScene("OpenDayPuzzleObject");
    }
    public void LoadDemo()
    {
        SwitchScene("OpenDayDemo");
    }
    void LoadSceneWithMRUK(string sceneName)
    {
        Debug.Log("Scene data loaded, switching scene...");
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    public void SwitchScene(string sceneName)
    {
        LoadSceneWithMRUK(sceneName);
    }
}
