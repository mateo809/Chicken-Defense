using System.Collections.Generic;
using UnityEngine;

public class BoidSystem : MonoBehaviour
{
    public Transform BoidPrefab; // Prefab des boids
    public Transform Attractor; // Transform de l'attracteur
    public Transform SpawnPoint; // Point de spawn pour les boids
    public int NumberOf; // Nombre de boids à générer
    private Dictionary<Vector3, List<Boids>> regions = new(); // Gestion des régions (non utilisée ici)
    [SerializeField] private BoidsSetting Setting; // Réglages des boids
    private Boids[] boids; // Tableau des boids

    private void Start()
    {
        // Initialisation des boids
        boids = new Boids[NumberOf];
        for (int i = 0; i < NumberOf; i++)
        {
            // Instancie les boids avec une rotation de 90 degrés vers la gauche
            Vector3 spawnPosition = SpawnPoint != null ? SpawnPoint.position + Random.insideUnitSphere * 2f : Random.insideUnitSphere * 10f;
            Quaternion initialRotation = Quaternion.Euler(0, -90, 0); // Rotation de 90° vers la gauche (Y axis)
            Transform boidTransform = Instantiate(BoidPrefab, spawnPosition, initialRotation, transform);
            boids[i] = new Boids
            {
                transform = boidTransform,
                Velocity = Random.onUnitSphere, // Vitesse initiale aléatoire
                Attractor = Attractor // Assigner l'attracteur global
            };
        }
    }


    private void Update()
    {
        // Mise à jour des boids
        ComputeNextVelocity(); // Calcul des prochaines vélocités
        ApplyNextVelocity();   // Applique les vélocités aux boids
    }

    void ComputeNextVelocity()
    {
        // Calcul des prochaines vélocités pour chaque boid
        for (int i = 0; i < boids.Length; i++)
        {
            boids[i].ComputeNextVelocity(boids, Setting);
        }
    }

    void ApplyNextVelocity()
    {
        // Applique les prochaines vélocités et met à jour les positions
        for (int i = 0; i < boids.Length; i++)
        {
            boids[i].ApplyNextVelocity(Setting);
        }
    }

    public struct Boids
    {
        public Transform transform; // Transform du boid
        public Transform Attractor; // Référence à l'attracteur
        public Vector3 Velocity; // Vitesse actuelle
        public Vector3 NextVelocity; // Prochaine vélocité calculée

        public void ComputeNextVelocity(Boids[] boids, BoidsSetting setting)
        {
            // Initialisation des forces
            Vector3 alignment = Vector3.zero;
            Vector3 cohesion = Vector3.zero;
            Vector3 avoidance = Vector3.zero;
            Vector3 attraction = Vector3.zero;

            int neighborCount = 0;

            for (int i = 0; i < boids.Length; i++)
            {
                if (boids[i].transform == transform) continue;

                Vector3 direction = boids[i].transform.position - transform.position;
                float distance = direction.magnitude;

                // Si le boid est dans la portée
                if (distance < setting.FarThreshold)
                {
                    neighborCount++;

                    // Alignement : Regarder dans la même direction que les voisins
                    alignment += boids[i].Velocity;

                    // Cohésion : Se rapprocher des voisins
                    cohesion += boids[i].transform.position;

                    // Évitement : S'éloigner des voisins trop proches
                    if (distance < setting.FarThreshold * 0.5f)
                    {
                        avoidance -= direction / distance; // Normalisation implicite
                    }
                }
            }

            if (neighborCount > 0)
            {
                // Moyenne des forces pour alignement et cohésion
                alignment /= neighborCount;
                alignment.Normalize();

                cohesion /= neighborCount;
                cohesion = (cohesion - transform.position).normalized;
            }

            // Attraction vers l'attracteur
            if (Attractor != null)
            {
                Vector3 dirAttractor = Attractor.position - transform.position;
                attraction = dirAttractor.normalized;
            }

            // Calcul de la prochaine vélocité
            NextVelocity =
                alignment * setting.Alignemant +
                cohesion * setting.Cohesion +
                avoidance * setting.Avoidance +
                attraction * setting.Atraction;

            NextVelocity.Normalize(); // Normaliser pour éviter des vitesses trop élevées
        }

        public void ApplyNextVelocity(BoidsSetting setting)
        {
            // Mise à jour de la vélocité avec interpolation pour des mouvements fluides
            Velocity = Vector3.Slerp(Velocity, NextVelocity, setting.TurnRate);
            // Déplacement du boid
            transform.position += Velocity * setting.Speed * Time.deltaTime;
        }
    }

    [System.Serializable]
    public class BoidsSetting
    {
        public float Avoidance = 1.5f;   // Force d'évitement
        public float Cohesion = 1.0f;    // Force de cohésion
        public float Alignemant = 1.0f;  // Force d'alignement
        public float Atraction = 0.5f;  // Force d'attraction vers l'attracteur
        public float FarThreshold = 5.0f; // Portée d'interaction
        public float Speed = 5.0f;       // Vitesse de déplacement
        public float TurnRate = 0.1f;    // Taux de rotation pour des mouvements fluides
    }
}
