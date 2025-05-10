using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Puzzle3DSocket : PuzzleSocket
{
    private PuzzleData3D puzzleData3D;
    private GameObject objectToRender;
    public PuzzleData3D PuzzleData3D { get => puzzleData3D; set => puzzleData3D = value; }
    public GameObject ObjectToRender { get => objectToRender; set => objectToRender = value; }
    public void Initialize(Puzzle3D feat)
    {
        PuzzleData = feat.PuzzleData;
        PuzzleData3D = feat.PuzzleData3D;
        ObjectToRender = feat.ObjectToRender;
        GenerateAndPlaceSockets();
    }

    //================CONFIGURATION===================
    protected override void CalculateBounds()
    {
        Mesh m = ObjectToRender.GetComponent<MeshFilter>().mesh;
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
                    GameObject socket = GeneratePuzzleSocket(ObjectToRender, index, i, j, k);
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
    protected override GameObject GeneratePuzzleSocket(GameObject puzzlePiece, int index, int i, int j, int k)
    {
        // 1. Create the socket game object.
        GameObject socket = new ($"{puzzlePiece.name}-tile{index}");

        // 2. Add required components.
        BoxCollider box = socket.AddComponent<BoxCollider>();
        XRSocketInteractor xRSocketInteractor = socket.AddComponent<XRSocketInteractor>();

        // 3. Configure the BoxCollider.
        box.size = new Vector3(Bounds.x / 4f, Bounds.y / 4f, Bounds.z / 4f);
        box.isTrigger = true;

        // 4. Create and configure attach point at the center of cell (i, j, k).
        GameObject attachPoint = new ("Attach Point");
        AddAttachPoint(socket, attachPoint, i, j, k);

        // 5. Set up the XRSocketInteractor to attach to that point.
        xRSocketInteractor.hoverSocketSnapping = true;
        xRSocketInteractor.interactionLayers = LayerMask.GetMask("Puzzle3D");
        xRSocketInteractor.interactableCantHoverMeshMaterial = cantHoverMat;
        xRSocketInteractor.interactableHoverMeshMaterial = canHoverMat;
        xRSocketInteractor.attachTransform = attachPoint.transform;

        // 6. Add listeners for puzzle-logic events.
        xRSocketInteractor.selectEntered.AddListener((s) => CheckPieceCorrect(xRSocketInteractor, index));
        xRSocketInteractor.selectExited.AddListener((s) => UpdateMatrix(xRSocketInteractor, index));

        // 7. (Optional) Add a visible "back" to your puzzle socket.
        var back = Instantiate(socketBack, socket.transform);
        back.transform.localScale = Bounds;
        back.transform.parent = socket.transform;

        // 8. Scale & position the entire socket, then parent it.
        socket.transform.localScale = PuzzleData.PieceScale * Vector3.one;
        socket.transform.position += transform.position;
        socket.transform.parent = transform;

        return socket;
    }

    private void AddAttachPoint(GameObject socket, GameObject attachPoint, int i, int j, int k)
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
        // 1. Create object and get mesh
        GameObject o = new(ObjectToRender.name);
        var mesh = ObjectToRender.GetComponent<MeshFilter>().mesh;

        // 2. Add components
        var mf = o.AddComponent<MeshFilter>(); mf.mesh = mesh;
        var mr = o.AddComponent<MeshRenderer>(); mr.material = ObjectToRender.GetComponent<MeshRenderer>().material;
        var rb = o.AddComponent<Rigidbody>();
        var box = o.AddComponent<BoxCollider>();
        var grab = o.AddComponent<XRGrabInteractable>();
        o.AddComponent<DontFallUnderFloor>();

        // 3. Configure XRGrabInteractable
        grab.interactionLayers = LayerMask.GetMask("Grabbable");
        grab.farAttachMode = InteractableFarAttachMode.Near;
        grab.selectMode = InteractableSelectMode.Single;

        // 4. Set attach point
        var attachPoint = new GameObject("Attach Point");
        attachPoint.transform.position = new Vector3(0, mesh.bounds.center.y - mesh.bounds.extents.y, 0);
        attachPoint.transform.parent = o.transform;
        grab.attachTransform = attachPoint.transform;

        // 5. Rigidbody & collider setup
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.None;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        box.size = mesh.bounds.size * 0.9f;
        box.center = mesh.bounds.center;

        // 6. Position and scale
        o.transform.position = transform.position;
        o.transform.localScale = PuzzleData.PieceScale * Vector3.one;
    }

    protected override GameObject GeneratePuzzleSocket(Sprite puzzlePiece, int index)
    {
        throw new System.NotImplementedException();
    }
}

