using UnityEngine;
using UnityEngine.Serialization;

public class Building : MonoBehaviour
{
    [FormerlySerializedAs("MainRenderer")] public Renderer mainRenderer;
    [FormerlySerializedAs("Size")] public Vector2Int size = Vector2Int.one;
    [FormerlySerializedAs("MainColor")] public Color mainColor;

    private void Awake()
    {
        mainColor = mainRenderer.material.color;
    }

    public void SetTransparent(bool availiable)
    {
        if (availiable)
        {
            mainRenderer.material.color = Color.green;
        }
        else
        {
            mainRenderer.material.color = Color.red;
        }
    }

    public void SetNormal()
    {
        mainRenderer.material.color = mainColor;
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if ((x + y) % 2 == 0)
                    Gizmos.color = new Color(1f, 0f, 0.36f, 0.31f);
                else
                    Gizmos.color = new Color(0.24f, 0.04f, 1f, 0.31f);
                Gizmos.DrawCube(transform.position + new Vector3(x, 0, y), new Vector3(1, .1f, 1));
            }
        }
    }
}