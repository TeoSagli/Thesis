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
    /// <summary>
    /// Subscribe the other managers to the actions to perform
    /// </summary>
    private void OnEnable()
    {
        controllerMenuAction.action.performed += ControllerMenuActionPerformed;
        GameManager.Instance.onGamePaused += ControllerRayInteractorInput;
        GameManager.Instance.onGameResumed += ControllerRayInteractorInput;
        GameManager.Instance.onGameSolved += ControllerRayInteractorInput;
    }
    /// <summary>
    /// Unsubscribe the other managers to the actions to perform
    /// </summary>
    private void OnDisable()
    {
        controllerMenuAction.action.performed -= ControllerMenuActionPerformed;
        GameManager.Instance.onGamePaused -= ControllerRayInteractorInput;
        GameManager.Instance.onGameResumed -= ControllerRayInteractorInput;
        GameManager.Instance.onGameSolved -= ControllerRayInteractorInput;
    }
    /// <summary>
    /// Add menu binding when button pressed
    /// </summary>
    private void ControllerMenuActionPerformed(InputAction.CallbackContext obj)
    {
        onControllerMenuActionExecuted?.Invoke();
    }
    /// <summary>
    /// Apply layers to rays depending on the current game state
    /// </summary>
    private void ControllerRayInteractorInput(GameState gameState = GameState.Playing)
    {
        foreach (var rayInteractor in cachedRayInteractors)
        {
            objLayer = LayerMask.LayerToName(rayInteractor.gameObject.layer);
            Debug.Log(objLayer);
            /*   rayInteractor.gameObject.SetActive(gameState == GameState.Paused);*/
            if (gameState == GameState.Paused)
                ApplyLayersToRays(rayInteractor.transform, "UI");
            else
                //       ApplyLayersToRays(rayInteractor.transform.parent, "Default");
                ApplyLayersToRays(rayInteractor.transform, objLayer);
        }
    }
    /// <summary>
    /// Subscribe the other managers to the actions to perform
    /// </summary>
    /// <param name="rayParent">Current ray interactors.</param>
    /// <param name="layerName">Layers to apply.</param>
    private void ApplyLayersToRays(Transform rayParent, string layerName)
    {
        LayerMask uiLayerMask = LayerMask.NameToLayer(layerName);
        rayParent.gameObject.layer = uiLayerMask;
        foreach (Transform transform in rayParent.GetComponent<Transform>())
        {
            transform.gameObject.layer = uiLayerMask;
        }
    }
}
