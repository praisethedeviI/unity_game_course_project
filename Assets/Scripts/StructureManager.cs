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

    private Dictionary<StructurePrefab, int> structuresQuantityDictionary = new Dictionary<StructurePrefab, int>();

    private bool TrySpendResources(StructurePrefab prefab)
    {
        return resourceManager.SpendResource(ResourceType.Money, prefab.resourcesCost.money) &&
               resourceManager.SpendResource(ResourceType.Tree, prefab.resourcesCost.tree) &&
               resourceManager.SpendResource(ResourceType.Rock, prefab.resourcesCost.rock);
    }

    public void TryUpgradeHouseToSecondLevel(Vector3Int pos)
    {
        var randHouse = UnityEngine.Random.Range(0, houseLvl2Prefabs.Length);

        if (!TrySpendResources(houseLvl2Prefabs[randHouse]))
        {
            Debug.Log("Not enough resource!");
            return;
        }
        
        IncreaseQuantityOfStructuresDictionary(houseLvl2Prefabs[randHouse]);
        placementManager.DestroyStructureAt(pos);
        placementManager.PlaceObjectOnTheMap(pos, houseLvl2Prefabs[randHouse].prefab, CellType.Structure);
        RotateStructure(pos, houseLvl2Prefabs[randHouse].prefab);

        AudioPlayer.instance.PlayPlacementSound();
        
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
        aiDirector.SpawnRandomCountOfAgents(placementManager.GetStructureAt(pos));

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
