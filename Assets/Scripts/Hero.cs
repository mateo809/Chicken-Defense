using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hero : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int damage = 30;
    private Transform _target;

    [SerializeField] private GameObject _animPlane; 
    [SerializeField] private GameObject _prefabToInstantiate; 
    private GameObject _previewPrefabToInstantiate;
    private Plane _buildPlane;
    private GameObject _rangePreviewSphere;

    private Vector3 originalPointerPosition;

    [SerializeField] private GameObject _ClickablePanel; 
    [SerializeField] private TextMeshProUGUI _cooldownText; 

    [SerializeField] private float _cooldownTime = 15.0f; 
    private float _lastUseTime = -Mathf.Infinity;

    void Start()
    {

        _buildPlane = new Plane(Vector3.up, Vector3.zero);

        if (_ClickablePanel != null) _ClickablePanel.SetActive(false);
        if (_cooldownText != null) _cooldownText.gameObject.SetActive(false);
    }

    private void Update()
    {
        float cooldownRemaining = Mathf.Max(0, _cooldownTime - (Time.time - _lastUseTime));

        if (_cooldownText != null)
        {
            _cooldownText.text = cooldownRemaining > 0
                ? cooldownRemaining.ToString("F1") + "s"
                : "Ready!";
        }

        if (_ClickablePanel != null && cooldownRemaining <= 0)
        {
            _ClickablePanel.SetActive(false);
            if (_cooldownText != null) _cooldownText.gameObject.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData data)
    {
        if (Time.time - _lastUseTime < _cooldownTime)
        {
            if (_ClickablePanel != null) _ClickablePanel.SetActive(true);
            if (_cooldownText != null) _cooldownText.gameObject.SetActive(true);
            Debug.Log("Action is on cooldown!");
            return;
        }

        if (_prefabToInstantiate != null)
        {
            if (_ClickablePanel != null) _ClickablePanel.SetActive(false);
            if (_cooldownText != null) _cooldownText.gameObject.SetActive(false);

            _previewPrefabToInstantiate = Instantiate(_prefabToInstantiate);
            _previewPrefabToInstantiate.name = "Preview " + _prefabToInstantiate.name;

            if (_previewPrefabToInstantiate.TryGetComponent(out Collider collider))
            {
                collider.enabled = false;
            }

            var particleSystem = _previewPrefabToInstantiate.GetComponentInChildren<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Simulate(3.9f, true, true);
                particleSystem.Pause();
            }
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (_previewPrefabToInstantiate != null)
        {
            GameManager.instance._InstructionMissile.gameObject.SetActive(true);
            UpdatePreviewPosition(data);
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (Time.time - _lastUseTime < _cooldownTime)
        {
            Debug.Log("Action is on cooldown!");
            return;
        }
        if (_previewPrefabToInstantiate != null)
        {
            PlaceObject(data);
            Destroy(_previewPrefabToInstantiate);
            GameManager.instance._InstructionMissile.gameObject.SetActive(false);
            _lastUseTime = Time.time;

            if (_ClickablePanel != null) _ClickablePanel.SetActive(true);
            if (_cooldownText != null) _cooldownText.gameObject.SetActive(true);
        }
    }

    private void UpdatePreviewPosition(PointerEventData data)
    {
        Ray ray = Camera.main.ScreenPointToRay(data.position);
        float enter = 0;

        if (_buildPlane.Raycast(ray, out enter))
        {
            Vector3 newPosition = ray.GetPoint(enter);
            _previewPrefabToInstantiate.transform.position = newPosition;
        }
    }

    private void PlaceObject(PointerEventData data)
    {
        Ray ray = Camera.main.ScreenPointToRay(data.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            if (hit.collider)
            {
                Vector3 position = hit.point + new Vector3(0,0.5f,0);
                if (_prefabToInstantiate != null)
                {
                    GameObject placedObject = Instantiate(_prefabToInstantiate, position, Quaternion.identity);
                    SphereCollider sphereCollider = placedObject.GetComponent<SphereCollider>();
                    if (sphereCollider != null)
                    {
                        sphereCollider.isTrigger = true; 
                    }

                    var particleSystem = placedObject.GetComponentInChildren<ParticleSystem>();
                    if (particleSystem != null)
                    {
                        _animPlane.gameObject.SetActive(true);
                        Destroy(_previewPrefabToInstantiate);
                        particleSystem.Play();

                        if (_animPlane != null)
                        {
                            StartCoroutine(DisableAnimPlaneAfterParticles(particleSystem));
                        }
                    }

                    Destroy(placedObject, 4f); 
                }
            }
        }
    }

    private IEnumerator DisableAnimPlaneAfterParticles(ParticleSystem particleSystem)
    {
        if (particleSystem == null) yield break;
        float particleDuration = particleSystem.main.duration + particleSystem.main.startLifetime.constantMax;
        yield return new WaitForSeconds(particleDuration);
        if (_animPlane != null) _animPlane.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
