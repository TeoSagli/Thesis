using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Puzzle3D : PuzzlePiece
{
    private GameObject objectToRender;
    private PuzzleData3D puzzleData3D;
    private Shader noCullShader;

    public PuzzleData3D PuzzleData3D { get => puzzleData3D; set => puzzleData3D = value; }
    public GameObject ObjectToRender { get => objectToRender; set => objectToRender = value; }
    public Shader NoCullShader { get => noCullShader; set => noCullShader = value; }

    public event Action<Puzzle3D> OnLoaded;
    public Puzzle3D(float pieceScale, string titleStr, int nCols, int nRows, int nDepth, string path, string name, Shader noCullShader) : base(pieceScale, titleStr, nCols, nRows, nDepth)
    {
        
        
    }

    public async void  Init(float pieceScale, string titleStr, int nCols, int nRows, int nDepth, string path, string name, Shader noCullShader)
    {
       PuzzleData = new(pieceScale, titleStr, nCols, nRows, nDepth);
       PuzzleData3D = new(path, name);
       NoCullShader = noCullShader;
       ObjectToRender = await LoadGLBFromBytes(PuzzleData3D.MeshPath, PuzzleData3D.MeshName);
       ExtractGridMesh();
       SpawnRndPuzzlePieces();
       OnLoaded?.Invoke(this);
    }
    private void CollectMeshes(Transform parent, List<MeshFilter> meshes)
    {
        // Check if the current transform has a MeshFilter
        if (parent.TryGetComponent<MeshFilter>(out var meshFilter))
        {
            meshes.Add(meshFilter);
        }

        // Recursively check all child transforms
        foreach (Transform child in parent)
        {
            CollectMeshes(child, meshes);
        }
    }
    //================EXTRACT PIECES===================
    public void ExtractGridMesh()
    {
        Mesh originalMesh;
        ObjectToRender = GenerateCombinedMeshObject();
        CalculateBounds();
        puzzlePiecesArr = new GameObject[PuzzleData.NCols * PuzzleData.NRows * PuzzleData.NDepth];

        originalMesh = objectToRender.GetComponent<MeshFilter>().mesh;
        // Get the mesh bounds
        Bounds bounds = originalMesh.bounds;
        Vector3 minBounds = bounds.min;
        Vector3 maxBounds = bounds.max;

        // Calculate grid step size
        Vector3 stepSize = new(
            (maxBounds.x - minBounds.x) / PuzzleData.NCols,
            (maxBounds.y - minBounds.y) / PuzzleData.NRows,
            (maxBounds.z - minBounds.z) / PuzzleData.NDepth
        );
        // Loop through each grid cell
        for (int z = 0; z < PuzzleData.NDepth; z++)
        {
            for (int x = 0; x < PuzzleData.NCols; x++)
            {
                for (int y = 0; y < PuzzleData.NRows; y++)
                {
                    int contTiles = z * PuzzleData.NCols * PuzzleData.NRows + x * PuzzleData.NRows + y;
                    Vector3 cubeMin = minBounds + new Vector3(x * stepSize.x, y * stepSize.y, z * stepSize.z);
                    Vector3 cubeMax = cubeMin + stepSize;
                    Mesh tileMesh = ExtractCubeMesh(cubeMin, cubeMax, originalMesh);
                    puzzlePiecesArr[contTiles] = GeneratePuzzlePiece(tileMesh, contTiles, CalculateOffsetVec(x, y, z),x,y,z);
                }
            }
        }
    }
    private GameObject GenerateCombinedMeshObject()
    {
        List<MeshFilter> meshes = new();
        CollectMeshes(ObjectToRender.transform, meshes);
        MeshFilter[] objectMultipleMesh = meshes.ToArray();

        Mesh originalMesh = new();
        GameObject combinedMeshObject = new(ObjectToRender.name);
        MeshRenderer mr = combinedMeshObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        MeshFilter mf = combinedMeshObject.AddComponent(typeof(MeshFilter)) as MeshFilter;

        CombineInstance[] combine = new CombineInstance[objectMultipleMesh.Length];
        originalMesh.indexFormat = IndexFormat.UInt32;
        if (objectMultipleMesh.Length > 1)
        {
            for (int i = 0; i < objectMultipleMesh.Length; i++)
            {
                MeshFilter mFilter = objectMultipleMesh[i].GetComponent<MeshFilter>();
                combine[i].mesh = mFilter.sharedMesh;
                combine[i].transform = mFilter.transform.localToWorldMatrix;
            }

            originalMesh.CombineMeshes(combine, true, true);
            mf.mesh = originalMesh;
        }
        else
        {
            MeshFilter mFilter = objectMultipleMesh[0].GetComponent<MeshFilter>();
            mf.mesh = mFilter.mesh;
        }
        MeshRenderer mRenderer = objectMultipleMesh[0].GetComponent<MeshRenderer>();
        mr.material.shader = mRenderer.material.shader;
        mr.material = mRenderer.material;
        combinedMeshObject.SetActive(false);
        return combinedMeshObject;
    }
    private Mesh ExtractCubeMesh(Vector3 cubeMin, Vector3 cubeMax, Mesh originalMesh)
    {
        Vector3[] originalVertices = originalMesh.vertices;
        int[] originalTriangles = originalMesh.triangles;
        Vector2[] originalUVs = originalMesh.uv;
        List<Vector3> newVertices = new();
        List<int> newTriangles = new();
        Dictionary<int, int> vertexMap = new();
        List<Vector2> newUVs = new();

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
    //================GENERATE PIECE===================
    private GameObject GeneratePuzzlePiece(Mesh mesh, int contTiles, Vector3 toTrans, int i, int j,int k)
    {
        // 1. Create the puzzle piece object & add basic components.
        GameObject piece = new ($"{ObjectToRender.name}-tile{contTiles}");
        MeshRenderer mr = piece.AddComponent<MeshRenderer>();
        MeshFilter mf = piece.AddComponent<MeshFilter>();
        Rigidbody rb = piece.AddComponent<Rigidbody>();
        BoxCollider box = piece.AddComponent<BoxCollider>();
        DontFallUnderFloor dontFall = piece.AddComponent<DontFallUnderFloor>();
        XRGrabInteractable xrGrab = piece.AddComponent<XRGrabInteractable>();

        // 2. Assign the mesh and materials.
        mf.mesh = mesh;
        mr.material.shader = noCullShader;
        mr.material.mainTexture = objectToRender.GetComponent<MeshRenderer>().material.mainTexture;

        // 3. Set up the BoxCollider with the mesh’s bounds.
        box.size = mesh.bounds.size;
        box.center = mesh.bounds.center;

        // 4. Configure the Rigidbody.
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.None;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // 5. Scale the entire piece to your puzzle’s scale.
        piece.transform.localScale = PuzzleData.PieceScale * Vector3.one;

        // 6. Create & set up the attach point so the pivot matches the puzzle socket center.
        GameObject attachPoint = new ("Attach Point");
        AddAttachPoint(piece, attachPoint, mesh, toTrans, i ,j, k);

        // 7. Configure the grab interactable to attach at the pivot.
        xrGrab.interactionLayers = LayerMask.GetMask("Puzzle3D");
        xrGrab.farAttachMode = InteractableFarAttachMode.Near;
        xrGrab.attachTransform = attachPoint.transform;
        xrGrab.selectMode = InteractableSelectMode.Single;

        // 8. Parent the piece to keep it within the puzzle's hierarchy.
        piece.transform.localScale = PuzzleData.PieceScale * Vector3.one;
        piece.transform.SetParent(transform, true);

        return piece;
    }
    private void AddAttachPoint(GameObject obj, GameObject attachPoint, Mesh puzzleMesh, Vector3 toTrans, int i, int j, int k)
    {
        Vector3 centerObj = puzzleMesh.bounds.center;
        Vector3 mismatch = Bounds / 2 - puzzleMesh.bounds.extents;
        Vector3 offset = new()
        {
            x = toTrans.x > 0 ? mismatch.x : -mismatch.x,
            y = (toTrans.y < 0 ? -mismatch.y : mismatch.y) - Bounds.y / PuzzleData.NRows,
            z = toTrans.z > 0 ? mismatch.z : -mismatch.z,
        };
        if (toTrans.x == 0) offset.x = 0;
        if (toTrans.y == 0) offset.y = -Bounds.y / PuzzleData.NRows;
        if (toTrans.z == 0) offset.z = 0;

        attachPoint.transform.parent = obj.transform;
        attachPoint.transform.localPosition = centerObj + offset;
    }
    private Vector3 ComputePieceCellPosition(int i, int j, int k)
    {
        Vector3 cellSize = new Vector3(
            Bounds.x / PuzzleData.NCols,
            Bounds.y / PuzzleData.NRows,
            Bounds.z / PuzzleData.NDepth
        );

        return new Vector3(
            (i + 0.5f) * cellSize.x - Bounds.x / 2f,
            (j + 0.5f) * cellSize.y - Bounds.y / 2f,
            (k + 0.5f) * cellSize.z - Bounds.z / 2f
        );
    }
    protected override void CalculateBounds()
    {
        Mesh m = objectToRender.GetComponent<MeshFilter>().mesh;
        Bounds = new()
        {
            x = m.bounds.size.x / PuzzleData.NCols,
            y = m.bounds.size.y / PuzzleData.NRows,
            z = m.bounds.size.z / PuzzleData.NDepth
        };
    }
    protected Vector3 CalculateOffsetVec(int i, int j, int k)
    {
        return new Vector3(CalculateOffsetForSocketCenter(PuzzleData.NCols, i) * transform.localScale.x, CalculateOffsetForSocketCenter(PuzzleData.NRows, j) * transform.localScale.y, CalculateOffsetForSocketCenter(PuzzleData.NDepth, k) * transform.localScale.z);
    }
    Vector3 CalculateDirectionToCenter(int i, int j, int k)
    {
        float x = Mathf.Sign(i - (PuzzleData.NCols - 1) / 2f);
        float y = Mathf.Sign(j - (PuzzleData.NRows - 1) / 2f);
        float z = Mathf.Sign(k - (PuzzleData.NDepth - 1) / 2f);
        return new Vector3(x, y, z);
    }
    float GetMeshBottomY(GameObject obj)
    {
        Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;
        Vector3 center = mesh.bounds.center;
        Vector3 extents = mesh.bounds.extents;
        float localBottomY = center.y - extents.y;
        float worldBottomY = localBottomY * obj.transform.localScale.y;
        return worldBottomY;
    }


}
