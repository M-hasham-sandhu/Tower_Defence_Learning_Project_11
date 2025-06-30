using UnityEngine;

public class TowerPlacementController : MonoBehaviour
{
    public GridSystem gridSystem;
    public TowerBuilder towerBuilder;
    public TowerType towerTypeToPlace = TowerType.Type_1;
    public LayerMask groundLayer;

    private GameObject previewTower;
    private bool isPlacing = false;

    void Update()
    {
        bool isTouch = Input.touchCount > 0;
        Vector3 pointerPos = Input.mousePosition;
        TouchPhase? touchPhase = null;
        if (isTouch)
        {
            var touch = Input.GetTouch(0);
            pointerPos = touch.position;
            touchPhase = touch.phase;
        }

        if (isPlacing && previewTower != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(pointerPos);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                if (gridSystem.TryGetGridPosition(hit.point, out int x, out int y))
                {
                    Vector3 snapPos = gridSystem.GetWorldPosition(x, y) + new Vector3(gridSystem.cellSize / 2f, 0, gridSystem.cellSize / 2f);
                    previewTower.transform.position = snapPos;

                    bool canPlace = !gridSystem.IsCellOccupied(x, y);

                    bool pointerDown = false;
                    if (isTouch)
                        pointerDown = touchPhase == TouchPhase.Began;
                    else
                        pointerDown = Input.GetMouseButtonDown(0);

                    if (pointerDown && canPlace)
                    {
                        gridSystem.SetOccupied(x, y, true);
                        GameObject placedTower = towerBuilder.BuildTower(towerTypeToPlace, 0, snapPos);
                        if (placedTower == null)
                        {
                            Debug.LogError($"BuildTower returned null for type {towerTypeToPlace} at position {snapPos}. Check TowerData assignment, prefab reference, and gold amount.");
                            Destroy(previewTower);
                            previewTower = null;
                            isPlacing = false;
                            return;
                        }

                        var sphere = placedTower.GetComponent<SphereCollider>();
                        if (sphere == null)
                            Debug.LogError($"No SphereCollider found on prefab: {placedTower.name}");
                        else
                            sphere.enabled = true;

                        Destroy(previewTower);
                        previewTower = null;
                        isPlacing = false;
                    }
                }
            }

            bool pointerUp = false;
            if (isTouch)
                pointerUp = touchPhase == TouchPhase.Ended || touchPhase == TouchPhase.Canceled;
            else
                pointerUp = Input.GetMouseButtonUp(1);
            if (pointerUp && previewTower != null)
            {
                Destroy(previewTower);
                previewTower = null;
                isPlacing = false;
            }
        }
        else if (!isPlacing)
        {
            bool pointerDown = false;
            if (isTouch)
                pointerDown = touchPhase == TouchPhase.Began;
            else
                pointerDown = Input.GetMouseButtonDown(0);

            if (pointerDown)
            {
                Ray ray = Camera.main.ScreenPointToRay(pointerPos);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Tower tower = hit.collider.GetComponent<Tower>();
                    if (tower != null)
                    {
                        tower.OnSelected();
                    }
                }
            }
        }
    }

    public void StartPlacingTower(TowerType type)
    {
        Debug.Log("StartPlacingTower called with type: " + type);
        if (previewTower != null)
            Destroy(previewTower);

        // Use the new preview method that does NOT spend gold
        previewTower = towerBuilder.GetPreviewTower(type);
        if (previewTower != null)
        {
            var sphere = previewTower.GetComponent<SphereCollider>();
            if (sphere != null)
                sphere.enabled = false;

            foreach (var r in previewTower.GetComponentsInChildren<Renderer>())
                r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, 0.5f);
        }
        else
        {
            Debug.LogWarning("Preview tower NOT instantiated! Check your prefab and TowerBuilder.");
        }
        isPlacing = true;
        towerTypeToPlace = type;
    }
}