using System;
using System.Collections;
using System.Collections.Generic;
using DilmerGames.Core.Singletons;
using Meta.XR.MRUtilityKit;
using TMPro;
using UnityEngine;

public class ProgressBarFeature : Singleton<ProgressBarFeature>
{
    [SerializeField]
    [Header("Percentage title")]

    private TextMeshProUGUI percentageText;
    [SerializeField]
    [Header("Green bar object")]
    private GameObject greenBar;
    private float percentage = 0;

    public void UpdateBar()
    {
        /*   if (transform.gameObject.activeSelf)
           {*/
        UpdateTextPercentage();
        AdjustBar();
        /* }*/
    }

    public void SetPercentage(float p)
    {
        percentage = p;
        if (transform.gameObject.activeSelf)
            UpdateBar();
    }

    private void UpdateTextPercentage()
    {
        percentageText.text = percentage + "%";
    }
    private void AdjustBar()
    {
        var s = greenBar.transform.localScale;
        greenBar.transform.localScale = new Vector3(percentage / 100, s.y, s.z);
    }

}
