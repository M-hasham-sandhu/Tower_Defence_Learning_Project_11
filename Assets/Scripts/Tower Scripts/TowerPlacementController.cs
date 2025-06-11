using UnityEngine;

public class TowerPlacementController : MonoBehaviour
{
    public GridSystem gridSystem;
    public TowerBuilder towerBuilder;
    public TowerType towerTypeToPlace = TowerType.Type_1; // Set this from UI or Inspector

    private GameObject previewTower;
    private bool isPlacing = false;

    void Update()
    {
        // Start placement with a key or UI (example: P key)
        if (!isPlacing && Input.GetKeyDown(KeyCode.P))
        {
            StartPlacingTower(towerTypeToPlace);
        }

        if (isPlacing && previewTower != null)
        {
            Vector3 pointerPos = Vector3.zero;
            bool pointerDown = false;
            bool pointerUp = false;

            // Mouse support
            if (Input.mousePresent)
            {
                pointerPos = Input.mousePosition;
                pointerDown = Input.GetMouseButtonDown(0);
                pointerUp = Input.GetMouseButtonUp(0);
            }
            // Touch support
            if (Input.touchCount > 0)
            {
                pointerPos = Input.GetTouch(0).position;
                pointerDown = Input.GetTouch(0).phase == TouchPhase.Began;
                pointerUp = Input.GetTouch(0).phase == TouchPhase.Ended;
            }

            Ray ray = Camera.main.ScreenPointToRay(pointerPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (gridSystem.TryGetGridPosition(hit.point, out int x, out int y))
                {
                    Vector3 snapPos = gridSystem.GetWorldPosition(x, y);
                    previewTower.transform.position = snapPos;

                    // Optional: Visual feedback for occupied cells
                    bool canPlace = !gridSystem.IsCellOccupied(x, y);

                    // Place tower on click/tap if cell is free
                    if (pointerDown && canPlace)
                    {
                        gridSystem.SetOccupied(x, y, true);
                        towerBuilder.BuildTower(towerTypeToPlace, 1, snapPos);
                        Destroy(previewTower);
                        previewTower = null;
                        isPlacing = false;
                    }
                }
            }

            // Cancel placement on right click or two-finger tap (optional)
            if (pointerUp && previewTower != null)
            {
                Destroy(previewTower);
                previewTower = null;
                isPlacing = false;
            }
        }
    }

    public void StartPlacingTower(TowerType type)
    {
        if (previewTower != null)
            Destroy(previewTower);

        previewTower = towerBuilder.BuildTower(type, 1, Vector3.zero);
        if (previewTower != null)
        {
            // Optional: Make preview semi-transparent
            foreach (var r in previewTower.GetComponentsInChildren<Renderer>())
                r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, 0.5f);
        }
        isPlacing = true;
        towerTypeToPlace = type;
    }
}