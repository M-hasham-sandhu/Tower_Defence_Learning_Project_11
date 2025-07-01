using UnityEngine;
using UnityEngine.EventSystems;

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
            // Always move preview to pointer/touch position
            Ray ray = Camera.main.ScreenPointToRay(pointerPos);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                if (gridSystem.TryGetGridPosition(hit.point, out int x, out int y))
                {
                    Vector3 snapPos = gridSystem.GetWorldPosition(x, y) + new Vector3(gridSystem.cellSize / 2f, 0, gridSystem.cellSize / 2f);
                    previewTower.transform.position = snapPos;

                    bool canPlace = !gridSystem.IsCellOccupied(x, y);

                    // Only place on second tap/click
                    bool pointerDown = false;
                    string touchPhaseStr = touchPhase.HasValue ? touchPhase.ToString() : "null";
                    bool overUI = false;
                    if (isTouch)
                    {
                        pointerDown = touchPhase == TouchPhase.Began;
                        // Check if touch is over UI
                        if (Input.touchCount > 0)
                            overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
                    }
                    else
                    {
                        pointerDown = Input.GetMouseButtonDown(0);
                        // Check if mouse is over UI
                        overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
                    }

                    if (pointerDown && canPlace && !overUI)
                    {
                        gridSystem.SetOccupied(x, y, true);
                        GameObject placedTower = towerBuilder.BuildTower(towerTypeToPlace, 0, snapPos);

                        if (placedTower == null)
                        {
                            Destroy(previewTower);
                            previewTower = null;
                            isPlacing = false;
                            return;
                        }

                        var sphere = placedTower.GetComponent<SphereCollider>();
                        if (sphere != null)
                            sphere.enabled = true;

                        Destroy(previewTower);
                        previewTower = null;
                        isPlacing = false;
                    }
                }
            }
        }
        else if (!isPlacing)
        {
            bool pointerDown = false;
            bool overUI = false;
            if (isTouch)
            {
                pointerDown = touchPhase == TouchPhase.Began;
                if (Input.touchCount > 0)
                    overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            }
            else
            {
                pointerDown = Input.GetMouseButtonDown(0);
                overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            }

            if (pointerDown && !overUI)
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