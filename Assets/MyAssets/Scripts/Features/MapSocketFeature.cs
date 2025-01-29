using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MapSocketFeature : MonoBehaviour
{
    [Header("Sprite numbers")]
    [SerializeField]
    public int nRows = 1;
    [SerializeField]
    public int nCols = 1;
    [SerializeField]
    [Header("Materials")]
    public Material validMaterial;
    [SerializeField]
    public Material notValidMaterial;
    private Vector3 pieceScale;
    private Sprite[] spriteRendererArr;
    private float boundsX;
    private float boundsY;
    private float boundsZ;
    // Start is called before the first frame update
    public void Start()
    {
        SpawnMapSockets();
    }

    private void CalculateBounds()
    {
        boundsX = spriteRendererArr[0].bounds.size.x;
        boundsY = spriteRendererArr[0].bounds.size.y;
        boundsZ = spriteRendererArr[0].bounds.size.y;
    }

    public void SpawnMapSockets()
    {
        var mapPiecesScript = GameObject.Find("MapPieces").GetComponent<MapFeature>();
        spriteRendererArr = mapPiecesScript.GetSpritesArray();
        CalculateBounds();
        pieceScale = GetPieceScale(mapPiecesScript);
        SetMapHolderScale();
        //generate sockets
        for (int i = 0; i < nRows; i++)
        {
            for (int j = 0; j < nCols; j++)
            {
                int index = nRows * nCols - ((i * nCols) + j) - 1;
                GameObject socket = GenerateMapSocket(spriteRendererArr[index]);
                PlaceSocketAt(ref socket, i, j);
            }
        }

    }

    private void PlaceSocketAt(ref GameObject socket, int i, int j)
    {
        float offsetX = CalculateTraslation(nCols, j);
        float offsetY = CalculateTraslation(nRows, i);
        socket.transform.Translate(offsetX * transform.localScale.x, offsetY * transform.localScale.y, 0);
    }
    private float CalculateTraslation(int tot, int i)
    {
        float pos = i < tot / 2 ? -(tot / 2 - i) : (i - tot / 2);
        if (tot % 2 == 0)
            pos = i >= tot / 2 ? _ = pos * 2 + 1 : _ = (pos + 1) * 2 - 1;
        return tot % 2 != 0 ? pos / tot : pos / (tot * 2);
    }
    private Vector3 GetPieceScale(MapFeature script)
    {
        return script.GetPieceScale();
    }

    private GameObject GenerateMapSocket(Sprite mapPiece)
    {
        GameObject socket = new(mapPiece.name);
        BoxCollider box = socket.AddComponent(typeof(BoxCollider)) as BoxCollider;
        XRSocketInteractor xRSocketInteractor = socket.AddComponent(typeof(XRSocketInteractor)) as XRSocketInteractor;


        //setup boc collider
        box.size = new Vector3(boundsX, boundsY, boundsZ * 20);
        box.isTrigger = true;

        //setup attachPoint
        GameObject attachPoint = new("Attach Point");
        AddAttachPoint(socket, ref attachPoint);
        //socket interactor
        xRSocketInteractor.hoverSocketSnapping = true;
        xRSocketInteractor.interactableHoverMeshMaterial = validMaterial;
        xRSocketInteractor.interactableCantHoverMeshMaterial = notValidMaterial;
        xRSocketInteractor.interactionLayers = LayerMask.GetMask("Grabbable");
        xRSocketInteractor.attachTransform = attachPoint.transform;
        //change transform and attach to parent

        socket.transform.position += transform.position;
        socket.transform.localScale = pieceScale;
        socket.transform.parent = transform;
        return socket;
    }

    private void SetMapHolderScale()
    {
        transform.localScale = new Vector3(boundsX * nCols * pieceScale.x, boundsY * nRows * pieceScale.y, transform.localScale.z * pieceScale.z);
    }
    void AddAttachPoint(GameObject obj, ref GameObject attachPoint)
    {
        //attachPoint.transform.localPosition = new(0, 0, 0);
        float offsetz = -0.2f;
        float offset = -boundsY / (2 * obj.transform.localScale.y);
        attachPoint.transform.Translate(new(0, offset, offsetz));
        attachPoint.transform.parent = obj.transform;
    }
}
