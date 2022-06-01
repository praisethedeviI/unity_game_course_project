using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 10f;
    public float panBorderThickness = 10f;
    public GameObject grid;
    
    
    
    public float scrollSpeed = 10f;
    
    // Update is called once per frame
    void Update()
    {
        Transform tr = transform;
        Vector3 pos = tr.position;
        Vector3 rot = tr.rotation.eulerAngles;

        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime * Mathf.Cos(Mathf.Deg2Rad * rot.y);
            pos.x += panSpeed * Time.deltaTime * Mathf.Sin(Mathf.Deg2Rad * rot.y);
        }
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime * Mathf.Sin(Mathf.Deg2Rad * rot.y);
            pos.x -= panSpeed * Time.deltaTime * Mathf.Cos(Mathf.Deg2Rad * rot.y);
        }
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime * Mathf.Cos(Mathf.Deg2Rad * rot.y);
            pos.x -= panSpeed * Time.deltaTime * Mathf.Sin(Mathf.Deg2Rad * rot.y);
        }
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime * Mathf.Sin(Mathf.Deg2Rad * rot.y);
            pos.x += panSpeed * Time.deltaTime * Mathf.Cos(Mathf.Deg2Rad * rot.y);
        }

        var scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;
        rot.x = 7.8f * pos.y + 6.8f;

        Vector2 panLimit = grid.gameObject.GetComponent<BuildingsGrid>().gridSize;

        pos.y = Mathf.Clamp(pos.y, 3, 7);
        rot.x = Mathf.Clamp(rot.x, 30, 60);

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);

        tr.rotation = Quaternion.Euler(rot);
        tr.position = pos;
    }
}
