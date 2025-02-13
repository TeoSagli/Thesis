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
    private GameObject bar;
    private float percentage = 0;
    void Start()
    {
        UpdateBar();
    }

    public void UpdateBar()
    {
        UpdateTextPercentage();
        AdjustBar();
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
        var s = bar.transform.localScale;
        bar.transform.localScale = new Vector3(percentage / 100, s.y, s.z);
    }

}
