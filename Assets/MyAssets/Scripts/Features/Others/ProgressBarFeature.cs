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
    private TextMeshProUGUI percentage;
    [SerializeField]
    [Header("Green bar object")]
    private GameObject bar;
    void Start()
    {
        UpdateBar(0);
    }

    public void UpdateBar(float percentage)
    {
        SetPercentage(percentage);
        AdjustBar(percentage);
    }
    private void SetPercentage(float text)
    {
        percentage.text = text + "%";
    }
    private void AdjustBar(float text)
    {
        var s = bar.transform.localScale;
        bar.transform.localScale = new Vector3(text / 100, s.y, s.z);
    }

}
