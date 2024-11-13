using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadlockCombination : MonoBehaviour
{
    public List<char> charsOrder;
    public float charsAngleOffset;
    public PadlockManager padlockManager;
    private Vector3 initialPosition;
    private char charSelected;
    /*   private BoundsControl boundsControl;
       private Interactable padlockInteractable;*/

    private void Awake()
    {
        /*    padlockInteractable = GetComponent<Interactable>();
            boundsControl = GetComponent<BoundsControl>();*/
        charSelected = charsOrder[0];
        initialPosition = transform.forward;
    }

    public void OnClick()
    {
        transform.Rotate(Vector3.right, charsAngleOffset);
        CalculateChar();
    }

    public char GetCharSelected()
    {
        return charSelected;
    }

    public void StopManipulation()
    {
        /*    boundsControl.enabled = false;
            padlockInteractable.enabled = false;*/
    }

    public void CalculateChar()
    {
        int charSelectedInt = (int)Mathf.Repeat(Mathf.RoundToInt((Vector3.SignedAngle(transform.forward, initialPosition, Vector3.up) / charsAngleOffset)), charsOrder.Count);
        charSelected = charsOrder[charSelectedInt];
        padlockManager.UpdateCombination();
    }
}
