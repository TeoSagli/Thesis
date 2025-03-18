using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenePuzzle : MonoBehaviour
{
    public void LoadPuzzle2D()
    {
        SceneManager.LoadScene("OpenDayPuzzleMap", LoadSceneMode.Single);
    }
    public void LoadPuzzle3D()
    {
        SceneManager.LoadScene("OpenDayPuzzleObject", LoadSceneMode.Single);
    }
    public void LoadDemo()
    {
        SceneManager.LoadScene("OpenDayDemo", LoadSceneMode.Single);
    }
}
