using Meta.XR.MRUtilityKit;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Puzzle2DFeature : PuzzlePiece
{
    [SerializeField, Header("Sprite params")]
    private Shader spriteShader;
    [SerializeField]
    private Sprite spriteToRender;
    private void Start()
    {
        if (MRUK.Instance)
            MRUK.Instance.RegisterSceneLoadedCallback(() =>
            {
                ExtractAndGeneratePieces();
                SpawnRndPuzzlePieces();
            });
    }
    private void ExtractAndGeneratePieces()
    {
        CalculateBounds();
        puzzlePiecesArr = new GameObject[nCols * nRows];
        for (int j = 0; j < nCols; j++)
        {
            for (int i = 0; i < nRows; i++)
            {
                int contTiles = j * nRows + i;
                Sprite puzzlePiece = SpriteExtractor(spriteToRender, nRows - 1 - i, j);
                puzzlePiecesArr[contTiles] = GeneratePuzzlePiece(puzzlePiece, spriteToRender.name + "-tile" + contTiles);
            }
        }
    }
    // EXTRACTION PROCESS
    protected override void CalculateBounds()
    {
        bounds = new()
        {
            x = spriteToRender.bounds.size.x / nCols,
            y = spriteToRender.bounds.size.y / nRows,
        };
    }
    private Sprite SpriteExtractor(Sprite sprite, int i, int j)
    {
        float w = bounds.x * 100; //tot 1000
        float h = bounds.y * 100; //tot 1330
        float x = j * w;
        float y = i * h;

        // Define the portion of the sprite to extract (x, y, width, height)
        Rect rect = new(x, y, w, h);
        // Create the new sprite
        Vector2 pivotDef = new(0.5f, 0.5f); //center
        Sprite s = Sprite.Create(sprite.texture, rect, pivotDef);
        return s;
    }
    // PUZZLE PIECES' GENERATION PROCESS
    private GameObject GeneratePuzzlePiece(Sprite puzzlePiece, string name)
    {
        GameObject piece = new(name);
        SpriteRenderer sr = piece.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        Rigidbody rb = piece.AddComponent(typeof(Rigidbody)) as Rigidbody;
        BoxCollider box = piece.AddComponent(typeof(BoxCollider)) as BoxCollider;
        DontFallUnderFloor script = piece.AddComponent(typeof(DontFallUnderFloor)) as DontFallUnderFloor;
        XRGrabInteractable xrGrabInteractable = piece.AddComponent(typeof(XRGrabInteractable)) as XRGrabInteractable;
        //setup box collider
        box.size = new Vector3(puzzlePiece.bounds.size.x, puzzlePiece.bounds.size.y, puzzlePiece.bounds.size.z);
        //setup renderer
        sr.sprite = puzzlePiece;
        sr.material.shader = spriteShader;
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
        piece.transform.localScale = pieceScale * Vector3.one;
        piece.transform.parent = transform;
        return piece;
    }
    void AddAttachPoint(GameObject obj, ref GameObject attachPoint, Sprite puzzlePiece)
    {
        float offset = -puzzlePiece.bounds.extents.y;
        attachPoint.transform.Translate(0, offset, 0);
        attachPoint.transform.parent = obj.transform;
    }
    // GETTERS
    public Sprite GetOriginalSprite()
    {
        return spriteToRender;
    }
}
