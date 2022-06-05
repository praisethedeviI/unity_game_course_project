using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    public PlacementManager placementManager;
    public GameObject[] pedestrianPrefabs;
    public int maxPedestriansPerHouse = 2;
    public int minPedestriansPerHouse = 2;
    
    public void SpawnAllAgents()
    {
        foreach (var house in placementManager.GetAllHouses())
        {
            SpawnRandomCountOfAgents(house);
        }
    }

    public void SpawnRandomCountOfAgents(StructureModel house)
    {
        for (var i = 0; i < Random.Range(minPedestriansPerHouse, maxPedestriansPerHouse); i++)
        {
            var pos = Vector3Int.RoundToInt(house.transform.position);
            var endStructure = GetEndStructureModel(pos);
            if (endStructure == null)
                return;
            TrySpawningAnAgent(house, endStructure);
        }
    }

    [CanBeNull]
    public StructureModel GetEndStructureModel(Vector3Int pos)
    {
        return placementManager.GetRandomHouseStructure(new Point(pos.x, pos.z));
    }

    [CanBeNull]
    public List<Vector3> GetPath(Vector3 start, Vector3 end)
    {
        var path = placementManager.GetPathBetween(Vector3Int.RoundToInt(start), Vector3Int.RoundToInt(end), true);
        if (path.Count > 0)
        {
            path.Reverse();
            var list = new List<Vector3>();
            foreach (var pos in path)
            {
                var xAdd = Random.Range(-0.3f, 0.3f);
                var zAdd = Random.Range(-0.3f, 0.3f);
                list.Add(new Vector3(xAdd, 0, zAdd) + pos);
            }

            list[0] = start;
            return list;
        }

        return null;
    }

    private void TrySpawningAnAgent(StructureModel startStructure, StructureModel endStructure)
    {
        if (startStructure != null && endStructure != null)
        {
            var startPosition = ((INeedingRoad)startStructure).RoadPosition;
            var endPosition = ((INeedingRoad)endStructure).RoadPosition;
            var path = placementManager.GetPathBetween(startPosition, endPosition, true);
            if (path.Count > 0)
            {
                var agent = Instantiate(GetRandomPedestrian(), startPosition, Quaternion.identity);
                agent.transform.SetParent(transform);
                path.Reverse();
                var AiAgent = agent.GetComponent<AIAgent>();
                AiAgent.aiDirector = this;

                var list = new List<Vector3>();
                foreach (var pos in path)
                {
                    var xAdd = Random.Range(-0.3f, 0.3f);
                    var zAdd = Random.Range(-0.3f, 0.3f);
                    list.Add(new Vector3(xAdd, 0, zAdd) + pos);
                }
                
                AiAgent.Initialize(list);
            }
        }
    }

    private GameObject GetRandomPedestrian()
    {
        return pedestrianPrefabs[UnityEngine.Random.Range(0, pedestrianPrefabs.Length)];
    }
}