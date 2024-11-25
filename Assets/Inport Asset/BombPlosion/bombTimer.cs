using System.Collections;
using UnityEngine;

public class BombTimer : MonoBehaviour
{
    [SerializeField] private ParticleSystem SparksVfx;
    [SerializeField] private Transform detonationCord;

    [SerializeField] private GameObject Xplosion;

    [SerializeField] private float timer = 3f;

    [SerializeField] private bool PlayOnStart = true;
    private bool isDetonating = false;

    private void Start()
    {
        if (PlayOnStart) isDetonating = true;
    }

    public void Detonate()
    {
        isDetonating = true;
    }

    private void Update()
    {
        if (isDetonating)
        {
            if (!SparksVfx.isPlaying) SparksVfx.Play();

            detonationCord.position = Vector3.Lerp(detonationCord.position, detonationCord.position - new Vector3(0, 0.2f, 0), 0.1f * Time.deltaTime);
            Invoke(nameof(Explode), timer);
        }
    }

    private void Explode()
    {
        SparksVfx.Stop();
        isDetonating = false;
        if (Xplosion != null)
        {
            GameObject explosionInstance = Instantiate(Xplosion, transform.position, Quaternion.identity);
            Destroy(explosionInstance, 2f);
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Map") || collision.gameObject.CompareTag("Floor"))
        {
            Explode();
        }
    }
}
