using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestRotationRange : MonoBehaviour
{
    public float minRotationX;
    public float maxRotationX;

    private Vector3 initialPosition;

    private void Awake()
    {
        initialPosition = transform.up;
    }

    private void LateUpdate()
    {
        int sign = Vector3.SignedAngle(transform.up, initialPosition, Vector3.forward) > 0 ? 1 : -1;
        float x = sign == 1 ? Mathf.Clamp(WrapAngle(transform.localEulerAngles.x), minRotationX, maxRotationX) : minRotationX; 

        transform.localRotation = Quaternion.Euler(x, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    private float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180) return angle - 360;

        return angle;
    }
}
