using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Serializable]
    public struct ResourceTextPair
    {
        public ResourceType type;
        public Text text;
    }
    
    
    
    
    public Action OnRoadPlacement, OnHousePlacement, OnSpecialPlacement, OnCloseMenu, OnModifyStructure;
    public Button placeRoadButton, placeHouseButton, placeSpecialButton, closeButton, modifyStructureButton;

    public GameObject modifyStructurePanel;
    
    public Text moneyText, treeText, rockText;

    public Color outlineColor;
    private List<Button> buttonList;

    public PlacementManager placementManager;
    
    
    public List<ResourceTextPair> modifyUpgradeText;
    public List<ResourceTextPair> modifyIncomeText;
    public Text currentLvlText;

    private void Start()
    {
        buttonList = new List<Button> {placeRoadButton, placeHouseButton, placeSpecialButton, modifyStructureButton};
        
        modifyStructureButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyOutline(modifyStructureButton);
            OnModifyStructure?.Invoke();
        });
        
        closeButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            OnCloseMenu?.Invoke();
        });

        placeRoadButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyOutline(placeRoadButton);
            OnRoadPlacement?.Invoke();
        });
        placeHouseButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyOutline(placeHouseButton);
            OnHousePlacement?.Invoke();
        });
        placeSpecialButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyOutline(placeSpecialButton);
            OnSpecialPlacement?.Invoke();
        });
    }

    private void ModifyOutline(Button button)
    {
        var outline = button.GetComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.enabled = true;
    }

    private void ResetButtonColor()
    {
        foreach (var button in buttonList)
        {
            button.GetComponent<Outline>().enabled = false;
        }
    }

    public void SetPanelActive(Vector3Int pos)
    {
        var structure = placementManager.GetStructureAt(pos);
        placementManager.currentPos = pos;
        if (structure == null) return;

        modifyStructurePanel.SetActive(true);
        
        SetInfoPanel(structure);
    }

    public void SetInfoPanel(StructureModel structure)
    {
        currentLvlText.text = structure.info.lvl.ToString();
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            var incomeText = GetTextByType(modifyIncomeText, type);
            incomeText.text = structure.info.currentIncome.GetAttr(type).ToString();

            var upgradeText = GetTextByType(modifyUpgradeText, type);
            upgradeText.text = structure.info.nextLvlPrefab.resourcesCost.GetAttr(type).ToString();
            if (structure.info.currentPrefab.prefab == structure.info.nextLvlPrefab.prefab)
            {
                upgradeText.text = "Maxed out";
            }
        }
    }

    private Text GetTextByType(List<ResourceTextPair> list, ResourceType type)
    {
        foreach (var pair in list)
        {
            if (pair.type == type)
            {
                return pair.text;
            }
        }

        return null;
    }
}