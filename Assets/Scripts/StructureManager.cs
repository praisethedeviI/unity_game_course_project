using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public StructurePrefabWeighted[] housesPrefabs, specialPrefabs;
    public PlacementManager placementManager;

    private float[] houseWeights, specialWeights;

    private void Start()
    {
        houseWeights = housesPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        specialWeights = specialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
    }

    public void PlaceHouse(Vector3Int pos)
    {
        if (CheckPositionBeforePlacement(pos))
        {
            placementManager.PlaceObjectOnTheMap(pos, housesPrefabs[0].prefab, CellType.Structure);
            RotateStructure(pos, housesPrefabs[0].prefab);
        }
    }

    private void RotateStructure(Vector3Int pos, GameObject prefab)
    {
        var yAxis = -1;
        var result = placementManager.GetNeighbourTypesFor(pos);
        for (var i = 0; i < 4; i++)
        {
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
        if (CheckPositionBeforePlacement(pos))
        {
            placementManager.PlaceObjectOnTheMap(pos, specialPrefabs[0].prefab, CellType.Structure);
            RotateStructure(pos, specialPrefabs[0].prefab);
        }
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

        if (placementManager.GetNeighbourOfTypeFor(pos, CellType.Road).Count == 0)
        {
            Debug.Log("Must be placed near a road");
            return false;
        }
        return true;
    }
}

[Serializable]
public struct StructurePrefabWeighted
{
    public GameObject prefab;
    [Range(0, 1)]
    public float weight;
}
