//	Created by: Sunny Valley Studio 
//	https://svstudio.itch.io

using System;
using UnityEngine;

namespace SVS
{

    public class CameraMovement : MonoBehaviour
    {
        public Camera gameCamera;
        public float cameraMovementSpeed = 5;
        public float cameraScrollSpeed = 10f;

        private void Start()
        {
            gameCamera = GetComponent<Camera>();
        }
        public void MoveCamera(Vector3 inputVector)
        {
            var movementVector = Quaternion.Euler(0,30,0) * inputVector;
            gameCamera.transform.position += movementVector * Time.deltaTime * cameraMovementSpeed;
        }

        private void Update()
        {
            Transform tr = transform;
            Vector3 pos = tr.position;
            Vector3 rot = tr.rotation.eulerAngles;
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            pos.y -= scroll * cameraScrollSpeed * 100f * Time.deltaTime;
            rot.x = 7.8f * pos.y + 6.8f;
            
            pos.y = Mathf.Clamp(pos.y, 3, 7);
            rot.x = Mathf.Clamp(rot.x, 30.2f, 60);
            
            tr.rotation = Quaternion.Euler(rot);
            tr.position = pos;
        }
    }
}