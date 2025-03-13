using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Puzzle2DSocketFeature : PuzzleSocket
{
    private bool[] isPieceCorrect;
    private Sprite originalSprite;

    // Start is called before the first frame update
    private void Start()
    {
        SetVolume(0.2f);
        GenerateAndPlaceSockets();
    }
    //================CONFIGURATION===================
    protected override void ImportParameters()
    {
        Puzzle2DFeature puzzlePiecesScript = puzzleObject.GetComponent<Puzzle2DFeature>();
        originalSprite = puzzlePiecesScript.GetOriginalSprite();
        nRows = puzzlePiecesScript.GetNRows();
        nCols = puzzlePiecesScript.GetNCols();
        titleToSet = puzzlePiecesScript.GetTitle();
        pieceScale = puzzlePiecesScript.GetPieceScale();
    }
    protected override void CalculateBounds()
    {
        bounds.x = originalSprite.bounds.size.x / nCols;
        bounds.y = originalSprite.bounds.size.y / nRows;
        bounds.z = originalSprite.bounds.size.z;
    }
    //================SOCKETS===================
    public override void GenerateAndPlaceSockets()
    {
        ImportParameters();
        //configure bounds
        CalculateBounds();
        //configure puzzle size
        Vector3 size = new(bounds.x * nCols, bounds.y * nRows, transform.localScale.z);
        SetPuzzleSize(size * pieceScale);
        //configure title
        SetTitle(titleToSet);
        PositionTitle(new Vector3(0, -bounds.y * 0.75f * nRows, 0) * pieceScale);
        //define bool matrix to evaluate win conditions
        isPieceCorrect = new bool[nRows * nCols];
        sockets = new GameObject[nRows * nCols];
        //configure puzzle sockets
        for (int i = 0; i < nCols; i++)
        {
            for (int j = 0; j < nRows; j++)
            {
                int index = nRows * i + (nRows - j - 1);
                GameObject socket = GeneratePuzzleSocket(originalSprite, index);
                PlaceSocketAt(ref socket, CalculateOffsetVec(i, j, 0));
                sockets[index] = socket;
            }
        }
    }
    protected override Vector3 CalculateOffsetVec(int i, int j, int k)
    {
        return new Vector3(CalculateOffsetForSocketCenter(nCols, i) * transform.localScale.x, CalculateOffsetForSocketCenter(nRows, j) * transform.localScale.y, 0);
    }
    protected override void PlaceSocketAt(ref GameObject socket, Vector3 offsetVec)
    {
        socket.transform.Translate(offsetVec);
    }

    protected override GameObject GeneratePuzzleSocket(Sprite puzzlePiece, int index)
    {
        GameObject socket = new(puzzlePiece.name + "-tile" + index);
        BoxCollider box = socket.AddComponent(typeof(BoxCollider)) as BoxCollider;
        XRSocketInteractor xRSocketInteractor = socket.AddComponent(typeof(XRSocketInteractor)) as XRSocketInteractor;

        //setup box collider
        Vector3 reduceVec = new(2, 2, 1);
        box.size = new Vector3(bounds.x / reduceVec.x, bounds.y / reduceVec.y, bounds.z / reduceVec.z);
        box.isTrigger = true;

        //setup attachPoint
        GameObject attachPoint = new("Attach Point");
        AddAttachPoint(socket, ref attachPoint);
        //socket interactor
        xRSocketInteractor.hoverSocketSnapping = true;
        xRSocketInteractor.interactionLayers = LayerMask.GetMask("Puzzle2D");
        xRSocketInteractor.attachTransform = attachPoint.transform;
        xRSocketInteractor.selectEntered.AddListener((s) =>
        {
            //PlayOnStarted();
            CheckPieceCorrect(xRSocketInteractor, index);
        });
        //socket back
        var back = Instantiate(socketBack);
        back.transform.position = socket.transform.position;
        back.transform.localScale = bounds;
        back.transform.parent = socket.transform;
        //change transform and attach to parent
        socket.transform.position += transform.position;
        socket.transform.localScale = pieceScale * Vector3.one;
        socket.transform.parent = transform;
        //create child game object to show
        return socket;
    }
    void AddAttachPoint(GameObject socket, ref GameObject attachPoint)
    {
        Vector3 offset = new()
        {
            x = 0,
            y = -bounds.y / 2,
            z = -0.2f,
        };
        attachPoint.transform.Translate(offset);
        attachPoint.transform.parent = socket.transform;
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
        DisableAllSockets();
        ReplaceWithSprite();
    }
    private void ReplaceWithSprite()
    {
        GameObject s = new(originalSprite.name);
        var sr = s.AddComponent<SpriteRenderer>();
        sr.sprite = originalSprite;
        s.transform.position = transform.position + new Vector3(0, 0, -0.01f);
        s.transform.localScale = pieceScale * Vector3.one;
        s.transform.parent = transform;
    }
    protected override GameObject GeneratePuzzleSocket(GameObject puzzlePiece, int index)
    {
        throw new System.NotImplementedException();
    }
}
