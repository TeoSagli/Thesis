using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PortalFeature : BaseFeature
{

    [Header("Portal Configuration")]
    [SerializeField]
    private GameObject portalMesh;
    [SerializeField]
    private GameObject portalDestination;
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
        ToggleParticle(particleSystemIn);
        socketInteractor?.selectEntered.AddListener((s) =>
        {
            UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable selectedInteractable = socketInteractor.firstInteractableSelected;
            GameObject objectPlaced = selectedInteractable.transform.gameObject;

            Debug.Log("" + objectPlaced.name);
            socketInteractor.interactionManager.SelectExit(socketInteractor, selectedInteractable);
            TeleportObjectTo(objectPlaced, portalDestination.transform.position, portalDestination.transform.rotation);
            SetVolume(0.05f);
            //PlayOnStarted();
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
        rotX = portalMesh.transform.localEulerAngles.x;
        rotY = portalMesh.transform.localEulerAngles.y;
        rotZ = portalMesh.transform.localEulerAngles.z;
    }
    private void UpdatePortalRotationZ(float speed)
    {
        rotZ += speed;
        portalMesh.transform.localEulerAngles = new Vector3(rotX, rotY, rotZ);
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
    private void TeleportObjectTo(GameObject objectToTeleport, Vector3 destination, Quaternion rotation)
    {
        Instantiate(objectToTeleport, destination, rotation);
        RemoveOldObj(objectToTeleport);
    }
    private void RemoveOldObj(GameObject obj)
    {
        Destroy(obj);
    }
    //==============================================================================

}
