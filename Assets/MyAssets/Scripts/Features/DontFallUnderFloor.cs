using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DontFallUnderFloor : MonoBehaviour
{
    void FixedUpdate()
    {
        if (transform.position.y < -1)
        {
            transform.position = new Vector3(0, 0.5f, 0);
        }
    }
}
