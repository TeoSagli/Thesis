using Meta.XR.MRUtilityKit;
using System.IO;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Puzzle2DSocket : PuzzleSocket
{

    private PuzzleData2D puzzleData2D;
    private Sprite spriteToRender;
    public Sprite SpriteToRender { get => spriteToRender; set => spriteToRender = value; }
    public PuzzleData2D PuzzleData2D { get => puzzleData2D; set => puzzleData2D = value; }

    public Puzzle2DSocket(float pieceScale, string titleStr, int nCols, int nRows, string path,string name) : base(pieceScale, titleStr, nCols, nRows)
    {
        puzzleData2D = new(path,name);
    }
    public void Initialize(Puzzle2D feat)
    {
        PuzzleData = feat.PuzzleData;
        PuzzleData2D = feat.PuzzleData2D;
        spriteToRender = LoadFromPath(PuzzleData2D.SpritePath, PuzzleData2D.SpriteName);
        GenerateAndPlaceSockets();
    }
    //================CONFIGURATION===================

    
    protected override void CalculateBounds()
    {
        Bounds = new()
        {
            x = SpriteToRender.bounds.size.x / PuzzleData.NCols,
            y = SpriteToRender.bounds.size.y / PuzzleData.NRows,
            z = SpriteToRender.bounds.size.z,
        };
    }
    //================GENERATE SOCKET===================
    public override void GenerateAndPlaceSockets()
    {
      
        //configure bounds
        CalculateBounds();
        //configure puzzle size
        Vector3 size = new(Bounds.x * PuzzleData.NCols, Bounds.y * PuzzleData.NRows, transform.localScale.z);
        SetPuzzleSize(size * PuzzleData.PieceScale);
        //configure title
        SetTitle(PuzzleData.TitleStr);
        PositionTitle(new Vector3(0, -Bounds.y * 0.75f * PuzzleData.NRows, 0) * PuzzleData.PieceScale);
        //define bool matrix to evaluate win conditions
        isPieceCorrect = new bool[PuzzleData.NRows * PuzzleData.NCols];
        sockets = new GameObject[PuzzleData.NRows * PuzzleData.NCols];
        //configure puzzle sockets
        for (int i = 0; i < PuzzleData.NCols; i++)
        {
            for (int j = 0; j < PuzzleData.NRows; j++)
            {
                int index = PuzzleData.NRows * i + (PuzzleData.NRows - j - 1);
                GameObject socket = GeneratePuzzleSocket(SpriteToRender, index);
                PlaceSocketAt(ref socket, CalculateOffsetVec(i, j, 0));
                sockets[index] = socket;
            }
        }
    }
    protected override Vector3 CalculateOffsetVec(int i, int j, int k)
    {
        return new Vector3(CalculateOffsetForSocketCenter(PuzzleData.NCols, i) * transform.localScale.x, CalculateOffsetForSocketCenter(PuzzleData.NRows, j) * transform.localScale.y, 0);
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
        box.size = new Vector3(Bounds.x / reduceVec.x, Bounds.y / reduceVec.y, Bounds.z / reduceVec.z);
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
        xRSocketInteractor.selectExited.AddListener((s) =>
        {
            UpdateMatrix(xRSocketInteractor, index);
        });
        //socket back
        var back = Instantiate(socketBack);
        back.transform.position = socket.transform.position;
        back.transform.localScale = Bounds;
        back.transform.parent = socket.transform;
        //change transform and attach to parent
        socket.transform.position += transform.position;
        socket.transform.localScale = PuzzleData.PieceScale * Vector3.one;
        socket.transform.parent = transform;
        //create child game object to show
        return socket;
    }
    void AddAttachPoint(GameObject socket, ref GameObject attachPoint)
    {
        Vector3 offset = new()
        {
            x = 0,
            y = -Bounds.y / 2,
            z = -0.2f,
        };
        attachPoint.transform.Translate(offset);
        attachPoint.transform.parent = socket.transform;
    }
    //================EVALUATE PUZZLE COMPLETION===================
    protected override bool TestWin()
    {
        for (int i = 0; i < PuzzleData.NRows; i++)
            for (int j = 0; j < PuzzleData.NCols; j++)
                if (!isPieceCorrect[i * PuzzleData.NCols + j])
                    return false;
        return true;
    }
    protected override void OnWin()
    {
        PuzzleManager.Instance.PuzzleAdvancement();
        DisableAllSockets();
        ReplaceWithSprite();
    }
    private void ReplaceWithSprite()
    {
        GameObject s = new(SpriteToRender.name);
        var sr = s.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteToRender;
        s.transform.position = transform.position + new Vector3(0, 0, -0.01f);
        s.transform.localScale = PuzzleData.PieceScale * Vector3.one;
        s.transform.parent = transform;
    }

    protected override GameObject GeneratePuzzleSocket(GameObject puzzlePiece, int index)
    {
        throw new System.NotImplementedException();
    }
}
