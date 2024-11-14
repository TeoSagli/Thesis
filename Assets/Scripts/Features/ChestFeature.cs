using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


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

    [Header("Lock Configuration")]
    [SerializeField]
    private AudioClip AudioClipForLock;
    private const float rotAngle = 36;// 360 / 10
    [SerializeField]
    private int keycodeAnswer = 2024;
    [SerializeField]
    private UnityEvent onKeycodesCorrect;
    private int num1 = 0, num2 = 0, num3 = 0, num4 = 0;

    [Header("Interaction Configuration")]
    [SerializeField]
    private GameObject firstCode;
    [SerializeField]
    private GameObject secondCode;
    [SerializeField]
    private GameObject thirdCode;
    [SerializeField]
    private GameObject firthCode;
    private IEnumerator currCoroutine;
    private XRSimpleInteractable simple1, simple2, simple3, simple4;
    protected override void Awake()
    {
        base.Awake();
        InitLockInteractables();
        simple1.selectEntered.AddListener((s) =>
        {
            EnterNumAndCheck(ref num1, ref firstCode);
        });
        simple2.selectEntered.AddListener((s) =>
        {
            EnterNumAndCheck(ref num2, ref secondCode);
        });
        simple3.selectEntered.AddListener((s) =>
        {
            EnterNumAndCheck(ref num3, ref thirdCode);
        });
        simple4.selectEntered.AddListener((s) =>
        {
            EnterNumAndCheck(ref num4, ref firthCode);
        });
    }
    private void InitLockInteractables()
    {
        simple1 = firstCode.GetComponent<XRSimpleInteractable>();
        simple2 = secondCode.GetComponent<XRSimpleInteractable>();
        simple3 = thirdCode.GetComponent<XRSimpleInteractable>();
        simple4 = firthCode.GetComponent<XRSimpleInteractable>();
    }
    private void EnterNumAndCheck(ref int num, ref GameObject numCode)
    {
        num++;
        if (num > 9)
            num = 0;
        PlayAudioLock();
        RotateLock(1, numCode);
        CheckCodeCombination();
    }
    private void CheckCodeCombination()
    {
        if (int.TryParse($"{num1}{num2}{num3}{num4}", out int keyCodeEntered))
        {
            if (keycodeAnswer == keyCodeEntered)
            {
                onKeycodesCorrect?.Invoke();
                PlayOnStarted();
                OpenChest();
                StopInteractions();
            }
            else
            {
                PlayOnEnded();
            }
        }
    }
    private void RotateLock(int numRotations, GameObject numCode)
    {
        numCode.transform.RotateAround(numCode.transform.position, numCode.transform.right, rotAngle * numRotations);
    }
    public void OpenChest()
    {
        if (!open)
        {
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
        //disable interactables when opened
        simple1.enabled = false;
        simple2.enabled = false;
        simple3.enabled = false;
        simple4.enabled = false;
    }

    private void PlayAudioLock()
    {
        PlayAudioClip(AudioClipForLock);
    }
}
