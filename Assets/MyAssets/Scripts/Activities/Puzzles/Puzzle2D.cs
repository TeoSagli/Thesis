using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[System.Serializable]
public class Puzzle2D : PuzzlePiece
{
    private PuzzleData2D puzzleData2D;
    private Sprite spriteToRender;
    public PuzzleData2D PuzzleData2D { get => puzzleData2D; set => puzzleData2D = value; }
    public Sprite SpriteToRender { get => spriteToRender; set => spriteToRender = value; }

    public Puzzle2D(float pieceScale, string titleStr, int nCols, int nRows, string path, string name) : base(pieceScale, titleStr, nCols, nRows)
    {

    }
    public void Init(float pieceScale, string titleStr, int nCols, int nRows, string path, string name)
    {
        PuzzleData = new(pieceScale, titleStr, nCols, nRows);
        PuzzleData2D = new(path, name);
        //SpriteToRender = LoadFromPath(PuzzleData2D.SpritePath, PuzzleData2D.SpriteName);
        if (name == "Scuola")
            SpriteToRender = ToDelete.Instance.S1;
        else
            SpriteToRender = ToDelete.Instance.S2;
        ExtractAndGeneratePieces();
        SpawnRndPuzzlePieces();
    }
    //================CONFIGURATION===================
    protected override void CalculateBounds()
    {
        Bounds = new()
        {
            x = SpriteToRender.bounds.size.x / PuzzleData.NCols,
            y = SpriteToRender.bounds.size.y / PuzzleData.NRows,
            z = 0
        };
    }
    private void ExtractAndGeneratePieces()
    {
        CalculateBounds();
        puzzlePiecesArr = new GameObject[PuzzleData.NCols * PuzzleData.NRows];
        for (int j = 0; j < PuzzleData.NCols; j++)
        {
            for (int i = 0; i < PuzzleData.NRows; i++)
            {
                int contTiles = j * PuzzleData.NRows + i;
                Sprite puzzlePiece = SpriteExtractor(SpriteToRender, PuzzleData.NRows - 1 - i, j);
                puzzlePiecesArr[contTiles] = GeneratePuzzlePiece(puzzlePiece, contTiles);
            }
        }
    }

    //================EXTRACT PIECES===================
    private Sprite SpriteExtractor(Sprite sprite, int i, int j)
    {
        float w = Bounds.x * 100; //tot 1000
        float h = Bounds.y * 100; //tot 1330
        float x = j * w;
        float y = i * h;

        // Define the portion of the sprite to extract (x, y, width, height)
        Rect rect = new(x, y, w, h);
        // Create the new sprite
        Vector2 pivotDef = new(0.5f, 0.5f); //center
        Sprite s = Sprite.Create(sprite.texture, rect, pivotDef);
        return s;
    }
    //================GENERATE PIECE===================
    private GameObject GeneratePuzzlePiece(Sprite puzzlePiece, int contTiles)
    {
        GameObject piece = new($"{SpriteToRender.name}-tile{contTiles}");
        SpriteRenderer sr = piece.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        Rigidbody rb = piece.AddComponent(typeof(Rigidbody)) as Rigidbody;
        BoxCollider box = piece.AddComponent(typeof(BoxCollider)) as BoxCollider;
        DontFallUnderFloor script = piece.AddComponent(typeof(DontFallUnderFloor)) as DontFallUnderFloor;
        XRGrabInteractable xrGrabInteractable = piece.AddComponent(typeof(XRGrabInteractable)) as XRGrabInteractable;
        //setup box collider
        box.size = new Vector3(puzzlePiece.bounds.size.x, puzzlePiece.bounds.size.y, puzzlePiece.bounds.size.z);
        //setup renderer
        sr.sprite = puzzlePiece;
        sr.material.shader = Shader.Find("Sprites/Default");
        //setup rigidBody
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.None;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        //setup attachPoint
        GameObject attachPoint = new("Attach Point");
        AddAttachPoint(piece, ref attachPoint, puzzlePiece);
        //grab interactable
        xrGrabInteractable.interactionLayers = LayerMask.GetMask("Puzzle2D");
        xrGrabInteractable.farAttachMode = InteractableFarAttachMode.Near;
        xrGrabInteractable.attachTransform = attachPoint.transform;
        xrGrabInteractable.selectMode = InteractableSelectMode.Single;
        //change transform and attach to parent
        piece.transform.localScale = PuzzleData.PieceScale * Vector3.one;
        piece.transform.parent = transform;
        return piece;
    }
    void AddAttachPoint(GameObject obj, ref GameObject attachPoint, Sprite puzzlePiece)
    {
        float offset = -puzzlePiece.bounds.extents.y;
        attachPoint.transform.Translate(0, offset, 0);
        attachPoint.transform.parent = obj.transform;
    }
    //================GETTERS===================
    public Sprite GetOriginalSprite()
    {
        return SpriteToRender;
    }
}
