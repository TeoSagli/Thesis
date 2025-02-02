using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlashlightFeature : BaseFeature
{
    [Header("Flashlight configuration")]
    [SerializeField]
    private Transform flashlightPivot;

    [SerializeField]
    private bool on = false;
    [Header("Interaction configuration")]
    [SerializeField]
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private void Start()
    {
        grabInteractable?.activated.AddListener((s) =>
        {
            ToggleFlashLight();
        });
    }
    private void ToggleFlashLight()
    {
        on = !on;
        flashlightPivot.GetComponentInChildren<Light>().enabled = on;
        if (on)
            PlayOnStarted();
        else
            PlayOnEnded();
    }
}
