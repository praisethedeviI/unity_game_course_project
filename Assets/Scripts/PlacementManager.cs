using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public int width, height;
    private Grid placementGrid;
    private Dictionary<Vector3Int, StructureModel> structureDictionary = new Dictionary<Vector3Int, StructureModel>();

    private Dictionary<Vector3Int, StructureModel> temporaryRoadObjects = new Dictionary<Vector3Int, StructureModel>();


    private void Start()
    {
        placementGrid = new Grid(width, height);
    }

    public bool CheckIfPositionInBound(Vector3Int pos)
    {
        if (pos.x >= 0 && pos.x < width && pos.z >= 0 && pos.z < height)
            return true;

        return false;
    }

    public bool CheckIfPositionIsFree(Vector3Int pos)
    {
        return CheckIfPositionIsOfType(pos, CellType.Empty);
    }

    private bool CheckIfPositionIsOfType(Vector3Int pos, CellType type)
    {
        return placementGrid[pos.x, pos.z] == type;
    }

    public void PlaceTemporaryStructure(Vector3Int pos, GameObject structurePrefab, CellType type)
    {
        placementGrid[pos.x, pos.z] = type;
        StructureModel structure = CreateANewStructureModel(pos, structurePrefab, type);
        temporaryRoadObjects.Add(pos, structure);
        DestroyGroundAt(pos);
    }

    public void PlaceGround(Vector3Int pos, GameObject prefab, CellType type)
    {
        placementGrid[pos.x, pos.z] = type;
        CreateANewStructureModel(pos, prefab, type);
        temporaryRoadObjects.Remove(pos);
    }

    private StructureModel CreateANewStructureModel(Vector3Int pos, GameObject structurePrefab, CellType type)
    {
        GameObject structure = new GameObject(type.ToString());
        structure.transform.SetParent(transform);
        structure.transform.localPosition = pos;
        var structureModel = structure.AddComponent<StructureModel>();
        structureModel.CreateModel(structurePrefab);
        return structureModel;
    }

    public void ModifyStructureModel(Vector3Int pos, GameObject newModel, Quaternion rotation)
    {
        if (temporaryRoadObjects.ContainsKey(pos))
            temporaryRoadObjects[pos].SwapModel(newModel, rotation);
        else if (structureDictionary.ContainsKey(pos))
            structureDictionary[pos].SwapModel(newModel, rotation);
    }

    public CellType[] GetNeighbourTypesFor(Vector3Int pos)
    {
        return placementGrid.GetAllAdjacentCellTypes(pos.x, pos.z);
    }

    public List<Vector3Int> GetNeighbourOfTypeFor(Vector3Int pos, CellType type)
    {
        var neighbourVertices = placementGrid.GetAdjacentCellsOfType(pos.x, pos.z, type);
        List<Vector3Int> neighbours = new List<Vector3Int>();
        foreach (var point in neighbourVertices)
        {
            neighbours.Add(new Vector3Int(point.X, 0, point.Y));
        }

        return neighbours;
    }

    public void RemoveAllTemporaryStructures()
    {
        foreach (var structure in temporaryRoadObjects.Values)
        {
            var pos = Vector3Int.RoundToInt(structure.transform.position);
            placementGrid[pos.x, pos.z] = CellType.Empty;
            Destroy(structure.gameObject);
        }

        temporaryRoadObjects.Clear();
    }

    public List<Vector3Int> GetPathBetween(Vector3Int startPosition, Vector3Int endPosition, bool isAgent = false)
    {
        var resultPath = GridSearch.AStarSearch(placementGrid,
            new Point(startPosition.x, startPosition.z),
            new Point(endPosition.x, endPosition.z), isAgent);
        List<Vector3Int> path = new List<Vector3Int>();
        foreach (var point in resultPath)
        {
            path.Add(new Vector3Int(point.X, 0, point.Y));
        }

        return path;
    }

    public void AddTemporaryStructuresToDictionary()
    {
        foreach (var structure in temporaryRoadObjects)
        {
            structureDictionary.Add(structure.Key, structure.Value);
            DestroyNatureAt(structure.Key);
            DestroyGroundAt(structure.Key);
        }

        temporaryRoadObjects.Clear();
    }

    public void PlaceObjectOnTheMap(Vector3Int pos, GameObject structurePrefab, CellType type)
    {
        placementGrid[pos.x, pos.z] = type;
        StructureModel structure = CreateANewStructureModel(pos, structurePrefab, type);
        structureDictionary.Add(pos, structure);
        DestroyNatureAt(pos);
    }

    private void DestroyNatureAt(Vector3Int pos)
    {
        RaycastHit[] hits = Physics.BoxCastAll(pos + new Vector3(0f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.5f),
            transform.up, Quaternion.identity, 1f, 1 << LayerMask.NameToLayer("Nature")
        );
        foreach (var item in hits)
        {
            Destroy(item.collider.transform.parent.gameObject);
        }
    }

    private void DestroyGroundAt(Vector3Int pos)
    {
        RaycastHit[] hits = Physics.BoxCastAll(pos, new Vector3(0.1f, 0.1f, 0.1f),
            transform.up, Quaternion.identity, 1f, 1 << LayerMask.NameToLayer("Ground")
        );
        foreach (var item in hits)
        {
            Destroy(item.collider.transform.parent.gameObject);
        }
    }

    public bool CheckIfPositionInWaterLayer(Vector3Int pos)
    {
        RaycastHit[] hits = Physics.BoxCastAll(pos, new Vector3(0.1f, 0.1f, 0.1f),
            transform.up, Quaternion.identity, 1f, 1 << LayerMask.NameToLayer("Water")
        );
        return hits.Length > 0;
    }

    public List<StructureModel> GetAllHouses()
    {
        List<StructureModel> returnList = new List<StructureModel>();
        var housePositions = placementGrid.GetAllHouses();
        foreach (var point in housePositions)
        {
            returnList.Add(structureDictionary[new Vector3Int(point.X, 0, point.Y)]);
        }

        return returnList;
    }

    internal List<StructureModel> GetAllSpecialStructures()
    {
        List<StructureModel> returnList = new List<StructureModel>();
        var housePositions = placementGrid.GetAllSpecialStructure();
        foreach (var point in housePositions)
        {
            returnList.Add(structureDictionary[new Vector3Int(point.X, 0, point.Y)]);
        }

        return returnList;
    }

    public StructureModel GetRandomSpecialStructure()
    {
        var point = placementGrid.GetRandomSpecialStructurePoint();
        return GetStructureAt(point);
    }

    public StructureModel GetRandomHouseStructure()
    {
        var point = placementGrid.GetRandomHouseStructurePoint();
        return GetStructureAt(point);
    }

    private StructureModel GetStructureAt(Point point)
    {
        if (point != null)
        {
            return structureDictionary[new Vector3Int(point.X, 0, point.Y)];
        }

        return null;
    }

    public StructureModel GetStructureAt(Vector3Int position)
    {
        if (structureDictionary.ContainsKey(position))
        {
            return structureDictionary[position];
        }

        return null;
    }
}