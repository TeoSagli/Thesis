using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [field: SerializeField]
    public Button resumeButton { get; set; }

    [field: SerializeField]
    public Button restartButton { get; set; }

    [field: SerializeField]
    public TextMeshProUGUI solvedText { get; set; }

}
