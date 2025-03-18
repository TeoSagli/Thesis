using System;
using System.Collections;
using System.Collections.Generic;
using DilmerGames.Core.Singletons;
using Unity.XR.CoreUtils;
using UnityEngine;

public class EditModeManager : Singleton<EditModeManager>
{
    public GameObject editUI;
    private bool isEditable = false;
    public void ToggleEditableState()
    {
        isEditable = !isEditable;
        if (isEditable)
            LookForEditables();
        else
            DisableAllEditables();
    }

    private void DisableAllEditables()
    {
        GameObject[] editableArr = GameObject.FindGameObjectsWithTag("Editable");
        foreach (GameObject editable in editableArr)
        {
            GameObject ui = editable.GetNamedChild("EditUI");
            ui.SetActive(false);
        }
    }

    private void LookForEditables()
    {
        GameObject[] editableArr = GameObject.FindGameObjectsWithTag("Editable");
        foreach (GameObject editable in editableArr)
        {
            GameObject uiObj = editable.GetNamedChild("EditUI");
            if (uiObj != null)
            {
                uiObj.SetActive(true);
            }
            else
            {
                var ui = Instantiate(editUI, editable.transform);
                ui.name = editUI.name;
                if (editable.TryGetComponent<BoxCollider>(out var collider))
                {
                    var offset = collider.bounds.size.y * Vector3.one;
                    ui.transform.Translate(offset);
                }
                else
                {
                    ui.transform.Translate(0, 0.75f, 0);
                }
            }
        }
    }
}
