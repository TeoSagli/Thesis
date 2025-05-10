using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Puzzle2DSocket : PuzzleSocket
{

    private PuzzleData2D puzzleData2D;
    private Sprite spriteToRender;
    public Sprite SpriteToRender { get => spriteToRender; set => spriteToRender = value; }
    public PuzzleData2D PuzzleData2D { get => puzzleData2D; set => puzzleData2D = value; }
    public void Initialize(Puzzle2D feat)
    {
        PuzzleData = feat.PuzzleData;
        PuzzleData2D = feat.PuzzleData2D;
        spriteToRender = feat.SpriteToRender;
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
        // 1. Create the socket GameObject
        var socket = new GameObject($"{puzzlePiece.name}-tile{index}");

        // 2. Add and configure BoxCollider
        var box = socket.AddComponent<BoxCollider>();
        box.size = new Vector3(Bounds.x / 2, Bounds.y / 2, Bounds.z); // reduced size
        box.isTrigger = true;

        // 3. Add and configure XRSocketInteractor
        var interactor = socket.AddComponent<XRSocketInteractor>();
        interactor.hoverSocketSnapping = true;
        interactor.interactionLayers = LayerMask.GetMask("Puzzle2D");

        // 4. Create attach point and assign
        var attachPoint = new GameObject("Attach Point");
        AddAttachPoint(socket, ref attachPoint);
        interactor.attachTransform = attachPoint.transform;

        // 5. Register interaction events
        interactor.selectEntered.AddListener(_ => CheckPieceCorrect(interactor, index));
        interactor.selectExited.AddListener(_ => UpdateMatrix(interactor, index));

        // 6. Create and attach socket back visual
        var back = Instantiate(socketBack, socket.transform.position, Quaternion.identity, socket.transform);
        back.transform.localScale = Bounds;

        // 7. Finalize transform and hierarchy
        socket.transform.position += transform.position;
        socket.transform.localScale = PuzzleData.PieceScale * Vector3.one;
        socket.transform.parent = transform;

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

    protected override GameObject GeneratePuzzleSocket(GameObject puzzlePiece, int index, int i, int j, int k)
    {
        throw new System.NotImplementedException();
    }
}
