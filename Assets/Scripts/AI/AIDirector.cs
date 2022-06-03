using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    public PlacementManager placementManager;
    public GameObject[] pedestrianPrefabs;

    public void SpawnAllAgents()
    {
        foreach (var house in placementManager.GetAllHouses())
        {
            TrySpawningAnAgent(house, placementManager.GetRandomHouseStructure());
        }

        foreach (var specialStructure in placementManager.GetAllSpecialStructures())
        {
            TrySpawningAnAgent(specialStructure, placementManager.GetRandomSpecialStructure());
        }
    }

    private void TrySpawningAnAgent(StructureModel startStructure, StructureModel endStructure)
    {
        if (startStructure != null && endStructure != null)
        {
            var startPosition = ((INeedingRoad)startStructure).RoadPosition;
            var endPosition = ((INeedingRoad)endStructure).RoadPosition;
            var agent = Instantiate(GetRandomPedestrian(), startPosition, Quaternion.identity);
            var path = placementManager.GetPathBetween(startPosition, endPosition, true);
            if (path.Count > 0)
            {
                path.Reverse();
                var AiAgent = agent.GetComponent<AIAgent>();
                var list = new List<Vector3>(path.Select(x => (Vector3) x).ToList());
                AiAgent.Initialize(new List<Vector3>(path.Select(x => (Vector3)x).ToList()));
            }
        }
    }

    private GameObject GetRandomPedestrian()
    {
        return pedestrianPrefabs[UnityEngine.Random.Range(0, pedestrianPrefabs.Length)];
    }
}