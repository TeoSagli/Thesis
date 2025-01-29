using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MapFeature : MonoBehaviour
{
    [SerializeField]
    [Header("Image pieces")]
    private Sprite[] spriteRendererArr;

    [SerializeField]
    [Header("Sprite shader")]
    private Shader spriteShader;
    [SerializeField]
    [Header("Scale of the image pieces")]
    private Vector3 pieceScale = new(0.05f, 0.05f, 0.05f);
    //---------------------------------
    private Quaternion pieceRotation = Quaternion.identity;


    public void SpawnMapPieces()
    {
        GameObject[] objectsArr = new GameObject[spriteRendererArr.Length];
        int i = 0;
        foreach (Sprite mapPiece in spriteRendererArr)
        {
            GameObject piece = GenerateMapPiece(mapPiece);
            objectsArr[i] = piece;
            i++;
        }
        SpawnRndMapPieces(objectsArr);
    }

    private GameObject GenerateMapPiece(Sprite mapPiece)
    {
        GameObject piece = new(mapPiece.name);

        SpriteRenderer sr = piece.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        Rigidbody rb = piece.AddComponent(typeof(Rigidbody)) as Rigidbody;
        BoxCollider box = piece.AddComponent(typeof(BoxCollider)) as BoxCollider;
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
        xrGrabInteractable.interactionLayers = LayerMask.GetMask("Grabbable");
        xrGrabInteractable.farAttachMode = InteractableFarAttachMode.Near;
        xrGrabInteractable.attachTransform = attachPoint.transform;
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
    public Sprite[] GetSpritesArray()
    {
        return spriteRendererArr;
    }
    public Vector3 GetPieceScale()
    {
        return pieceScale;
    }
}
