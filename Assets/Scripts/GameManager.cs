using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SVS;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CameraMovement cameraMovement;
    public RoadManager roadManager;
    public InputManager inputManager;
    public ResourceManager resourceManager;
    public UIController uiController;
    public StructureManager structureManager;

    private void Start()
    {
        InvokeRepeating(nameof(IncreaseResources), 0.01f, 1f);
        uiController.OnRoadPlacement += RoadPlacementHandler;
        uiController.OnHousePlacement += HousePlacementHandler;
        uiController.OnSpecialPlacement += SpecialPlacementHandler;
    }

    private void SpecialPlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += structureManager.PlaceSpecial;
    }

    private void HousePlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += structureManager.PlaceHouse;
    }

    private void RoadPlacementHandler()
    {
        ClearInputActions();

        inputManager.OnMouseClick += roadManager.PlaceRoad;
        inputManager.OnMouseHold += roadManager.PlaceRoad;
        inputManager.OnMouseUp += roadManager.FinishPlacingRoad;
    }

    private void ClearInputActions()
    {
        inputManager.OnMouseClick = null;
        inputManager.OnMouseHold = null;
        inputManager.OnMouseUp = null;
    }

    private void IncreaseResources() // по идее это должно быть в другом менеджере
    {
        var dict = structureManager.GetStructuresQuantityDictionary();
        if (dict.Count > 0)
            resourceManager.CountIncome(structureManager.GetStructuresQuantityDictionary());
        resourceManager.SetNewResourcesValues();
        uiController.moneyText.text = resourceManager.GetQuantityOfResource(ResourceType.Money).ToString();
        uiController.treeText.text = resourceManager.GetQuantityOfResource(ResourceType.Tree).ToString();
        uiController.rockText.text = resourceManager.GetQuantityOfResource(ResourceType.Rock).ToString();
    }

    private void Update()
    {
        cameraMovement.MoveCamera(new Vector3(inputManager.CameraMovementVector.x, 0,
            inputManager.CameraMovementVector.y));
    }
}
