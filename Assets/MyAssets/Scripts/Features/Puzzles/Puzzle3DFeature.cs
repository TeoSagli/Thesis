using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Puzzle3DFeature : PuzzlePiece
{
    [SerializeField]
    private int nDepth = 1;
    [SerializeField, Header("Object params")]
    private GameObject objectToRender;
    [SerializeField]
    private GameObject objectMesh;

    private void Start()
    {
        if (MRUK.Instance)
            MRUK.Instance.RegisterSceneLoadedCallback(() =>
            {
                ExtractGridMesh();
                SpawnRndPuzzlePieces();
            });
    }
    // EXTRACTION PROCESS
    public void ExtractGridMesh()
    {
        CalculateBounds();
        puzzlePiecesArr = new GameObject[nCols * nRows * nDepth];
        MeshFilter meshFilter = objectMesh.GetComponent<MeshFilter>();
        Mesh originalMesh = meshFilter.sharedMesh;
        // Get the mesh bounds
        Bounds bounds = originalMesh.bounds;
        Vector3 minBounds = bounds.min;
        Vector3 maxBounds = bounds.max;

        // Calculate grid step size
        Vector3 stepSize = new(
            (maxBounds.x - minBounds.x) / nCols,
            (maxBounds.y - minBounds.y) / nRows,
            (maxBounds.z - minBounds.z) / nDepth
        );
        // Loop through each grid cell
        for (int z = 0; z < nDepth; z++)
        {
            for (int x = 0; x < nCols; x++)
            {
                for (int y = 0; y < nRows; y++)
                {
                    int contTiles = z * nCols * nRows + x * nRows + y;
                    Vector3 cubeMin = minBounds + new Vector3(x * stepSize.x, y * stepSize.y, z * stepSize.z);
                    Vector3 cubeMax = cubeMin + stepSize;
                    Mesh tileMesh = ExtractCubeMesh(originalMesh.vertices, originalMesh.triangles, cubeMin, cubeMax);
                    puzzlePiecesArr[contTiles] = GeneratePuzzlePiece(tileMesh, objectToRender.name + $"-tile{contTiles}", CalculateOffsetVec(x, y, z), y, x);
                }
            }
        }
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
    private Texture2D ExtractTexture(Sprite sprite)
    {
        var croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);

        // var pixels = sprite.texture.GetPixels32();
        //   croppedTexture.SetPixels32(pixels);
        var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                           (int)sprite.textureRect.y,
                                           (int)sprite.textureRect.width,
                                           (int)sprite.textureRect.height);

        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();
        return croppedTexture;
    }
    private Mesh ExtractCubeMesh(Vector3[] originalVertices, int[] originalTriangles, Vector3 cubeMin, Vector3 cubeMax)
    {
        List<Vector3> newVertices = new();
        List<int> newTriangles = new();
        Dictionary<int, int> vertexMap = new();
        List<Vector2> newUVs = new();
        Vector2[] originalUVs = objectMesh.GetComponent<MeshFilter>().mesh.uv;

        // Find vertices inside the cube
        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            List<int> insideIndices = new();

            for (int j = 0; j < 3; j++)
            {
                int index = originalTriangles[i + j];
                Vector3 vertex = originalVertices[index];

                if (vertex.x >= cubeMin.x && vertex.x <= cubeMax.x &&
                    vertex.y >= cubeMin.y && vertex.y <= cubeMax.y &&
                    vertex.z >= cubeMin.z && vertex.z <= cubeMax.z)
                {
                    if (!vertexMap.ContainsKey(index))
                    {
                        vertexMap[index] = newVertices.Count;
                        newVertices.Add(vertex);
                        newUVs.Add(originalUVs[index]);
                    }
                    insideIndices.Add(vertexMap[index]);
                }
            }

            // Only add triangles if all three vertices are inside the cube
            if (insideIndices.Count == 3)
            {
                newTriangles.AddRange(insideIndices);
            }
        }

        // If no triangles were found, skip creating a new mesh
        if (newTriangles.Count == 0) Debug.Log("No triangles");

        // Create new mesh
        Mesh newMesh = new()
        {
            vertices = newVertices.ToArray(),
            triangles = newTriangles.ToArray(),
            name = "Mesh",
            uv = newUVs.ToArray(),
        };
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();
        return newMesh;
    }
    private Vector2[] GenerateDefaultUVs(Mesh mesh)
    {
        Vector2[] uvs = new Vector2[mesh.vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            Vector3 vertex = mesh.vertices[i];
            uvs[i] = new Vector2(vertex.x, vertex.z); // Basic planar UV mapping
        }
        return uvs;
    }
    // PUZZLE PIECES' GENERATION PROCESS
    private GameObject GeneratePuzzlePiece(Mesh mesh, string name, Vector3 toTrans, int i, int j)
    {
        GameObject piece = new(name);
        MeshRenderer mr = piece.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        MeshFilter mf = piece.AddComponent(typeof(MeshFilter)) as MeshFilter;
        Rigidbody rb = piece.AddComponent(typeof(Rigidbody)) as Rigidbody;
        BoxCollider box = piece.AddComponent(typeof(BoxCollider)) as BoxCollider;
        DontFallUnderFloor script = piece.AddComponent(typeof(DontFallUnderFloor)) as DontFallUnderFloor;
        XRGrabInteractable xrGrabInteractable = piece.AddComponent(typeof(XRGrabInteractable)) as XRGrabInteractable;
        //mesh
        MeshRenderer mrOriginal = objectMesh.GetComponent<MeshRenderer>();
        mf.mesh = mesh;
        Material meshMat = new(mrOriginal.material);
        float w = bounds.x * 100; //tot 1000
        float h = bounds.y * 100; //tot 1330
        Vector2 offset = new()
        {
            x = j * w,
            y = i * h,
        };
        //  meshMat.SetTextureScale("_DetailAlbedoMap", Vector2.one / (nCols * nRows * nDepth));
        //   meshMat.SetTextureOffset("_DetailAlbedoMap", offset);
        mr.material = mrOriginal.material;
        //setup box collider
        box.size = mesh.bounds.size;
        box.center = mesh.bounds.center;
        //setup rigidBody
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.None;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        //attach
        GameObject attachPoint = new("Attach Point");
        AddAttachPoint(piece, ref attachPoint, mesh, toTrans);
        //grab interactable
        xrGrabInteractable.interactionLayers = LayerMask.GetMask("Puzzle3D");
        xrGrabInteractable.farAttachMode = InteractableFarAttachMode.Near;
        xrGrabInteractable.attachTransform = attachPoint.transform;
        xrGrabInteractable.selectMode = InteractableSelectMode.Single;
        //change transform and attach to parent
        piece.transform.localScale = pieceScale * Vector3.one;
        piece.transform.parent = transform;
        return piece;
    }
    protected override void CalculateBounds()
    {
        Mesh m = objectMesh.GetComponent<MeshFilter>().mesh;
        bounds = new()
        {
            x = m.bounds.size.x / nCols,
            y = m.bounds.size.y / nRows,
            z = m.bounds.size.z / nDepth
        };
    }
    protected Vector3 CalculateOffsetVec(int i, int j, int k)
    {
        return new Vector3(CalculateOffsetForSocketCenter(nCols, i) * transform.localScale.x, CalculateOffsetForSocketCenter(nRows, j) * transform.localScale.y, CalculateOffsetForSocketCenter(nDepth, k) * transform.localScale.z);
    }
    void AddAttachPoint(GameObject obj, ref GameObject attachPoint, Mesh puzzleMesh, Vector3 toTrans)
    {
        Vector3 centerObj = puzzleMesh.bounds.center;
        Vector3 mismatch = bounds / 2 - puzzleMesh.bounds.extents;
        Vector3 offset = new()
        {
            x = toTrans.x > 0 ? mismatch.x : -mismatch.x,
            y = (toTrans.y < 0 ? -mismatch.y : mismatch.y) - bounds.y / nRows,
            z = toTrans.z > 0 ? mismatch.z : -mismatch.z,
        };
        if (toTrans.x == 0) offset.x = 0;
        if (toTrans.y == 0) offset.y = -bounds.y / nRows;
        if (toTrans.z == 0) offset.z = 0;

        attachPoint.transform.position = offset;
        attachPoint.transform.position += centerObj;
        attachPoint.transform.parent = obj.transform;
    }
    // GETTERS
    public GameObject GetOriginalObject()
    {
        return objectToRender;
    }
    public int GetNDepth()
    {
        return nDepth;
    }
    public GameObject GetOriginalMeshObject()
    {
        return objectMesh;
    }

}
