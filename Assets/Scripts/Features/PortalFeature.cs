using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PortalFeature : BaseFeature
{
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
    [Header("Interaction Configuration")]
    [SerializeField]
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketInteractor;
    void Start()
    {
        InitRotations();
    }


    void Update()
    {
        UpdatePortalRotationZ(rotSpeed);
    }
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
}
