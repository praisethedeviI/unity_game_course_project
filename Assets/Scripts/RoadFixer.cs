using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadFixer : MonoBehaviour
{
    public GameObject deadEnd, straight, corner, threeWay, fourWay;

    public void FixRoadAtPosition(PlacementManager placementManager, Vector3Int tempPos)
    {
        var result = placementManager.GetNeighbourTypesFor(tempPos);
        var roadCount = 0;
        roadCount = result.Count(x => x == CellType.Road);
        if (roadCount == 0 || roadCount == 1)
        {
            CreateDeadEnd(placementManager, result, tempPos);
        }
        else if (roadCount == 2)
        {
            if (CreateStraight(placementManager, result, tempPos))
                return;
            CreateCorner(placementManager, result, tempPos);
        }
        else if (roadCount == 3)
        {
            Create3Way(placementManager, result, tempPos);
        }
        else if (roadCount == 4)
        {
            Create4Way(placementManager, result, tempPos);
        }
    }

    private void Create4Way(PlacementManager placementManager, CellType[] result, Vector3Int pos)
    {
        placementManager.ModifyStructureModel(pos, fourWay, Quaternion.identity);
    }

    private void Create3Way(PlacementManager placementManager, CellType[] result, Vector3Int pos)
    {
        // result[1] == CellType.Road && result[2] == CellType.Road && result[3] == CellType.Road
        var yAxis = -1;
        for (var i = 0; i < 4; i++)
        {
            if (result[i] != CellType.Road)
            {
                yAxis = -90 + i * 90;
                break;
            }
        }

        if (yAxis != -1)
        {
            placementManager.ModifyStructureModel(pos, threeWay, Quaternion.Euler(0, yAxis, 0));
        }
    }

    private void CreateCorner(PlacementManager placementManager, CellType[] result, Vector3Int pos)
    {
        // result[1] == CellType.Road && result[2] == CellType.Road
        var yAxis = -1;
        for (var i = 0; i < 4; i++)
        {
            var j = i == 3 ? 0 : i + 1;
            if (result[i] == CellType.Road && result[j] == CellType.Road)
            {
                yAxis = -180 + i * 90;
                break;
            }
        }

        if (yAxis != -1)
        {
            placementManager.ModifyStructureModel(pos, corner, Quaternion.Euler(0, yAxis, 0));
        }
    }

    private bool CreateStraight(PlacementManager placementManager, CellType[] result, Vector3Int pos)
    {
        if (result[1] == CellType.Road && result[3] == CellType.Road)
        {
            placementManager.ModifyStructureModel(pos, straight, Quaternion.Euler(0, 0, 0));
            return true;
        }

        if (result[0] == CellType.Road && result[2] == CellType.Road)
        {
            placementManager.ModifyStructureModel(pos, straight, Quaternion.Euler(0, 90, 0));
            return true;
        }

        return false;
    }

    private void CreateDeadEnd(PlacementManager placementManager, CellType[] result, Vector3Int pos)
    {
        var yAxis = -1;
        for (var i = 0; i < 4; i++)
        {
            if (result[i] == CellType.Road)
            {
                yAxis = 90 + i * 90;
                break;
            }
        }

        if (yAxis != -1)
        {
            placementManager.ModifyStructureModel(pos, deadEnd, Quaternion.Euler(0, yAxis, 0));
        }
    }
}