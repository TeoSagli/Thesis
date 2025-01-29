using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;


public class DrawerFeature : BaseFeature
{
    [Header("Drawer Configuration")]
    [SerializeField]
    private Transform drawerPivot;
    private float startPos;

    private readonly float startOffset = 0.008f;

    [SerializeField]
    private float maxDistance = 0.25f;

    [SerializeField]
    private FeatureDirection featureDirection = FeatureDirection.Forward;

    [SerializeField]
    private float speed = 2.5f;

    [SerializeField]
    private bool open = false;

    private IEnumerator currCoroutine;
    [Header("Interaction Configuration")]

    [SerializeField]
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable simpleInteractable;

    private void Start()
    {
        startPos = drawerPivot.localPosition.z + startOffset;
        // drawers with simple interactables selection
        simpleInteractable?.selectEntered.AddListener((s) =>
        {
            featureDirection = open == false ? FeatureDirection.Forward : FeatureDirection.Backward;
            ToggleDrawer();
        });
        simpleInteractable?.selectExited.AddListener((s) =>
        {
            PlayOnEnded();
        });
    }

    public void ToggleDrawer()
    {
        PlayOnStarted();
        currCoroutine = ProcessMotion();
        StartCoroutine(currCoroutine);

    }
    private IEnumerator ProcessMotion()
    {
        while (true)
        {
            if (featureDirection == FeatureDirection.Forward && drawerPivot.localPosition.z <= maxDistance)
            {
                drawerPivot.Translate(Vector3.forward * Time.deltaTime * speed);
            }
            else if (featureDirection == FeatureDirection.Backward && drawerPivot.localPosition.z >= startPos)
            {
                drawerPivot.Translate(Vector3.back * Time.deltaTime * speed);
            }
            else
            {
                open = !open;
                StopCoroutine(currCoroutine);
            }
            yield return null;
        }
    }
}
