using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetNumber : MonoBehaviour
{
    [SerializeField]
    private int num;
    [SerializeField]
    private TextMeshProUGUI textToChange;
    void Start()
    {
        textToChange.text = "" + num;
    }


}
