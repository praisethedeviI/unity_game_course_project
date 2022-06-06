using System;
using UnityEngine;

[Serializable]
public struct StructureInfo
{
    public int lvl;
    public StructurePrefab nextLvlPrefab;
    public StructurePrefab currentPrefab;
    public int randPos;
    public Resources currentIncome;
}