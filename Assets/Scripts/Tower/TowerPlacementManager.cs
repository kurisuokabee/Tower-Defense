
using UnityEngine;
using UnityEngine.InputSystem;

public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TowerPlacementArea placementArea;
    

    [Header("Preview")]
    [SerializeField] private Color validColor = new Color(0f, 1f, 0f, 0.5f);
    [SerializeField] private Color invalidColor = new Color(1f, 0f, 0f, 0.5f);

    private GameObject selectedPrefab;
    private GameObject previewObject;

    private TowerRangePreview rangePreview;
    
    private int selectedTowerCost;
    private bool isPlacing;
    private bool canPlace;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    
    private void Update()
    {
        if (!isPlacing)
            return;

        if (Mouse.current == null)
            return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelPlacement();
            return;
        }

        UpdatePreviewPosition();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Left click detected. Can place: " + canPlace);

            bool overUI =
                UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current
                    .IsPointerOverGameObject();

            Debug.Log("Pointer over UI: " + overUI);

            if (!overUI && canPlace)
                PlaceTower();
        }
    }


    public void SelectTower(GameObject prefab, int cost)
    {
        if (prefab == null)
            return;

        if (CurrencyManager.Instance == null ||
            !CurrencyManager.Instance.CanAfford(cost))
            return;

        if (isPlacing)
            CancelPlacement();

        selectedPrefab = prefab;
        selectedTowerCost = cost;

        CreatePreview();
        isPlacing = true;
    }

    private void CreatePreview()
    {
        if (previewObject != null)
            Destroy(previewObject);

        previewObject = Instantiate(selectedPrefab);

        foreach (MonoBehaviour script in
         previewObject.GetComponentsInChildren<MonoBehaviour>())
        {
            if (script is TowerRangePreview)
                continue;

            script.enabled = false;
        }

        rangePreview = previewObject.GetComponent<TowerRangePreview>();

        if (rangePreview != null)
            rangePreview.Show(true);

    }

    

    private void UpdatePreviewPosition()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        mousePosition.z = 0f;
        previewObject.transform.position = mousePosition;

        canPlace = IsValidPosition(mousePosition);

        if (rangePreview != null)
        {
            rangePreview.SetColor(canPlace ? validColor : invalidColor);
        }
    }

    
    
    private bool IsValidPosition(Vector3 position)
    {   
        if (CurrencyManager.Instance == null ||
            !CurrencyManager.Instance.CanAfford(selectedTowerCost))
            return false;

        if (placementArea == null ||
            !placementArea.Contains(position))
            return false;

        TowerPlacementMarker[] towers =
            FindObjectsByType<TowerPlacementMarker>(
                
            );

        float previewRadius = GetTowerRadius();

        foreach (TowerPlacementMarker tower in towers)
        {
            if (tower == null)
                continue;

            // Ignore the tower preview itself.
            if (previewObject != null &&
                tower.transform.IsChildOf(previewObject.transform))
                continue;

            float minimumDistance =
                previewRadius + tower.PlacementRadius;

            float distance = Vector2.Distance(
                position,
                tower.transform.position
            );

            if (distance < minimumDistance)
                return false;
        }

        return true;
    }


    private float GetTowerRadius()
    {
        TowerPlacementMarker marker =
            previewObject.GetComponent<TowerPlacementMarker>();

        return marker != null ? marker.PlacementRadius : 0.75f;
    }


    private void PlaceTower()
    {
        if (!canPlace ||
            CurrencyManager.Instance == null ||
            !CurrencyManager.Instance.CanAfford(selectedTowerCost))
            return;

        if (!CurrencyManager.Instance.SpendGold(selectedTowerCost))
            return;

        Vector3 position = previewObject.transform.position;

        Instantiate(selectedPrefab, position, Quaternion.identity);

        Destroy(previewObject);
        previewObject = null;
        rangePreview = null;
        selectedPrefab = null;
        selectedTowerCost = 0;
        isPlacing = false;
    }

    private void CancelPlacement()
    {
        if (previewObject != null)
            Destroy(previewObject);

        previewObject = null;
        selectedPrefab = null;
        isPlacing = false;
        canPlace = false;
        rangePreview = null;
    }
}
