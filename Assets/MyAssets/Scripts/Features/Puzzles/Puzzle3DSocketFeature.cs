using System;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Puzzle3DSocketFeature : PuzzleSocket
{
    private int nDepth = 1;
    private bool[] isPieceCorrect;
    private GameObject originalObject;
    private GameObject originalMeshObject;
    [SerializeField]
    private Material canHoverMat;
    [SerializeField]
    private Material cantHoverMat;

    // Start is called before the first frame update
    private void Start()
    {
        SetVolume(0.2f);
        GenerateAndPlaceSockets();
    }
    //================CONFIGURATION===================
    protected override void ImportParameters()
    {
        Puzzle3DFeature puzzlePiecesScript = puzzleObject.GetComponent<Puzzle3DFeature>();
        originalObject = puzzlePiecesScript.GetOriginalObject();
        originalMeshObject = puzzlePiecesScript.GetOriginalMeshObject();
        nRows = puzzlePiecesScript.GetNRows();
        nCols = puzzlePiecesScript.GetNCols();
        nDepth = puzzlePiecesScript.GetNDepth();
        titleToSet = puzzlePiecesScript.GetTitle();
        pieceScale = puzzlePiecesScript.GetPieceScale();
    }
    protected override void CalculateBounds()
    {
        Mesh m = originalMeshObject.GetComponent<MeshFilter>().mesh;
        bounds = new()
        {
            x = m.bounds.size.x / nCols,
            y = m.bounds.size.y / nRows,
            z = m.bounds.size.z / nDepth
        };
    }
    //================SOCKETS===================
    public override void GenerateAndPlaceSockets()
    {
        ImportParameters();
        //configure bounds
        CalculateBounds();
        //configure puzzle size
        Vector3 size = new(bounds.x * nCols, bounds.y * nRows, bounds.z * nDepth);
        SetPuzzleSize(size * pieceScale);
        //configure title
        SetTitle(titleToSet);
        PositionTitle(new Vector3(0, -bounds.y * 0.75f * nRows, 0) * pieceScale);
        //define bool matrix to evaluate win conditions
        isPieceCorrect = new bool[nRows * nCols * nDepth];
        sockets = new GameObject[nRows * nCols * nDepth];
        //configure puzzle sockets
        for (int k = 0; k < nDepth; k++)
        {
            for (int i = 0; i < nCols; i++)
            {
                for (int j = 0; j < nRows; j++)
                {
                    int index = nCols * nRows * k + nRows * i + j;
                    GameObject socket = GeneratePuzzleSocket(originalObject, index);
                    PlaceSocketAt(ref socket, CalculateOffsetVec(i, j, k));
                    sockets[index] = socket;
                }
            }
        }
    }
    protected override Vector3 CalculateOffsetVec(int i, int j, int k)
    {
        return new Vector3(CalculateOffsetForSocketCenter(nCols, i) * transform.localScale.x, CalculateOffsetForSocketCenter(nRows, j) * transform.localScale.y, CalculateOffsetForSocketCenter(nDepth, k) * transform.localScale.z);
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
        box.size = new Vector3(bounds.x / reduceVec.x, bounds.y / reduceVec.y, bounds.z / reduceVec.z);
        box.isTrigger = true;
        //setup attachPoint
        GameObject attachPoint = new("Attach Point");
        AddAttachPoint(socket, ref attachPoint, originalMeshObject.GetComponent<MeshFilter>().mesh);
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
        //socket back
        var back = Instantiate(socketBack);
        back.transform.position = socket.transform.position;
        back.transform.localScale = bounds;
        back.transform.parent = socket.transform;
        //change transform and attach to parent
        socket.transform.localScale = pieceScale * Vector3.one;
        socket.transform.position += transform.position;
        socket.transform.parent = transform;
        //create child game object to show
        return socket;
    }
    void AddAttachPoint(GameObject socket, ref GameObject attachPoint, Mesh puzzleMesh)
    {
        attachPoint.transform.position = new(0, -bounds.y / 2, 0);
        attachPoint.transform.parent = socket.transform;
    }
    //================CHECK AND HANDLE WIN===================
    private void CheckPieceCorrect(XRSocketInteractor socket, int index)
    {
        isPieceCorrect[index] = socket.isSelectActive && socket.name == socket.interactablesSelected[0].transform.name;
        CheckWin();
    }
    private bool TestWin()
    {
        for (int k = 0; k < nDepth; k++)
            for (int i = 0; i < nCols; i++)
                for (int j = 0; j < nRows; j++)
                    if (!isPieceCorrect[nCols * nRows * k + nRows * i + j])
                        return false;
        return true;
    }
    private void CheckWin()
    {
        if (TestWin())
            OnWin();
    }
    private void OnWin()
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
        o.transform.localScale = pieceScale * Vector3.one;
    }
    protected override GameObject GeneratePuzzleSocket(Sprite puzzlePiece, int index)
    {
        throw new System.NotImplementedException();
    }
}

