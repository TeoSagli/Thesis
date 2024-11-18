using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PortalFeature : BaseFeature
{
    private const float teleportDistance = 0.5f;

    [Header("Portal Configuration")]
    [SerializeField]
    private GameObject portalModel;
    [SerializeField]
    private float rotSpeed = 0.05f;
    [SerializeField]
    private float rotX;
    [SerializeField]
    private float rotY;
    [SerializeField]
    private float rotZ;
    [SerializeField]
    public ParticleSystem particleSystemIn;
    [SerializeField]
    public ParticleSystem particleSystemOut;


    [Header("Interaction Configuration")]
    [SerializeField]
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketInteractor;

    //==============================================================================
    void Start()
    {
        InitRotations();
        InitParticles();

        socketInteractor?.selectEntered.AddListener((s) =>
        {
            UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable selectedInteractable = socketInteractor.firstInteractableSelected;
            GameObject objectPlaced = selectedInteractable.transform.gameObject;

            socketInteractor.interactionManager.SelectExit(socketInteractor, selectedInteractable);
            TeleportObject(objectPlaced);
            SetVolume(0.05f);
            //  PlayOnStarted();
        });
    }


    void Update()
    {
        UpdatePortalRotationZ(rotSpeed);
    }

    //==============================================================================
    //  ROTATIONS
    private void InitRotations()
    {
        rotX = portalModel.transform.localEulerAngles.x;
        rotY = portalModel.transform.localEulerAngles.y;
        rotZ = portalModel.transform.localEulerAngles.z;
    }
    private void UpdatePortalRotationZ(float speed)
    {
        rotZ += speed;
        portalModel.transform.localEulerAngles = new UnityEngine.Vector3(rotX, rotY, rotZ);
    }
    //==============================================================================
    //  PARTICLES
    private void InitParticles()
    {
        particleSystemIn.gameObject.SetActive(false);
        particleSystemOut.gameObject.SetActive(false);
    }
    public void ToggleParticle(ParticleSystem p)
    {
        p.gameObject.SetActive(!p.gameObject.activeSelf);
    }
    public ref ParticleSystem GetParticleSystemIn()
    {
        return ref particleSystemIn;
    }

    //==============================================================================
    //  TELEPORT
    private void TeleportObject(GameObject objectToTeleport)
    {
        objectToTeleport.transform.Translate(-teleportDistance, 0, 0);
    }
    private void RemoveOldObj(ref GameObject obj)
    {
        Destroy(obj);
    }
    //==============================================================================

}
