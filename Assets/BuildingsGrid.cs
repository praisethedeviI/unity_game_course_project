using UnityEngine;
using UnityEngine.Serialization;

public class BuildingsGrid : MonoBehaviour
{
    [FormerlySerializedAs("GridSize")] public Vector2Int gridSize;
    private Building[,] _grid;
    private Building _flyingBuilding;
    private Camera _mainCamera;
    [FormerlySerializedAs("ObjectsTilemap")] public GameObject objectsTilemap;
    

    void Awake()
    {
        _grid = new Building[gridSize.x, gridSize.y];

        _mainCamera = Camera.main;
    }

    public void StartPlacingBuilding(Building buildingPrefab)
    {
        if (_flyingBuilding != null)
            Destroy(_flyingBuilding.gameObject);
        _flyingBuilding = Instantiate(buildingPrefab, objectsTilemap.transform);
    }

    void Update()
    {
        if (_flyingBuilding != null)
        {
            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            
            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 worldPosition = ray.GetPoint(position);
                
                int x = Mathf.RoundToInt(worldPosition.x);
                int y = Mathf.RoundToInt(worldPosition.z);

                bool available = !(x < 0 || x > gridSize.x - _flyingBuilding.size.x || y < 0 ||
                                   y > gridSize.y - _flyingBuilding.size.y || IsPlaceTaken(x, y));
                
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject obj = hit.transform.gameObject;
                    if (obj.transform.parent != null && obj.transform.parent.gameObject.tag.Contains("Ground"))
                    {
                        GroundTypes type = obj.GetComponentInParent<GroundObject>().groundTypes;

                        if (type != GroundTypes.Grass)
                        {
                            available = false;
                        }
                    }
                }

                _flyingBuilding.transform.position = new Vector3(x, 0, y);
                _flyingBuilding.SetTransparent(available);

                if (available && Input.GetMouseButtonUp(0))
                {
                    PlaceFlyingBuilding(x, y);
                }
            }
        }
    }

    private bool IsPlaceTaken(int placeX, int placeY)
    {
        for (int x = 0; x < _flyingBuilding.size.x; x++)
        {
            for (int y = 0; y < _flyingBuilding.size.y; y++)
            {
                if (_grid[placeX + x, placeY + y] != null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void PlaceFlyingBuilding(int placeX, int placeY)
    {
        for (int x = 0; x < _flyingBuilding.size.x; x++)
        {
            for (int y = 0; y < _flyingBuilding.size.y; y++)
            {
                _grid[placeX + x, placeY + y] = _flyingBuilding;
            }
        }
        
        _flyingBuilding.SetNormal();
        _flyingBuilding = null;
    }
}