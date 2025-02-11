using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ShowIfAttribute : PropertyAttribute
{
    public string conditionField;
    public object conditionValue;

    public ShowIfAttribute(string conditionField, object conditionValue)
    {
        this.conditionField = conditionField;
        this.conditionValue = conditionValue;
    }
}
