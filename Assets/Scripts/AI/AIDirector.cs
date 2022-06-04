using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    public PlacementManager placementManager;
    public GameObject[] pedestrianPrefabs;
    public int maxPedestriansPerHouse = 2;
    public int minPedestriansPerHouse = 2;

    public void SpawnAllAgents()
    {
        Debug.Log(placementManager.GetAllHouses());
        foreach (var house in placementManager.GetAllHouses())
        {
            for (var i = 0; i < UnityEngine.Random.Range(minPedestriansPerHouse, maxPedestriansPerHouse); i++)
            {
                TrySpawningAnAgent(house, placementManager.GetRandomHouseStructure());
            }
        }
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
                path.Reverse();
                var AiAgent = agent.GetComponent<AIAgent>();
                
                var list = new List<Vector3>();
                foreach (var pos in path)
                {
                    var xAdd = UnityEngine.Random.Range(-0.5f, 0.5f);
                    var zAdd = UnityEngine.Random.Range(-0.5f, 0.5f);
                    list.Add(new Vector3(xAdd, 0, zAdd) + (Vector3) pos);
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