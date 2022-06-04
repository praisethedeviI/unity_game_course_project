using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SVS;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public StructurePrefab[] housePrefabs, specialPrefabs;
    public PlacementManager placementManager;
    public ResourceManager resourceManager;

    private float[] houseWeights, specialWeights;

    private Dictionary<StructurePrefab, int> structuresQuantityDictionary = new Dictionary<StructurePrefab, int>();

    private void Start()
    {
        houseWeights = housePrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        specialWeights = specialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
    }

    private bool TrySpendResources(StructurePrefab prefab)
    {
        return resourceManager.SpendResource(ResourceType.Money, prefab.resourcesCost.money) &&
               resourceManager.SpendResource(ResourceType.Tree, prefab.resourcesCost.tree) &&
               resourceManager.SpendResource(ResourceType.Rock, prefab.resourcesCost.rock);
    }

    public void PlaceHouse(Vector3Int pos)
    {
        if (!CheckPositionBeforePlacement(pos))
            return;
        
        if (!TrySpendResources(housePrefabs[0]))
        {
            Debug.Log("Not enough resource!");
            return;
        }

        IncreaseQuantityOfStructuresDictionary(housePrefabs[0]);

        placementManager.PlaceObjectOnTheMap(pos, housePrefabs[0].prefab, CellType.Structure);
        RotateStructure(pos, housePrefabs[0].prefab);
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
        if (!TrySpendResources(specialPrefabs[0]))
        {
            Debug.Log("Not enough resource!");
            return;
        }

        if (!CheckPositionBeforePlacement(pos))
            return;

        IncreaseQuantityOfStructuresDictionary(specialPrefabs[0]);

        placementManager.PlaceObjectOnTheMap(pos, specialPrefabs[0].prefab, CellType.Structure);
        RotateStructure(pos, specialPrefabs[0].prefab);
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
    
    public GameObject prefab;
    [Range(0, 1)] public float weight;
}
