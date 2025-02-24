using System;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Puzzle2DSocketFeature : BaseFeature
{

    [SerializeField, Header("Materials")]
    private Material validMaterial;
    [SerializeField]
    private Material notValidMaterial;
    [SerializeField, Header("Title")]
    private TextMeshProUGUI title;
    [SerializeField, Header("Socket Background")]
    private GameObject socketBack;
    [SerializeField, Header("Puzzle Object Reference")]
    private GameObject puzzleObject;
    private float pieceScale;
    private bool[] isPieceCorrect;
    private float boundsX;
    private float boundsY;
    private float boundsZ;
    private Sprite originalSprite;
    private int nRows = 1;
    private int nCols = 1;
    private string titleToSet;

    // Start is called before the first frame update
    private void Start()
    {
        SetVolume(0.2f);
        GenerateAndPlaceSockets();
    }
    //================CONFIGURATION===================
    private void ImportParameters()
    {
        Puzzle2DFeature puzzlePiecesScript = puzzleObject.GetComponent<Puzzle2DFeature>();
        originalSprite = puzzlePiecesScript.GetOriginalSprite();
        nRows = puzzlePiecesScript.GetNRows();
        nCols = puzzlePiecesScript.GetNCols();
        titleToSet = puzzlePiecesScript.GetTitle();
        pieceScale = puzzlePiecesScript.GetPieceScale();
    }
    private void SetTitle(String titleToSet)
    {
        title.text = titleToSet;
    }
    private void PositionTitle(Vector3 vector)
    {
        title.transform.Translate(vector);
    }
    private void CalculateBounds()
    {
        boundsX = originalSprite.bounds.size.x / nCols;
        boundsY = originalSprite.bounds.size.y / nRows;
        boundsZ = originalSprite.bounds.size.z;
    }
    private void SetPuzzleSize()
    {
        transform.localScale = new Vector3(boundsX * nCols, boundsY * nRows, transform.localScale.z) * pieceScale;
    }
    //================SOCKETS===================
    public void GenerateAndPlaceSockets()
    {
        ImportParameters();
        //configure bounds
        CalculateBounds();
        //configure puzzle size
        SetPuzzleSize();
        //configure title
        SetTitle(titleToSet);
        PositionTitle(new Vector3(0, -boundsY * 0.75f * nRows, 0) * pieceScale);
        //define bool matrix to evaluate win conditions
        isPieceCorrect = new bool[nRows * nCols];
        //configure puzzle sockets
        for (int i = 0; i < nCols; i++)
        {
            for (int j = 0; j < nRows; j++)
            {
                int index = nRows * i + (nRows - j - 1);
                GameObject socket = GeneratePuzzleSocket(originalSprite, index);
                PlaceSocketAt(ref socket, i, j);
            }
        }
    }
    private void PlaceSocketAt(ref GameObject socket, int i, int j)
    {
        float offsetX = CalculateTraslation(nCols, i);
        float offsetY = CalculateTraslation(nRows, j);
        socket.transform.Translate(offsetX * transform.localScale.x, offsetY * transform.localScale.y, 0);
    }
    private float CalculateTraslation(int tot, int i)
    {
        float pos = i < tot / 2 ? -(tot / 2 - i) : (i - tot / 2);
        if (tot % 2 == 0)
            pos = i >= tot / 2 ? _ = pos * 2 + 1 : _ = (pos + 1) * 2 - 1;
        return tot % 2 != 0 ? pos / tot : pos / (tot * 2);
    }
    private GameObject GeneratePuzzleSocket(Sprite puzzlePiece, int i)
    {
        GameObject socket = new(puzzlePiece.name + "-tile" + i);
        BoxCollider box = socket.AddComponent(typeof(BoxCollider)) as BoxCollider;
        XRSocketInteractor xRSocketInteractor = socket.AddComponent(typeof(XRSocketInteractor)) as XRSocketInteractor;

        //setup box collider
        Vector3 reduceVec = new(4, 4, 1);
        box.size = new Vector3(boundsX / reduceVec.x, boundsY / reduceVec.y, boundsZ / reduceVec.z);
        box.isTrigger = true;

        //setup attachPoint
        GameObject attachPoint = new("Attach Point");
        AddAttachPoint(socket, ref attachPoint);
        //socket interactor
        xRSocketInteractor.hoverSocketSnapping = true;
        xRSocketInteractor.interactableHoverMeshMaterial = validMaterial;
        xRSocketInteractor.interactableCantHoverMeshMaterial = notValidMaterial;
        xRSocketInteractor.interactionLayers = LayerMask.GetMask("Puzzle2D");
        xRSocketInteractor.attachTransform = attachPoint.transform;
        xRSocketInteractor.selectEntered.AddListener((s) =>
        {
            //PlayOnStarted();
            CheckPieceCorrect(xRSocketInteractor, i);
        });
        //socket back
        var back = Instantiate(socketBack);
        back.transform.position = socket.transform.position;
        back.transform.localScale = new Vector3(boundsX, boundsY, boundsZ);
        back.transform.parent = socket.transform;
        //change transform and attach to parent
        socket.transform.position += transform.position;
        socket.transform.localScale = pieceScale * Vector3.one;
        socket.transform.parent = transform;
        //create child game object to show
        return socket;
    }
    void AddAttachPoint(GameObject obj, ref GameObject attachPoint)
    {
        //attachPoint.transform.localPosition = new(0, 0, 0);
        float offsetz = -0.2f;
        float offset = -boundsY / (2 * obj.transform.localScale.y);
        attachPoint.transform.Translate(new(0, offset, offsetz));
        attachPoint.transform.parent = obj.transform;
    }
    //================CHECK AND HANDLE WIN===================
    private void CheckPieceCorrect(XRSocketInteractor socket, int index)
    {
        isPieceCorrect[index] = socket.isSelectActive && socket.name == socket.interactablesSelected[0].transform.name;
        CheckWin();
    }
    private void CheckWin()
    {
        bool res = TestWin();
        if (res)
            OnWin();

        //INTERCTABLESECELT SELECT MODER socket.interactablesSelected[0].SELECTOMODE
    }
    private bool TestWin()
    {
        for (int i = 0; i < nRows; i++)
            for (int j = 0; j < nCols; j++)
                if (!isPieceCorrect[i * nCols + j])
                    return false;
        return true;
    }
    private void OnWin()
    {
        PuzzleManager.Instance.PuzzleAdvancement();
    }
}
