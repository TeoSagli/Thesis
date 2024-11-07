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
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor[] cachedRayInteractors;

    [Header("Events")]
    public Action onControllerMenuActionExecuted;
    private void Awake()
    {
        cachedRayInteractors = FindObjectsByType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
    }
    private void OnEnable()
    {
        // bind controller events
        controllerMenuAction.action.performed += ControllerMenuActionPerformed;

        // bind to game manager events
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
            rayInteractor.gameObject.SetActive(gameState == GameState.Paused);
            if (gameState == GameState.Paused)
                ApplyDefaultLayers(rayInteractor.transform.parent, "UI");
            else
                ApplyDefaultLayers(rayInteractor.transform.parent, "Default");
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
