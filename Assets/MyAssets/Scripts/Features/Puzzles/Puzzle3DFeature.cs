using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Puzzle3DFeature : Puzzle
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
                    puzzlePiecesArr[contTiles] = GeneratePuzzlePiece(tileMesh, objectToRender.name + $"-tile{contTiles}");
                }
            }
        }
    }
    private Mesh ExtractCubeMesh(Vector3[] originalVertices, int[] originalTriangles, Vector3 cubeMin, Vector3 cubeMax)
    {
        List<Vector3> newVertices = new();
        List<int> newTriangles = new();
        Dictionary<int, int> vertexMap = new();

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
        };
        newMesh.RecalculateNormals();
        return newMesh;
    }
    // PUZZLE PIECES' GENERATION PROCESS
    private GameObject GeneratePuzzlePiece(Mesh mesh, string name)
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
        AddAttachPoint(piece, ref attachPoint, mesh);
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
    void AddAttachPoint(GameObject obj, ref GameObject attachPoint, Mesh m)
    {
        Vector3 extendsObj = m.bounds.extents;
        Vector3 centerObj = m.bounds.center;
        Vector3 offset = new()
        {
            x = centerObj.x,
            y = centerObj.y - extendsObj.y,
            z = centerObj.z,
        };
        attachPoint.transform.position = offset;
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
