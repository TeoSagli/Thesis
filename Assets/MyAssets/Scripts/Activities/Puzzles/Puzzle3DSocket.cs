using System;
using System.Linq;
using Meta.XR.MRUtilityKit;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Puzzle3DSocket : PuzzleSocket
{
    private PuzzleData3D puzzleData3D;
    private GameObject originalObject;
    private GameObject originalMeshObject;

    public PuzzleData3D PuzzleData3D { get => puzzleData3D; set => puzzleData3D = value; }

    public Puzzle3DSocket(float pieceScale, string titleStr, int nCols, int nRows, int nDepth) : base(pieceScale, titleStr, nCols, nRows, nDepth)
    {
    }
    public void Initialize(Puzzle3D feat)
    {
        PuzzleData = feat.PuzzleData;
        PuzzleData3D = feat.PuzzleData3D;
        
        GenerateAndPlaceSockets();
    }
    // Start is called before the first frame update
    private void Start()
    {
        if (MRUK.Instance)
            MRUK.Instance.RegisterSceneLoadedCallback(() =>
            {
                GenerateAndPlaceSockets();
            });
    }
    //================CONFIGURATION===================
    protected override void CalculateBounds()
    {
        Mesh m = originalMeshObject.GetComponent<MeshFilter>().mesh;
        Bounds = new()
        {
            x = m.bounds.size.x / PuzzleData.NCols,
            y = m.bounds.size.y / PuzzleData.NRows,
            z = m.bounds.size.z / PuzzleData.NDepth
        };
    }
    //================GENERATE SOCKET===================
    public override void GenerateAndPlaceSockets()
    {
        //configure bounds
        CalculateBounds();
        //configure puzzle size
        Vector3 size = new(Bounds.x * PuzzleData.NCols, Bounds.y * PuzzleData.NRows, Bounds.z * PuzzleData.NDepth);
        SetPuzzleSize(size * PuzzleData.PieceScale);
        //configure title
        SetTitle(PuzzleData.TitleStr);
        PositionTitle(new Vector3(0, -Bounds.y * 0.75f * PuzzleData.NRows, 0) * PuzzleData.PieceScale);
        //define bool matrix to evaluate win conditions
        isPieceCorrect = new bool[PuzzleData.NRows * PuzzleData.NCols * PuzzleData.NDepth];
        sockets = new GameObject[PuzzleData.NRows * PuzzleData.NCols * PuzzleData.NDepth];
        //configure puzzle sockets
        for (int k = 0; k < PuzzleData.NDepth; k++)
        {
            for (int i = 0; i < PuzzleData.NCols; i++)
            {
                for (int j = 0; j < PuzzleData.NRows; j++)
                {
                    int index = PuzzleData.NCols * PuzzleData.NRows * k + PuzzleData.NRows * i + j;
                    GameObject socket = GeneratePuzzleSocket(originalObject, index);
                    PlaceSocketAt(ref socket, CalculateOffsetVec(i, j, k));
                    sockets[index] = socket;
                }
            }
        }
    }
    protected override Vector3 CalculateOffsetVec(int i, int j, int k)
    {
        return new Vector3(CalculateOffsetForSocketCenter(PuzzleData.NCols, i) * transform.localScale.x, CalculateOffsetForSocketCenter(PuzzleData.NRows, j) * transform.localScale.y, CalculateOffsetForSocketCenter(PuzzleData.NDepth, k) * transform.localScale.z);
    }
    protected override void PlaceSocketAt(ref GameObject socket, Vector3 offsetVec)
    {
        socket.transform.Translate(offsetVec);
    }
    protected override GameObject GeneratePuzzleSocket(GameObject puzzlePiece, int index)
    {
        GameObject socket = new(puzzlePiece.name + "-tile" + index);
        BoxCollider box = socket.AddComponent(typeof(BoxCollider)) as BoxCollider;
        XRSocketInteractor xRSocketInteractor = socket.AddComponent(typeof(XRSocketInteractor)) as XRSocketInteractor;

        //setup box collider
        Vector3 reduceVec = new(4, 4, 4);
        box.size = new Vector3(Bounds.x / reduceVec.x, Bounds.y / reduceVec.y, Bounds.z / reduceVec.z);
        box.isTrigger = true;
        //setup attachPoint
        GameObject attachPoint = new("Attach Point");
        AddAttachPoint(socket, ref attachPoint);
        //socket interactor
        xRSocketInteractor.hoverSocketSnapping = true;
        xRSocketInteractor.interactionLayers = LayerMask.GetMask("Puzzle3D");
        xRSocketInteractor.interactableCantHoverMeshMaterial = cantHoverMat;
        xRSocketInteractor.interactableHoverMeshMaterial = canHoverMat;
        xRSocketInteractor.attachTransform = attachPoint.transform;
        xRSocketInteractor.selectEntered.AddListener((s) =>
        {
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
        socket.transform.localScale = PuzzleData.PieceScale * Vector3.one;
        socket.transform.position += transform.position;
        socket.transform.parent = transform;
        return socket;
    }
    void AddAttachPoint(GameObject socket, ref GameObject attachPoint)
    {
        attachPoint.transform.position = new(0, -Bounds.y / PuzzleData.NRows, 0);
        attachPoint.transform.parent = socket.transform;
    }
    //================EVALUATE PUZZLE COMPLETION===================
    protected override bool TestWin()
    {
        for (int k = 0; k < PuzzleData.NDepth; k++)
            for (int i = 0; i < PuzzleData.NCols; i++)
                for (int j = 0; j < PuzzleData.NRows; j++)
                    if (!isPieceCorrect[PuzzleData.NCols * PuzzleData.NRows * k + PuzzleData.NRows * i + j])
                        return false;
        return true;
    }
    protected override void OnWin()
    {
        PuzzleManager.Instance.PuzzleAdvancement();
        DisableAllSockets();
        ReplaceWithObject();
    }
    private void ReplaceWithObject()
    {
        GameObject o = new(originalObject.name);
        var m = originalMeshObject.GetComponent<MeshFilter>().mesh;
        var mf = o.AddComponent<MeshFilter>();
        var mr = o.AddComponent<MeshRenderer>();
        var rb = o.AddComponent<Rigidbody>();
        var box = o.AddComponent<BoxCollider>();
        var grab = o.AddComponent<XRGrabInteractable>();
        o.AddComponent<DontFallUnderFloor>();

        grab.interactionLayers = LayerMask.GetMask("Grabbable");
        grab.farAttachMode = InteractableFarAttachMode.Near;
        GameObject attachPoint = new("Attach Point");
        Vector3 offset = new()
        {
            y = m.bounds.center.y - m.bounds.extents.y,
        };
        attachPoint.transform.position = offset;
        attachPoint.transform.parent = o.transform;
        grab.attachTransform = attachPoint.transform;
        grab.selectMode = InteractableSelectMode.Single;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.None;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        box.size = m.bounds.size * 0.9f;
        box.center = m.bounds.center;
        mf.mesh = m;
        mr.material = originalMeshObject.GetComponent<MeshRenderer>().material;
        o.transform.position = transform.position;
        o.transform.localScale = PuzzleData.PieceScale * Vector3.one;
    }
    protected override GameObject GeneratePuzzleSocket(Sprite puzzlePiece, int index)
    {
        throw new System.NotImplementedException();
    }
}

