using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DragUIElementBuild : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private GameObject _prefabToInstantiate;
    private GameObject _uiElements;
    private GameObject _inventoryCase;
    private GameObject _previewPrefabToInstantiate;

    private Vector2 _originalPointerPosition;
    private Vector3 _originalPanelPosition;

    private bool _isOverTrash = false;


    public void Initialize(GameObject prefabToInstantiate, GameObject uiElement, GameObject inventoryCase)
    {
        _prefabToInstantiate = prefabToInstantiate;
        _uiElements = uiElement;
        _inventoryCase = inventoryCase;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _uiElements.transform as RectTransform, data.position, data.pressEventCamera, out _originalPointerPosition);

        if (_prefabToInstantiate != null)
        {
            _previewPrefabToInstantiate = Instantiate(_prefabToInstantiate);
            _previewPrefabToInstantiate.name = "Preview " + _prefabToInstantiate.name;
        }
    }
    public void OnDrag(PointerEventData data)
    {
        InventoryBuildManager.Instance.TrashAreaGameObject.SetActive(true);
        if (_isOverTrash) return;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _uiElements.transform as RectTransform, data.position, data.pressEventCamera, out Vector2 localPointerPosition))
        {
            Vector3 offset = localPointerPosition - _originalPointerPosition;

            if (_previewPrefabToInstantiate != null)
            {
                UpdatePreviewPosition(data);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryBuildManager.Instance.TrashAreaGameObject.SetActive(false);
        if (_isOverTrash && _previewPrefabToInstantiate != null)
        {
            Destroy(_previewPrefabToInstantiate);
            _previewPrefabToInstantiate = null;
            InventoryBuildManager.Instance.FadeUIElement(1f);
            return;
        }

        if (_previewPrefabToInstantiate != null)
        {
            Destroy(_previewPrefabToInstantiate);
            TryPlaceObject(eventData);
        }
        InventoryBuildManager.Instance.FadeUIElement(1f);
    }

    private void UpdatePreviewPosition(PointerEventData data)
    {
        Ray ray = Camera.main.ScreenPointToRay(data.position);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f))
        {
            Vector3 snappedPosition = SnapToGrid(hit.point);
            _previewPrefabToInstantiate.transform.position = snappedPosition;
        }
    }

    private Vector3 SnapToGrid(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x);
        int z = Mathf.RoundToInt(position.z);
        return new Vector3(x, 1, z);
    }

    private void TryPlaceObject(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f))
        {
            Vector3 snappedPosition = SnapToGrid(hit.point);
            InventoryBuildManager.Instance.CreateObjectOnMap(_prefabToInstantiate, snappedPosition);
        }
    }

    private bool IsPointerOverTrash(PointerEventData data)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            InventoryBuildManager.Instance.TrashArea, data.position, data.pressEventCamera);
    }
}
