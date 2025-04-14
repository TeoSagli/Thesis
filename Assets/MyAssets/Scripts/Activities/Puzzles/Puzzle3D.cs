using System.Collections.Generic;
using System.IO;
using Meta.XR.MRUtilityKit;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Puzzle3D : PuzzlePiece
{
    [SerializeField, Header("Object params")]
    private GameObject objectToRender;
    [SerializeField]
    private GameObject[] objectMultipleMesh;
    private GameObject objectMesh;
    private PuzzleData3D puzzleData3D;

    public PuzzleData3D PuzzleData3D { get => puzzleData3D; set => puzzleData3D = value; }

    public Puzzle3D(float pieceScale, string titleStr, int nCols, int nRows, int nDepth, string path, string name) : base(pieceScale, titleStr, nCols, nRows, nDepth)
    {
        PuzzleData = new(pieceScale, titleStr, nCols, nRows);
        PuzzleData3D = new(path, name);
        Init();
    }

    private void Init()
    {
       ExtractGridMesh();
       SpawnRndPuzzlePieces();
          
    }
    //================EXTRACT PIECES===================
    public void ExtractGridMesh()
    {
        Mesh originalMesh;
        objectMesh = GenerateCombinedMeshObject();
        CalculateBounds();
        puzzlePiecesArr = new GameObject[PuzzleData.NCols * PuzzleData.NRows * PuzzleData.NDepth];

        originalMesh = objectMesh.GetComponent<MeshFilter>().mesh;
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
                    puzzlePiecesArr[contTiles] = GeneratePuzzlePiece(tileMesh, objectToRender.name + $"-tile{contTiles}", CalculateOffsetVec(x, y, z), y, x);
                }
            }
        }
    }
    private GameObject GenerateCombinedMeshObject()
    {
        Mesh originalMesh = new();
        GameObject combinedMeshObject = new(objectToRender.name);
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
        float w = Bounds.x * 100; //tot 1000
        float h = Bounds.y * 100; //tot 1330
        Vector2 offset = new()
        {
            x = j * w,
            y = i * h,
        };
        //  meshMat.SetTextureScale("_DetailAlbedoMap", Vector2.one / (puzzleData.NCols * puzzleData.NRows * puzzleData.NDepth));
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
        piece.transform.localScale = PuzzleData.PieceScale * Vector3.one;
        piece.transform.parent = transform;
        return piece;
    }
    protected override void CalculateBounds()
    {
        Mesh m = objectMesh.GetComponent<MeshFilter>().mesh;
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
    void AddAttachPoint(GameObject obj, ref GameObject attachPoint, Mesh puzzleMesh, Vector3 toTrans)
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

        attachPoint.transform.position = offset;
        attachPoint.transform.position += centerObj;
        attachPoint.transform.parent = obj.transform;
    }
    //================GETTERS===================
    public GameObject GetOriginalObject()
    {
        return objectToRender;
    }
    public int GetNDepth()
    {
        return PuzzleData.NDepth;
    }
    public GameObject GetOriginalMeshObject()
    {
        return objectMesh;
    }

}
