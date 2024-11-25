using UnityEngine;
using UnityEngine.EventSystems;

public class DragUIElementBuild : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject _prefabToInstantiate;
    private GameObject _uiElements;
    private GameObject _inventoryCase;
    private GameObject _previewPrefabToInstantiate;
    private GameObject _rangePreviewSphere;

    public BuildingScriptableObject tourelle;
    private Vector2 _originalPointerPosition;
    private Vector3 _originalPanelPosition;

    private bool _isOverTrash = false;
    private Plane _buildPlane;

    public void Initialize(GameObject prefabToInstantiate, GameObject uiElement, GameObject inventoryCase)
    {
        _prefabToInstantiate = prefabToInstantiate;
        _uiElements = uiElement;
        _inventoryCase = inventoryCase;
    }

    public void SetTourelle(BuildingScriptableObject newTourelle)
    {
        tourelle = newTourelle;
        Debug.Log("Tourelle assignée : " + (tourelle != null ? tourelle.name : "Aucune"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _previewPrefabToInstantiate != null)
        {
            _previewPrefabToInstantiate.transform.Rotate(Vector3.up, 90f);  
        }
    }

    public void OnBeginDrag(PointerEventData data)
    {
        _originalPanelPosition = _uiElements.transform.localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _uiElements.transform as RectTransform, data.position, data.pressEventCamera, out _originalPointerPosition);

        if (_prefabToInstantiate != null)
        {
            _previewPrefabToInstantiate = Instantiate(_prefabToInstantiate);
            _previewPrefabToInstantiate.name = "Preview " + _prefabToInstantiate.name;

            if (_previewPrefabToInstantiate.TryGetComponent(out Collider collider))
            {
                collider.enabled = false;
            }
            CreateRangePreview();
        }

        _buildPlane = new Plane(Vector3.up, Vector3.zero);
        InventoryBuildManager.Instance.FadeUIElement(0f);
    }

    public void OnDrag(PointerEventData data)
    {
        InventoryBuildManager.Instance.TrashAreaGameObject.SetActive(true);
        if (_isOverTrash) return;

        if (_previewPrefabToInstantiate != null)
        {
            UpdatePreviewPosition(data);
            UpdateRangePreview();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryBuildManager.Instance.TrashAreaGameObject.SetActive(false);

        if (_isOverTrash && _previewPrefabToInstantiate != null)
        {
            Destroy(_previewPrefabToInstantiate);
            Destroy(_rangePreviewSphere);
            _previewPrefabToInstantiate = null;
            InventoryBuildManager.Instance.FadeUIElement(1f);
            return;
        }

        if (_previewPrefabToInstantiate != null)
        {
            Destroy(_previewPrefabToInstantiate);
            Destroy(_rangePreviewSphere);
            TryPlaceObject(eventData);
        }
        InventoryBuildManager.Instance.FadeUIElement(1f);
    }

    private void UpdatePreviewPosition(PointerEventData data)
    {
        Ray ray = Camera.main.ScreenPointToRay(data.position);
        if (_buildPlane.Raycast(ray, out float enter))
        {
            _previewPrefabToInstantiate.GetComponent<Tourelle>().enabled = false;
            Vector3 newPosition = ray.GetPoint(enter);
            newPosition.y = 0.5f;
            _previewPrefabToInstantiate.transform.position = newPosition;

            if (_rangePreviewSphere != null)
            {
                _rangePreviewSphere.transform.position = newPosition;
            }
            RaycastHit hit;
            bool isValidPlacement = false;

            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                if (hit.collider.CompareTag("Floor"))
                {
                    isValidPlacement = true; 
                }
                if (hit.collider.CompareTag("Map"))
                {
                    isValidPlacement = false;
                }
            }
            Material previewMaterial = GameManager.instance.GetRangePreviewMaterial(isValidPlacement);
            _rangePreviewSphere.GetComponent<Renderer>().material = previewMaterial;
        }
    }


    private void TryPlaceObject(PointerEventData eventData)
    {
        if (tourelle != null && GameManager.instance.Coins >= tourelle.Cost)
        {
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f))
            {
                if (hit.collider.CompareTag("Floor"))
                {
                    Vector3 position = hit.point;
                    Quaternion rotation = _previewPrefabToInstantiate != null
                        ? _previewPrefabToInstantiate.transform.rotation
                        : Quaternion.identity;
                    InventoryBuildManager.Instance.CreateObjectOnMap(_prefabToInstantiate, position, rotation);
                    GameManager.instance.Coins -= tourelle.Cost;
                    Debug.Log("Bâtiment posé, argent restant : " + GameManager.instance.Coins);
                }
                else if(hit.collider.CompareTag("Map"))
                {
                    return;
                }

                if (hit.collider.CompareTag("Build"))
                {
                    Debug.Log("meowwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww");
                }
            }
        }
    }


    private void CreateRangePreview()
    {
        Material rangePreviewMaterial = GameManager.instance.GreenPreview;
        _rangePreviewSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _rangePreviewSphere.name = "Range Preview Sphere";
        _rangePreviewSphere.transform.localScale = new Vector3(tourelle.Range * 2, 0.1f, tourelle.Range * 2);
        _rangePreviewSphere.GetComponent<Collider>().enabled = false;
        if (rangePreviewMaterial != null)
        {
            _rangePreviewSphere.GetComponent<Renderer>().material = rangePreviewMaterial;
        }
        else
        {
            Debug.LogWarning("Range Preview material is not assigned in GameManager.");
        }
    }

    private void UpdateRangePreview()
    {
        if (_rangePreviewSphere != null && _previewPrefabToInstantiate != null)
        {
            _rangePreviewSphere.transform.position = _previewPrefabToInstantiate.transform.position;
        }
    }
}
