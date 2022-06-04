using System;
using System.Collections;
using System.Collections.Generic;
using SVS;
using UnityEngine;
using UnityEngine.Serialization;

public class RoadManager : MonoBehaviour
{
    public PlacementManager placementManager;

    public List<Vector3Int> temporaryPlacementPositions = new List<Vector3Int>();
    public List<Vector3Int> roadPositionsToRecheck = new List<Vector3Int>();

    private Vector3Int startPosition;
    private bool placementMode = false;

    public GameObject ground;
    
    public GameObject roadStraight;

        public RoadFixer roadFixer;

    private void Start()
    {
        roadFixer = GetComponent<RoadFixer>();
    }

    public void PlaceRoad(Vector3Int pos)
    {
        if (placementManager.CheckIfPositionInBound(pos) == false)
            return;

        if (placementManager.CheckIfPositionIsFree(pos) == false)
            return;
        
        if (placementManager.CheckIfPositionInWaterLayer(pos))
            return;

        if (!placementMode)
        {
            roadPositionsToRecheck.Clear();
            temporaryPlacementPositions.Clear();
            
            placementMode = true;
            startPosition = pos;
            
            temporaryPlacementPositions.Add(pos);
            placementManager.PlaceTemporaryStructure(pos, roadFixer.deadEnd, CellType.Road);
        }
        else
        {
            placementManager.RemoveAllTemporaryStructures();

            var oldTemporaryPlacementPositions = new List<Vector3Int>(temporaryPlacementPositions);

            temporaryPlacementPositions.Clear();
            
            foreach (var posToFix in roadPositionsToRecheck)
            {
                roadFixer.FixRoadAtPosition(placementManager, posToFix);
            }
            
            roadPositionsToRecheck.Clear();

            temporaryPlacementPositions = placementManager.GetPathBetween(startPosition, pos);

            foreach (var tempPos in oldTemporaryPlacementPositions)
            {
                if(!temporaryPlacementPositions.Contains(tempPos) && placementManager.CheckIfPositionIsFree(tempPos))
                    placementManager.PlaceGround(tempPos, ground, CellType.Empty);
            }

            foreach (var tempPos in temporaryPlacementPositions)
            {
                if (placementManager.CheckIfPositionIsFree(tempPos))
                    placementManager.PlaceTemporaryStructure(tempPos, roadFixer.deadEnd, CellType.Road);
            }
        }

        FixRoadPrefabs();
    }
    
    private void FixRoadPrefabs()
    {
        foreach (var tempPos in temporaryPlacementPositions)
        {
            roadFixer.FixRoadAtPosition(placementManager, tempPos);
            var neighbours = placementManager.GetNeighboursOfTypeFor(tempPos, CellType.Road);
            foreach (var roadPos in neighbours)
            {
                if (!roadPositionsToRecheck.Contains(roadPos))
                {
                    roadPositionsToRecheck.Add(roadPos);
                }
            }
        }

        foreach (var posToFix in roadPositionsToRecheck)
        {
            roadFixer.FixRoadAtPosition(placementManager, posToFix);
        }
    }
    
    public void FinishPlacingRoad()
    {
        placementMode = false;
        placementManager.AddTemporaryStructuresToDictionary();
        if (temporaryPlacementPositions.Count > 0)
        {
            AudioPlayer.instance.PlayPlacementSound();
        }
        temporaryPlacementPositions.Clear();
        startPosition = Vector3Int.zero;
    }
}
