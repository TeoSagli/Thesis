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
    [SerializeField]
    public ParticleSystem particleSystemIn;
    [SerializeField]
    public ParticleSystem particleSystemOut;
    [Header("Interaction Configuration")]
    [SerializeField]
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketInteractor;
    void Start()
    {
        InitRotations();
        InitParticles();
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
    private void InitParticles()
    {
        particleSystemIn.gameObject.SetActive(false);
        particleSystemOut.gameObject.SetActive(false);
    }
    public void ToggleParticle(ParticleSystem p)
    {
        p.gameObject.SetActive(!p.gameObject.activeSelf);
    }
    private void UpdatePortalRotationZ(float speed)
    {
        rotZ += speed;
        portalModel.transform.localEulerAngles = new UnityEngine.Vector3(rotX, rotY, rotZ);
    }
    public ref ParticleSystem GetParticleSystemIn()
    {
        return ref particleSystemIn;
    }
}
