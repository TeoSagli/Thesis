using System;
using System.Collections;
using System.Collections.Generic;
using DilmerGames.Core.Singletons;
using UnityEngine;
using UnityEngine.InputSystem;


public class ControllerManager : Singleton<ControllerManager>
{
    [Header("Controller mapping")]
    [SerializeField]
    private InputActionProperty controllerMenuAction;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor[] cachedRayInteractors;

    [Header("Events")]
    public Action onControllerMenuActionExecuted;
    private string objLayer;
    private void Awake()
    {
        cachedRayInteractors = FindObjectsByType<UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
    }
    private void OnEnable()
    {
        controllerMenuAction.action.performed += ControllerMenuActionPerformed;
        GameManager.Instance.onGamePaused += ControllerRayInteractorInput;
        GameManager.Instance.onGameResumed += ControllerRayInteractorInput;
        GameManager.Instance.onGameSolved += ControllerRayInteractorInput;
    }
    private void OnDisable()
    {
        controllerMenuAction.action.performed -= ControllerMenuActionPerformed;
        GameManager.Instance.onGamePaused -= ControllerRayInteractorInput;
        GameManager.Instance.onGameResumed -= ControllerRayInteractorInput;
        GameManager.Instance.onGameSolved -= ControllerRayInteractorInput;
    }
    private void ControllerMenuActionPerformed(InputAction.CallbackContext obj)
    {
        onControllerMenuActionExecuted?.Invoke();
    }

    private void ControllerRayInteractorInput(GameState gameState = GameState.Playing)
    {
        foreach (var rayInteractor in cachedRayInteractors)
        {
            objLayer = LayerMask.LayerToName(rayInteractor.gameObject.layer);
            Debug.Log(objLayer);
            /*   rayInteractor.gameObject.SetActive(gameState == GameState.Paused);*/
            if (gameState == GameState.Paused)
                ApplyDefaultLayers(rayInteractor.transform, "UI");
            else
                //       ApplyDefaultLayers(rayInteractor.transform.parent, "Default");
                ApplyDefaultLayers(rayInteractor.transform, objLayer);
        }
    }
    private void ApplyDefaultLayers(Transform rayParent, string layerName)
    {
        LayerMask uiLayerMask = LayerMask.NameToLayer(layerName);
        rayParent.gameObject.layer = uiLayerMask;
        foreach (Transform transform in rayParent.GetComponent<Transform>())
        {
            transform.gameObject.layer = uiLayerMask;
        }
    }
}
