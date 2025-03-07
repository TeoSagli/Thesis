using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveObject : MonoBehaviour
{
    public Button forward;
    public Button back;
    public Button left;
    public Button right;
    public float distance = 1;
    void Start()
    {
        forward.onClick.AddListener(() =>
            {
                MoveForward();
            });
        back.onClick.AddListener(() =>
            {
                MoveBackward();
            });
        left.onClick.AddListener(() =>
            {
                MoveLeft();
            });
        right.onClick.AddListener(() =>
            {
                MoveRight();
            });
    }
    private void MoveRight()
    {
        transform.parent.Translate(distance * Vector3.right);
    }
    private void MoveLeft()
    {
        transform.parent.Translate(distance * Vector3.left);
    }
    private void MoveForward()
    {
        transform.parent.Translate(distance * Vector3.forward);
    }
    private void MoveBackward()
    {
        transform.parent.Translate(distance * Vector3.back);
    }
}
