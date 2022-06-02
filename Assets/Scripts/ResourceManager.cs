using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public Resources defaultResources;
    public Resources defaultIncome;

    private Dictionary<ResourceType, int> resourcesDictionary = new Dictionary<ResourceType, int>();

    private Dictionary<ResourceType, int> incomePerSecond = new Dictionary<ResourceType, int>();
    
    private void Start()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            resourcesDictionary[type] = defaultResources.GetAttr(type);
            incomePerSecond[type] = defaultIncome.GetAttr(type);
        }
    }

    public int GetQuantityOfResource(ResourceType type)
    {
        return resourcesDictionary[type];
    }

    public void IncreaseResource(ResourceType type, int quantity = 1)
    {
        resourcesDictionary[type] += quantity;
    }

    public bool SpendResource(ResourceType type, int quantity = 0)
    {
        if (quantity > resourcesDictionary[type])
            return false;
        resourcesDictionary[type] -= quantity;
        return true;
    }

    public void CountIncome(Dictionary<StructurePrefab, int> dict)
    {
        foreach (var pair in dict)
        {
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                incomePerSecond[type] = defaultIncome.GetAttr(type) + pair.Key.resourcesIncome.GetAttr(type) * pair.Value;
            }
        }
    }

    public void SetNewResourcesValues()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            var income = incomePerSecond[type];
            if (income > 0)
                IncreaseResource(type, income);
            else
                SpendResource(type, income);
        }
    }
}

public enum ResourceType
{
    Money,
    Tree,
    Rock
}

[Serializable]
public class Resources
{
    public int money, tree, rock;

    public int GetAttr(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Money:
                return money;
            case ResourceType.Tree:
                return tree;
            case ResourceType.Rock:
                return rock;
            default:
                return -1;
        }
    }
}
