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
       
        if (isPlacing && previewTower != null)
        {
            Vector3 pointerPos = Input.mousePosition;
            if (Input.touchCount > 0)
                pointerPos = Input.GetTouch(0).position;

            Ray ray = Camera.main.ScreenPointToRay(pointerPos);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                if (gridSystem.TryGetGridPosition(hit.point, out int x, out int y))
                {
                    Vector3 snapPos = gridSystem.GetWorldPosition(x, y) + new Vector3(gridSystem.cellSize / 2f, 0, gridSystem.cellSize / 2f);
                    previewTower.transform.position = snapPos;

                    bool canPlace = !gridSystem.IsCellOccupied(x, y);

                    bool pointerDown = Input.GetMouseButtonDown(0) ||
                        (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

                    if (pointerDown && canPlace)
                    {
                        gridSystem.SetOccupied(x, y, true);
                        GameObject placedTower = towerBuilder.BuildTower(towerTypeToPlace, 1, snapPos);

                        var sphere = placedTower.GetComponent<SphereCollider>();
                        if (sphere != null)
                            sphere.enabled = true;

                        Destroy(previewTower);
                        previewTower = null;
                        isPlacing = false;
                    }
                }
            }

            bool pointerUp = Input.GetMouseButtonUp(1) ||
                (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);
            if (pointerUp && previewTower != null)
            {
                Destroy(previewTower);
                previewTower = null;
                isPlacing = false;
            }
        }
       
        else if (!isPlacing)
        {
            bool pointerDown = Input.GetMouseButtonDown(0) ||
                (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

            if (pointerDown)
            {
                Vector3 pointerPos = Input.mousePosition;
                if (Input.touchCount > 0)
                    pointerPos = Input.GetTouch(0).position;

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

        previewTower = towerBuilder.BuildTower(type, 1, Vector3.zero);
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