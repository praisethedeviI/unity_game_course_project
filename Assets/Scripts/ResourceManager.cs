using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private Resources _resources = new Resources();

    
}

[Serializable]
public struct Resources
{
    [Range(0, Single.MinValue)] public float money, tree, rock;
    
    
}
