using UnityEngine;

public class Boids : MonoBehaviour
{
    public Transform boidsPrefab;

    public int NumberofBoids;

    Boid[] boids;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boids = new Boid[NumberofBoids];

        for (int i = 0; i < NumberofBoids; i++) 
        {
            boids[i] = new Boid { boidTransform = Instantiate(boidsPrefab, transform), velocity = Random.onUnitSphere };
        }
    }

    struct Boid
    {
        public Transform boidTransform;
        public Vector3 velocity;
    }
}
