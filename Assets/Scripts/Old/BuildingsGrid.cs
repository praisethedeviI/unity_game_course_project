using UnityEngine;
using UnityEngine.Serialization;


public class BuildingsGrid : MonoBehaviour
{
    [FormerlySerializedAs("GridSize")] public Vector2Int gridSize;
    private Building[,] _grid;
    private Building _flyingBuilding;
    private Camera _mainCamera;

    [FormerlySerializedAs("ObjectsTilemap")]
    public GameObject objectsTilemap;


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
        if (_flyingBuilding == null) return;
        var groundPlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);


        if (!groundPlane.Raycast(ray, out var position)) return;
        Vector3 mousePos = ray.GetPoint(position);

        var x = Mathf.RoundToInt(mousePos.x);
        var y = Mathf.RoundToInt(mousePos.z);

        var availableOnMousePos = CanPlaceBuilding(x, y);

        if (!Input.GetMouseButton(1))
        {
            MoveBuilding(mousePos);
            _flyingBuilding.SetTransparent(availableOnMousePos);
        }
        
        if (Input.GetMouseButton(1))
        {
            RotateBuilding(mousePos);
        }

        if (availableOnMousePos && Input.GetMouseButton(0))
        {
            PlaceFlyingBuilding(x, y);
        }
    }

    private void MoveBuilding(Vector3 mousePos)
    {
        var x = Mathf.RoundToInt(mousePos.x);
        var z = Mathf.RoundToInt(mousePos.z);
        _flyingBuilding.transform.position = new Vector3(x, 0, z);
    }

    private void RotateBuilding(Vector3 mousePos)
    {
        Transform tr;
        (tr = _flyingBuilding.transform).LookAt(new Vector3(mousePos.x, _flyingBuilding.transform.position.y,
            mousePos.z));
        var rot = tr.rotation;
        int y = Mathf.FloorToInt(rot.eulerAngles.y);
        y = (y > 45 && y <= 135) ? 90 : y;
        y = (y > 135 && y <= 225) ? 180 : y;
        y = (y > 215 && y <= 315) ? 270 : y;
        y = (y > 315 || y <= 45) ? 0 : y;
        _flyingBuilding.transform.rotation = Quaternion.Euler(rot.eulerAngles.x, y, rot.eulerAngles.z);
    }

    private bool CanPlaceBuilding(int x, int y)
    {
        var available = x >= 0 && x <= gridSize.x - _flyingBuilding.size.x && y >= 0 &&
                        y <= gridSize.y - _flyingBuilding.size.y;
        available = available && !IsPlaceTaken(x, y);
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            var obj = hit.transform.gameObject;
            if (obj.transform.parent != null && obj.transform.parent.gameObject.tag.Contains("Ground"))
            {
                var type = obj.GetComponentInParent<GroundObject>().groundTypes;
                available = type == GroundTypes.Grass && available;
            }
        }

        return available;
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