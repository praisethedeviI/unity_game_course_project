using System;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Renderer mainRenderer;
    public Vector2Int size = Vector2Int.one;
    public Color mainColor;

    private void Awake()
    {
        mainColor = mainRenderer.material.color;
    }
    
    public void SetTransparent(bool available)
    {
        mainRenderer.material.color = available ? new Color(0f, 1f, 0f, 0.33f) : new Color(1f, 0f, 0f, 0.33f);
        mainRenderer.material.shader = Shader.Find("Sprites/Default");
    }

    public void SetNormal()
    {
        mainRenderer.material.shader = Shader.Find("Standard");
        mainRenderer.material.color = mainColor;
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Gizmos.color = (x + y) % 2 == 0 ? Color.yellow : Color.blue;
                Gizmos.DrawCube(transform.position + new Vector3(x, 0, y), new Vector3(1, .1f, 1));
            }
        }
    }
}