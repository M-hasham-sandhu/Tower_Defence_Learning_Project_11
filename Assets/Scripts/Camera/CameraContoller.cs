using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraContoller : MonoBehaviour
{
    public GameObject terrainObject; // Assign your box/mesh in Inspector
    public float cameraHeight = 30f;
    public float moveSpeed = 10f;
    public float drag = 5f; // Higher = stops faster, lower = more inertia

    private Vector3 minBounds;
    private Vector3 maxBounds;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (terrainObject == null)
        {
            Debug.LogError("Terrain object not assigned to CameraController!");
            return;
        }

        // Use the renderer bounds of the object
        Renderer rend = terrainObject.GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogError("Terrain object has no Renderer component!");
            return;
        }

        Bounds bounds = rend.bounds;
        minBounds = bounds.min;
        maxBounds = bounds.max;

        // Center camera above the object
        Vector3 center = bounds.center;
        transform.position = new Vector3(center.x, cameraHeight, center.z);
        transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Look straight down
    }

    void Update()
    {
        if (terrainObject == null) return;

        Vector3 input = Vector3.zero;

        // Keyboard input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        input += new Vector3(h, 0, v);

        // Mouse drag
        if (Input.GetMouseButton(1))
        {
            float dragX = -Input.GetAxis("Mouse X");
            float dragZ = -Input.GetAxis("Mouse Y");
            input += new Vector3(dragX, 0, dragZ);
        }

        // Touch drag
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                input += new Vector3(-delta.x, 0, -delta.y) * 0.1f;
            }
        }

        // Apply input to velocity
        if (input.sqrMagnitude > 0.01f)
        {
            velocity += input.normalized * moveSpeed * Time.deltaTime;
        }

        // Apply drag to velocity (smooth stop)
        velocity = Vector3.Lerp(velocity, Vector3.zero, drag * Time.deltaTime);

        // Move camera
        Vector3 newPos = transform.position + velocity;

        // Clamp camera within object bounds
        newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
        newPos.z = Mathf.Clamp(newPos.z, minBounds.z, maxBounds.z);
        newPos.y = cameraHeight;

        transform.position = newPos;
    }
}
