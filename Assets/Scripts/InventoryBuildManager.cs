using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBuildManager : MonoBehaviour
{
    public static InventoryBuildManager Instance;

    [SerializeField] public Transform ImageParentDrag;
    [SerializeField] private RectTransform _panelBuildingParent;
    [SerializeField] private RectTransform _parentToparent;
    private RectTransform BuildingPrefabItemsParent;

    public RectTransform TrashArea;

    public GameObject InventoryButton;
    public GameObject TrashAreaGameObject;
    public GameObject InventoryBuildPanel;

    public List<GameObject> _buildingPrefabs = new List<GameObject>();
    public List<Sprite> _buildingImage = new List<Sprite>();
    [SerializeField] private GameObject _inventoryCasePrefab;

    [SerializeField] private List<GameObject> _invSlots = new List<GameObject>();

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
        for (int i = 0; i < _buildingPrefabs.Count; i++)
        {
            GameObject itemInstance = Instantiate(_inventoryCasePrefab, _panelBuildingParent);
            if (itemInstance.transform.childCount > 0)
            {
                GameObject child = itemInstance.transform.GetChild(0).gameObject;
                DragUIElementBuild dragComponent = child.AddComponent<DragUIElementBuild>();
            }

            itemInstance.GetComponent<Image>().sprite = _buildingImage[i];
            _invSlots.Add(itemInstance);
        }
    }

    public void CreateObjectOnMap(GameObject prefab, Vector3 position)
    {
        GameObject newBuilding = Instantiate(prefab, position, Quaternion.identity);
    }


    public void FadeUIElement(float targetAlpha)
    {
        if (_uiCanvasGroup != null)
        {
            _uiCanvasGroup.alpha = targetAlpha;
        }
    }
}
