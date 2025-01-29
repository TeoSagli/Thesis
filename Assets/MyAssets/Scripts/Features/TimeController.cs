using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public GameObject hourObject;
    public GameObject minObject;
    public GameObject secObject;
    private float maxAngle = 360f;
    private float maxSecs = 60f;
    private float maxMins = 60f;
    private float maxHours = 12f;
    static float hAngle;
    static float mAngle;
    static float sAngle;

    private float currHourAngle;
    private float currMinAngle;
    private float currSecAngle;
    private DateTime oldTime;
    private DateTime newTime;
    private DateTime timeDiff;
    private Vector3 startRot = new Vector3(90, 0, -90);
    // Start is called before the first frame update
    void Start()
    {
        hAngle = maxAngle / maxHours;
        mAngle = maxAngle / maxMins;
        sAngle = maxAngle / maxSecs;
        DateTime startTime = DateTime.Now;
        UpdateClock(startTime, startTime);
    }

    // Update is called once per frame
    void Update()
    {
        newTime = DateTime.Now;
        timeDiff = CalculateDiffTime(newTime, oldTime);
        if (timeDiff.Second > 0 || timeDiff.Minute > 0 || timeDiff.Hour > 0)
            UpdateClock(newTime, timeDiff);
    }
    void UpdateClock(DateTime time, DateTime timeDiff)
    {
        /*  Debug.Log(timeDiff.Hour + " " + timeDiff.Minute + " " + timeDiff.Second);*/
        oldTime = time;
        CalcCurrHourMinSec(timeDiff);
        RotateClock(currHourAngle, currMinAngle, currSecAngle);

    }
    void CalcCurrHourMinSec(DateTime time)
    {
        int currHour = time.Hour;
        if (currHour > 11)
            currHour -= 12;
        currHourAngle = currHour * hAngle;
        currMinAngle = time.Minute * mAngle;
        currSecAngle = time.Second * sAngle;
    }
    void RotateClock(float h, float m, float s)
    {
        hourObject.transform.Rotate(0, h, 0);
        minObject.transform.Rotate(0, m, 0);
        secObject.transform.Rotate(0, s, 0);
    }
    DateTime CalculateDiffTime(DateTime time1, DateTime time2)
    {
        int diffHour = newTime.Hour - oldTime.Hour;
        int diffMin = newTime.Minute - oldTime.Minute;
        int diffSec = newTime.Second - oldTime.Second;
        if (diffHour < 0) diffHour = 0;
        if (diffMin < 0) diffMin = 0;
        if (diffSec < 0) diffSec = 0;
        return new DateTime(newTime.Year, newTime.Month, newTime.Day, diffHour, diffMin, diffSec);
    }
}
