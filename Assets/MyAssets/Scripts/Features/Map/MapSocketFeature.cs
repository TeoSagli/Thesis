using System;
using System.Collections;
using System.Collections.Generic;
using DilmerGames.Core.Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MapSocketFeature : BaseFeature
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
    [SerializeField]
    [Header("Title")]
    public TextMeshProUGUI title;
    public String titleToSet;

    [SerializeField]
    [Header("Socket Background")]
    private GameObject socketBack;
    private Vector3 pieceScale;
    private Sprite[] spriteRendererArr;
    private bool[] isPieceCorrect;
    private float boundsX;
    private float boundsY;
    private float boundsZ;


    // Start is called before the first frame update
    private void Start()
    {
        SpawnMapSockets();
    }
    //================TITLE===================
    private void SetTitle(String titleToSet)
    {
        title.text = titleToSet;
    }
    private void PositionTitle(Vector3 vector)
    {
        title.transform.Translate(vector);
    }
    //================BOUNDS===================
    private void CalculateBounds()
    {
        boundsX = spriteRendererArr[0].bounds.size.x;
        boundsY = spriteRendererArr[0].bounds.size.y;
        boundsZ = spriteRendererArr[0].bounds.size.z;
    }
    //================MAP HOLDER===================
    private void SetMapHolderScale()
    {
        transform.localScale = new Vector3(boundsX * nCols * pieceScale.x, boundsY * nRows * pieceScale.y, transform.localScale.z * pieceScale.z);
    }
    //================SOCKETS===================
    public void SpawnMapSockets()
    {
        isPieceCorrect = new bool[nRows * nCols];
        var mapPiecesScript = GameObject.Find("SpawnMapPieces").GetComponent<MapFeature>();
        spriteRendererArr = mapPiecesScript.GetSpritesArray();
        //configure bounds
        CalculateBounds();
        //configure pieces' scale
        pieceScale = GetPieceScale(mapPiecesScript);
        //configure title
        SetTitle(titleToSet);
        PositionTitle(new Vector3(0, -boundsY * nRows * pieceScale.y, 0));
        //configure map holder
        SetMapHolderScale();
        //configure map sockets
        for (int i = 0; i < nRows; i++)
        {
            for (int j = 0; j < nCols; j++)
            {
                int index = nRows * nCols - ((i * nCols) + j) - 1;
                GameObject socket = GenerateMapSocket(spriteRendererArr[index], index);
                PlaceSocketAt(ref socket, i, nCols - j - 1);
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
    private GameObject GenerateMapSocket(Sprite mapPiece, int i)
    {
        GameObject socket = new(mapPiece.name);
        BoxCollider box = socket.AddComponent(typeof(BoxCollider)) as BoxCollider;
        XRSocketInteractor xRSocketInteractor = socket.AddComponent(typeof(XRSocketInteractor)) as XRSocketInteractor;

        //setup box collider
        Vector3 o = new Vector3(4, 4, 1);
        box.size = new Vector3(boundsX / o.x, boundsY / o.y, boundsZ / o.z);
        box.isTrigger = true;

        //setup attachPoint
        GameObject attachPoint = new("Attach Point");
        AddAttachPoint(socket, ref attachPoint);
        //socket interactor
        xRSocketInteractor.hoverSocketSnapping = true;
        xRSocketInteractor.interactableHoverMeshMaterial = validMaterial;
        xRSocketInteractor.interactableCantHoverMeshMaterial = notValidMaterial;
        xRSocketInteractor.interactionLayers = LayerMask.GetMask("Map");
        xRSocketInteractor.attachTransform = attachPoint.transform;
        xRSocketInteractor.selectEntered.AddListener((s) =>
        {
            CheckPieceCorrect(xRSocketInteractor, i);
        });
        //socket back
        var back = Instantiate(socketBack);
        back.transform.position = socket.transform.position;
        back.transform.localScale = new Vector3(boundsX, boundsY, boundsZ);
        back.transform.parent = socket.transform;
        //change transform and attach to parent
        socket.transform.position += transform.position;
        socket.transform.localScale = pieceScale;
        socket.transform.parent = transform;
        //create child game object to show
        return socket;
    }

    void AddAttachPoint(GameObject obj, ref GameObject attachPoint)
    {
        //attachPoint.transform.localPosition = new(0, 0, 0);
        float offsetz = -0.2f;
        float offset = -boundsY / (2 * obj.transform.localScale.y);
        attachPoint.transform.Translate(new(0, offset, offsetz));
        attachPoint.transform.parent = obj.transform;
    }
    private Vector3 GetPieceScale(MapFeature script)
    {
        return script.GetPieceScale();
    }
    //================CHECK AND HANDLE WIN===================
    private void CheckPieceCorrect(XRSocketInteractor socket, int index)
    {
        isPieceCorrect[index] = socket.isSelectActive && socket.name == socket.interactablesSelected[0].transform.name;
        CheckWin();
    }
    private void CheckWin()
    {
        bool res = TestWin();
        if (res)
            OnWin();

        //INTERCTABLESECELT SELECT MODER socket.interactablesSelected[0].SELECTOMODE
    }
    private bool TestWin()
    {
        for (int i = 0; i < nRows; i++)
            for (int j = 0; j < nCols; j++)
                if (!isPieceCorrect[i * nCols + j])
                    return false;
        return true;
    }
    private void OnWin()
    {
        PuzzleManager.Instance.PuzzleAdvancement();
        PlayOnStarted();
    }
}
