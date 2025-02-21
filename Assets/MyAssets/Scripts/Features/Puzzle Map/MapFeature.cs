using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Meta.XR.MRUtilityKit;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MapFeature : Spawn
{
    [SerializeField, Header("MRUK object")]
    private MRUK mruk;
    [SerializeField, Header("Sprite params")]
    private Shader spriteShader;
    [SerializeField]
    private Sprite spriteToRender;
    [SerializeField]
    private int nCols;
    [SerializeField]
    private int nRows;
    [SerializeField, Header("Scale of the image pieces")]
    private Vector3 pieceScale = new(0.05f, 0.05f, 0.05f);
    [SerializeField, Header("Title")]
    private string title;
    private void Start()
    {
        if (MRUK.Instance)
            MRUK.Instance.RegisterSceneLoadedCallback(() =>
            {
                SpawnMapPieces();
            });
    }
    private Sprite SpriteExtractor(Sprite sprite, int i, int j, int pos)
    {
        float w = sprite.bounds.size.x / nCols * 100; //tot 1000
        float h = sprite.bounds.size.y / nRows * 100; //tot 1330
        float x = j * w;
        float y = i * h;

        // Define the portion of the sprite to extract (x, y, width, height)
        Rect rect = new(x, y, w, h);
        // Create the new sprite
        Vector2 pivotDef = new(0.5f, 0.5f);
        Sprite s = Sprite.Create(sprite.texture, rect, pivotDef);
        s.name = sprite.name + "-tile" + pos;
        return s;
    }
    private void SpawnMapPieces()
    {
        int contTiles = 0;
        int totPieces = nCols * nRows;
        GameObject[] objectsArr = new GameObject[totPieces];
        for (int j = 0; j < nCols; j++)
        {
            for (int i = 0; i < nRows; i++)
            {
                Sprite mapPiece = SpriteExtractor(spriteToRender, nRows - 1 - i, j, contTiles);
                GameObject piece = GenerateMapPiece(mapPiece);
                objectsArr[contTiles] = piece;
                contTiles++;
            }

        }
        SpawnRndMapPieces(objectsArr);
    }
    private GameObject GenerateMapPiece(Sprite mapPiece)
    {
        GameObject piece = new(mapPiece.name);

        SpriteRenderer sr = piece.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        Rigidbody rb = piece.AddComponent(typeof(Rigidbody)) as Rigidbody;
        BoxCollider box = piece.AddComponent(typeof(BoxCollider)) as BoxCollider;
        DontFallUnderFloor script = piece.AddComponent(typeof(DontFallUnderFloor)) as DontFallUnderFloor;
        XRGrabInteractable xrGrabInteractable = piece.AddComponent(typeof(XRGrabInteractable)) as XRGrabInteractable;
        //setup boc collider
        box.size = new Vector3(mapPiece.bounds.size.x, mapPiece.bounds.size.y, mapPiece.bounds.size.z);
        //setup renderer
        sr.sprite = mapPiece;
        sr.material.shader = spriteShader;
        //setup rigidBody
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.None;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        //setup attachPoint
        GameObject attachPoint = new("Attach Point");
        AddAttachPoint(piece, ref attachPoint, mapPiece);
        //grab interactable
        xrGrabInteractable.interactionLayers = LayerMask.GetMask("Map");
        xrGrabInteractable.farAttachMode = InteractableFarAttachMode.Near;
        xrGrabInteractable.attachTransform = attachPoint.transform;
        xrGrabInteractable.selectMode = InteractableSelectMode.Single;
        //change transform and attach to parent
        piece.transform.localScale = pieceScale;
        piece.transform.parent = transform;
        return piece;
    }
    void SpawnRndMapPieces(GameObject[] objectsArr)
    {
        SpawnMapPieces script = GameObject.Find("SpawnMapPieces").GetComponent<SpawnMapPieces>();
        script.SetObjectsArr(objectsArr);
        script.StartSpawn();
    }
    void AddAttachPoint(GameObject obj, ref GameObject attachPoint, Sprite mapPiece)
    {
        //attachPoint.transform.localPosition = new(0, 0, 0);
        float offset = -mapPiece.bounds.size.y / (2 * obj.transform.localScale.y);
        attachPoint.transform.Translate(new(0, offset, 0));
        attachPoint.transform.parent = obj.transform;
    }
    public Sprite GetOriginalSprite()
    {
        return spriteToRender;
    }
    public Vector3 GetPieceScale()
    {
        return pieceScale;
    }
    public int GetNRows()
    {
        return nRows;
    }
    public int GetNCols()
    {
        return nCols;
    }
    public string GetTitle()
    {
        return title;
    }
}
