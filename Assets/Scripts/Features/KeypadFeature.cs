using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class KeypadFeature : BaseFeature
{
    [Header("Keypad Configuration")]
    [SerializeField]
    private Button firstCodeButton;
    [SerializeField]
    private Button secondCodeButton;
    [SerializeField]
    private Button thirdCodeButton;
    [SerializeField]
    private GameObject keypadUI;
    [SerializeField]
    private UnityEvent onKeycodesCorrect;
    [SerializeField]
    private int keycodeAnswer = 250;

    public int num1 = 0, num2 = 0, num3 = 0;
    /*
        [Header("Interaction Configuration")]
        [SerializeField]
        private XRSocketInteractor socketInteractor;

        [SerializeField]
        private XRSimpleInteractable simpleInteractable;
    */
    protected override void Awake()
    {
        base.Awake();
        firstCodeButton.onClick.AddListener(() =>
        {
            EnterNumAndCheck(ref num1, ref firstCodeButton);
        });
        secondCodeButton.onClick.AddListener(() =>
        {
            EnterNumAndCheck(ref num2, ref secondCodeButton);
        });
        thirdCodeButton.onClick.AddListener(() =>
        {
            EnterNumAndCheck(ref num3, ref thirdCodeButton);
        });
    }
    private void CheckCodeCombination()
    {
        if (int.TryParse($"{num1}{num2}{num3}", out int keyCodeEntered))
        {
            if (keycodeAnswer == keyCodeEntered)
            {
                onKeycodesCorrect?.Invoke();
                PlayOnStarted();
                OnWin();

            }
            else
            {
                PlayOnEnded();
            }
        }
    }
    public void ToggleKeypad()
    {
        bool active = !keypadUI.activeSelf;
        keypadUI.SetActive(active);
    }
    private void EnterNumAndCheck(ref int num, ref Button button)
    {
        num++;
        if (num > 9)
            num = 0;
        button.GetComponentInChildren<TextMeshProUGUI>().text = $"{num}";
        CheckCodeCombination();
    }
    public void OnWin()
    {
        ToggleKeypad();
        gameObject.GetComponent<XRSimpleInteractable>().enabled = false;
    }
}
