using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Hero : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int damage = 30;

    private Transform target;

    [SerializeField] private GameObject _animPlane;

    public GameObject prefabToInstantiate; 
    private GameObject previewPrefabToInstantiate; 
    private Plane buildPlane;
    private GameObject _rangePreviewSphere;

    private Vector3 originalPointerPosition; 

    void Start()
    {
        buildPlane = new Plane(Vector3.up, Vector3.zero); 
    }

    public void OnBeginDrag(PointerEventData data)
    {

        if (prefabToInstantiate != null)
        {
            previewPrefabToInstantiate = Instantiate(prefabToInstantiate);
            previewPrefabToInstantiate.name = "Preview " + prefabToInstantiate.name;

            if (previewPrefabToInstantiate.TryGetComponent(out Collider collider))
            {
                collider.enabled = false; 
            }

            var particleSystem = previewPrefabToInstantiate.GetComponentInChildren<ParticleSystem>();
            if (particleSystem != null)
            {
                // Ajuster la durée du Particle System
                var mainModule = particleSystem.main;
                // Simuler jusqu'à une progression précise (par exemple, la moitié de sa durée)
                particleSystem.Simulate(3.9f, true, true); // Simule 0.25 secondes
                particleSystem.Pause(); // Mettre en pause pour geler l'animation
            }
        }
    }
    public void OnDrag(PointerEventData data)
    {
        if (previewPrefabToInstantiate != null)
        {
            UpdatePreviewPosition(data);
        }
    }
    public void OnEndDrag(PointerEventData data)
    {
        if (previewPrefabToInstantiate != null)
        {
            PlaceObject(data); 
            Destroy(previewPrefabToInstantiate); 
        }
    }
    private void UpdatePreviewPosition(PointerEventData data)
    {
        Ray ray = Camera.main.ScreenPointToRay(data.position);
        float enter = 0;

        if (buildPlane.Raycast(ray, out enter)) 
        {
            Vector3 newPosition = ray.GetPoint(enter); 
            previewPrefabToInstantiate.transform.position = newPosition;
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
                Vector3 position = hit.point;
                GameObject placedObject = Instantiate(prefabToInstantiate, position, Quaternion.identity);

                // Trouver le Particle System
                var particleSystem = placedObject.GetComponentInChildren<ParticleSystem>();
                if (particleSystem != null)
                {
                    Destroy(previewPrefabToInstantiate); // Supprimer la prévisualisation
                    particleSystem.Play(); // Jouer le Particle System
                    _animPlane.gameObject.SetActive(true); // Activer l'animation du Plane

                    // Démarrer une coroutine pour désactiver _animPlane après la durée des particules
                    StartCoroutine(DisableAnimPlaneAfterParticles(particleSystem));
                }

                Destroy(placedObject, 4f); // Détruire l'objet après 4 secondes (ou ajustez cette valeur)
            }
        }
    }

    private IEnumerator DisableAnimPlaneAfterParticles(ParticleSystem particleSystem)
    {
        if (particleSystem == null) yield break;

        // Obtenez la durée totale des particules (inclut la durée et le temps de vie)
        float particleDuration = particleSystem.main.duration + particleSystem.main.startLifetime.constantMax;

        // Attendre la fin de l'animation
        yield return new WaitForSeconds(particleDuration);

        // Désactiver _animPlane
        _animPlane.gameObject.SetActive(false);
    }


    private void CreateRangePreview()
    {
        Material rangePreviewMaterial = GameManager.instance.GreenPreview;
        _rangePreviewSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _rangePreviewSphere.name = "Range Preview Sphere";
        _rangePreviewSphere.transform.localScale = new Vector3(15, 0.1f, 15);
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

    //void HitTarget()
    //{
    //    Enemy enemy = target.GetComponent<Enemy>();
    //    if (enemy != null)
    //    {
    //        enemy.TakeDamage(damage);
    //    }

    //    Destroy(gameObject);
    //}
}
