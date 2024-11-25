using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBuildManager : MonoBehaviour
{
    public static InventoryBuildManager Instance;

    [SerializeField] private RectTransform _panelBuildingParent;
    public RectTransform TrashArea;

    public GameObject TrashAreaGameObject;
    public GameObject InventoryBuildPanel;

    [SerializeField] private List<BuildingScriptableObject> _buildingPrefabs = new List<BuildingScriptableObject>();
    [SerializeField] private GameObject _inventoryCasePrefab; 

    private List<GameObject> _invSlots = new List<GameObject>();
    private CanvasGroup _uiCanvasGroup;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        InitializeBuildInventory();

        if (_uiCanvasGroup == null)
        {
            _uiCanvasGroup = InventoryBuildPanel.gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void InitializeBuildInventory()
    {
        foreach (var buildingData in _buildingPrefabs)
        {
            GameObject itemInstance = Instantiate(_inventoryCasePrefab, _panelBuildingParent);

            Image itemImage = itemInstance.GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = buildingData.sprite;
            }
            if (itemInstance.transform.childCount > 0)
            {
                GameObject child = itemInstance.transform.GetChild(0).gameObject;
                DragUIElementBuild dragComponent = child.AddComponent<DragUIElementBuild>();
                dragComponent.Initialize(buildingData.BuildPrefab, itemInstance, _panelBuildingParent.gameObject);
                dragComponent.SetTourelle(buildingData);
            }

            _invSlots.Add(itemInstance);
        }
    }

    public void CreateObjectOnMap(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject _previewObject = Instantiate(prefab, position, rotation);
        _previewObject.GetComponent<BoxCollider>().enabled = true;
        if (_previewObject.TryGetComponent(out Tourelle tourelle))
        {
            tourelle.enabled = true;
        }
    }


    public void FadeUIElement(float targetAlpha)
    {
        if (_uiCanvasGroup != null)
        {
            _uiCanvasGroup.alpha = targetAlpha;
        }
    }
}
