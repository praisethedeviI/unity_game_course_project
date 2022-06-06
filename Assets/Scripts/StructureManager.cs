using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SVS;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = System.Random;

public class StructureManager : MonoBehaviour
{
    public StructurePrefab[] houseLvl1Prefabs, houseLvl2Prefabs, houseLvl3Prefabs, specialPrefabs;
    public PlacementManager placementManager;
    public ResourceManager resourceManager;
    public AIDirector aiDirector;
    public UIController uiController;

    private Dictionary<StructurePrefab, int> structuresQuantityDictionary = new Dictionary<StructurePrefab, int>();

    private bool TrySpendResources(StructurePrefab prefab)
    {
        return resourceManager.SpendResource(ResourceType.Money, prefab.resourcesCost.money) &&
               resourceManager.SpendResource(ResourceType.Tree, prefab.resourcesCost.tree) &&
               resourceManager.SpendResource(ResourceType.Rock, prefab.resourcesCost.rock);
    }

    public void TryUpgradeHouseToNextLevel()
    {
        var pos = placementManager.currentPos;
        var house = placementManager.GetStructureAt(pos);
        if (house.info.currentPrefab.prefab == house.info.nextLvlPrefab.prefab) 
            return;
        var newHousePrefab = house.info.nextLvlPrefab;
        if (!TrySpendResources(newHousePrefab))
        {
            Debug.Log("Not enough resource!");
            return;
        }

        Debug.Log(house.info.randPos);
        
        IncreaseQuantityOfStructuresDictionary(newHousePrefab);
        DestroyHouse();
        placementManager.PlaceObjectOnTheMap(pos, newHousePrefab.prefab, CellType.Structure);
        RotateStructure(pos, newHousePrefab.prefab);
        var newHouseModel = placementManager.GetStructureAt(pos);
        
        newHouseModel.info = new StructureInfo
        {
            lvl = house.info.lvl + 1,
            randPos = house.info.randPos,
            currentIncome = newHousePrefab.resourcesIncome,
            nextLvlPrefab = newHousePrefab,
            currentPrefab = newHousePrefab
        };
        
        if (newHouseModel.info.lvl == 2)
        {
            newHouseModel.info.nextLvlPrefab = houseLvl3Prefabs[house.info.randPos];
        }
        uiController.SetInfoPanel(newHouseModel);
        uiController.modifyStructurePanel.SetActive(false);
        AudioPlayer.instance.PlayPlacementSound();
    }

    public void DestroyHouse()
    {
        var pos = placementManager.currentPos;
        var structure = placementManager.GetStructureAt(pos);
        DecreaseQuantityOfStructuresDictionary(structure.info.currentPrefab);
        placementManager.DestroyStructureAt(pos);
    }

    private void DecreaseQuantityOfStructuresDictionary(StructurePrefab prefab)
    {
        structuresQuantityDictionary[prefab] -= 1;
        if (structuresQuantityDictionary[prefab] == 0)
        {
            structuresQuantityDictionary.Remove(prefab);
        }
    }

    public void PlaceHouse(Vector3Int pos)
    {
        if (!CheckPositionBeforePlacement(pos))
            return;
        
        var randHouse = UnityEngine.Random.Range(0, houseLvl1Prefabs.Length);

        if (!TrySpendResources(houseLvl1Prefabs[randHouse]))
        {
            Debug.Log("Not enough resource!");
            return;
        }
        
        IncreaseQuantityOfStructuresDictionary(houseLvl1Prefabs[randHouse]);

        placementManager.PlaceObjectOnTheMap(pos, houseLvl1Prefabs[randHouse].prefab, CellType.Structure);
        
        RotateStructure(pos, houseLvl1Prefabs[randHouse].prefab);
        var house = placementManager.GetStructureAt(pos);
        house.info = new StructureInfo
        {
            lvl = 1,
            currentPrefab = houseLvl1Prefabs[randHouse],
            nextLvlPrefab = houseLvl2Prefabs[randHouse],
            currentIncome = houseLvl1Prefabs[randHouse].resourcesIncome,
            randPos = randHouse
        };
        aiDirector.SpawnRandomCountOfAgents(house);

        AudioPlayer.instance.PlayPlacementSound();
    }

    private void IncreaseQuantityOfStructuresDictionary(StructurePrefab prefab)
    {
        if (!structuresQuantityDictionary.ContainsKey(prefab))
            structuresQuantityDictionary.Add(prefab, 0);

        structuresQuantityDictionary[prefab] += 1;
    }

    public Dictionary<StructurePrefab, int> GetStructuresQuantityDictionary()
    {
        return structuresQuantityDictionary;
    }
    
    //[left, bottom, right, up]
    private void RotateStructure(Vector3Int pos, GameObject prefab)
    {
        var yAxis = -1;
        var result = placementManager.GetNeighbourTypesFor(pos);
        for (var i = 0; i < 4; i++)
        {
            // var tmpPos = i >= 2 ? i - 2 : i + 2; 
            if (result[i] == CellType.Road)
            {
                yAxis = -90 + i * 90;
                break;
            }
        }

        if (yAxis != 1)
        {
            placementManager.ModifyStructureModel(pos, prefab, Quaternion.Euler(0, yAxis, 0));
        }
    }

    public void PlaceSpecial(Vector3Int pos)
    {
        if (!CheckPositionBeforePlacement(pos))
            return;

        var treeHits = placementManager.GetAllObjectHitsByLayerAt(pos, "Tree");
        var rockHits = placementManager.GetAllObjectHitsByLayerAt(pos, "Rock");

        if (treeHits.Length <= 0 && rockHits.Length <= 0)
        {
            Debug.Log("There is no tree and rock");
            return;
        }
        StructurePrefab specialPrefab;
        if (treeHits.Length > 0 && rockHits.Length > 0)
        {
            var randSpecial = UnityEngine.Random.Range(0, treeHits.Length);
            specialPrefab = specialPrefabs[randSpecial];
        }
        else if (treeHits.Length > 0)
        {
            specialPrefab = specialPrefabs[0];
        }
        else
        {
            specialPrefab = specialPrefabs[1];
        }
        
        
        if (!TrySpendResources(specialPrefab))
        {
            Debug.Log("Not enough resource!");
            return;
        }
        

        IncreaseQuantityOfStructuresDictionary(specialPrefab);

        placementManager.PlaceObjectOnTheMap(pos, specialPrefab.prefab, CellType.Structure);
        var special = placementManager.GetStructureAt(pos);
        special.info = new StructureInfo
        {
            currentPrefab = specialPrefab,
            nextLvlPrefab = specialPrefab,
            lvl = 1,
            currentIncome = specialPrefab.resourcesIncome
        };
        RotateStructure(pos, specialPrefab.prefab);
        AudioPlayer.instance.PlayPlacementSound();
    }

    private bool CheckPositionBeforePlacement(Vector3Int pos)
    {
        if (!placementManager.CheckIfPositionInBound(pos))
        {
            Debug.Log("Out of bounds");
            return false;
        }

        if (!placementManager.CheckIfPositionIsFree(pos))
        {
            Debug.Log("Position isn't empty");
            return false;
        }

        if (placementManager.GetNeighboursOfTypeFor(pos, CellType.Road).Count == 0)
        {
            Debug.Log("Must be placed near a road");
            return false;
        }

        return true;
    }
}

[Serializable]
public struct StructurePrefab
{
    public Resources resourcesIncome;
    public Resources resourcesCost;

    public LayerMask layer;

    public GameObject prefab;
}
