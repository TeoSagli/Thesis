using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public abstract class PuzzleSocket : Puzzle
{
    [SerializeField, Header("Title")]
    protected TextMeshProUGUI title;
    [SerializeField, Header("Socket Background")]
    protected GameObject socketBack;
    [SerializeField]
    protected Material canHoverMat;
    [SerializeField]
    protected Material cantHoverMat;
    [SerializeField, Header("Reference to the pieces' spawner")]
    protected GameObject puzzleObject;
    protected GameObject[] sockets;
    protected bool[] isPieceCorrect;
    //title management
    protected void SetTitle(string titleToSet)
    {
        title.text = titleToSet;
    }
    protected void PositionTitle(Vector3 vector)
    {
        title.transform.Translate(vector);
    }
    protected void SetPuzzleSize(Vector3 s)
    {
        transform.localScale = s;
    }
    //test & handle win
    protected void UpdateMatrix(XRSocketInteractor socket, int index)
    {
        isPieceCorrect[index] = socket.hasSelection && socket.name == socket.interactablesSelected[0].transform.name;
    }
    protected abstract bool TestWin();
    protected abstract void OnWin();
    protected void CheckPieceCorrect(XRSocketInteractor socket, int index)
    {
        UpdateMatrix(socket, index);
        CheckWin();
    }
    private void CheckWin()
    {
        bool res = TestWin();
        if (res)
            OnWin();
    }
    //init
    protected abstract void ImportParameters();
    //sockets management
    protected void DisableAllSockets()
    {
        for (int i = 0; i < sockets.Length; i++)
        {
            XRSocketInteractor socketInteractor = sockets[i].GetComponent<XRSocketInteractor>();
            GameObject toDel = socketInteractor.interactablesSelected[0].transform.gameObject;
            toDel.SetActive(false);
            sockets[i].SetActive(false);
        }
    }
    public abstract void GenerateAndPlaceSockets();
    protected abstract GameObject GeneratePuzzleSocket(Sprite puzzlePiece, int index);
    protected abstract GameObject GeneratePuzzleSocket(GameObject puzzlePiece, int index);
    protected abstract void PlaceSocketAt(ref GameObject socket, Vector3 offsetVec);
    protected abstract Vector3 CalculateOffsetVec(int i, int j, int k);
}
