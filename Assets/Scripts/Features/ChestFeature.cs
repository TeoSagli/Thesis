using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ChestFeature : BaseFeature
{
    [Header("Chest Configuration")]
    [SerializeField]
    private Transform chestPivot;

    [SerializeField]
    private float maxAngle = 90.0f;

    [SerializeField]
    private bool reverseAngleDirection = false;

    [SerializeField]
    private float speed = 25f;

    [SerializeField]
    private bool open = false;

    [SerializeField]
    private bool makeItKinematicOnceOpened = false;
    [SerializeField]
    private UnityEvent onOpenChest;
    [Header("Interaction Configuration")]
    [SerializeField]
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketInteractor;

    [SerializeField]
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable simpleInteractable;
    private IEnumerator currCoroutine;
    private void Start()
    {
        OpenChest();
        // Chest with sockets
        socketInteractor?.selectEntered.AddListener((s) =>
        {
            OpenChest();
            PlayOnStarted();
        });
        socketInteractor?.selectExited.AddListener((s) =>
        {
            PlayOnEnded();
            socketInteractor.socketActive = featureUsage == FeatureUsage.Once ? true : false;
        });

        // Chest with simple interactables for instance a locker
        simpleInteractable?.selectEntered.AddListener((s) =>
        {
            OpenChest();
            PlayOnStarted();
        });
        simpleInteractable?.selectExited.AddListener((s) =>
        {
            PlayOnEnded();
        });

    }

    public void OpenChest()
    {
        if (!open)
        {
            PlayOnStarted();
            open = true;
            currCoroutine = ProcessMotion();
            StartCoroutine(currCoroutine);
            StopInteractions();
            onOpenChest?.Invoke();
        }
    }
    private IEnumerator ProcessMotion()
    {
        while (open)
        {
            var angle = chestPivot.localEulerAngles.x < 180 ? chestPivot.localEulerAngles.x : chestPivot.localEulerAngles.x - 360;

            angle = reverseAngleDirection ? Mathf.Abs(angle) : angle;

            if (angle <= maxAngle)
            {
                chestPivot.Rotate(Vector3.right, speed * Time.deltaTime * (reverseAngleDirection ? -1 : 1));
            }
            else
            {
                open = false;
                var featureRigidBody = GetComponent<Rigidbody>();
                if (featureRigidBody != null && makeItKinematicOnceOpened)
                    featureRigidBody.isKinematic = true;

            }
            yield return null;
        }


    }
    private void StopInteractions()
    {
        //disable interactable when opened
        if (simpleInteractable != null)
            simpleInteractable.enabled = false;
    }

}
