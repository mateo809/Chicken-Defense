using UnityEngine;

public class RandomMovementAI : MonoBehaviour
{
    public float moveSpeed = 3f; 
    public float changeDirectionInterval = 2f; 
    public float maxX = 10f; 
    public float maxZ = 10f; 

    private Vector3 targetDirection;
    private float timeSinceLastChange = 0f; 

    void Start()
    {
        SetNewRandomDirection();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            CheckForClick();
        }
        timeSinceLastChange += Time.deltaTime;
        if (timeSinceLastChange >= changeDirectionInterval)
        {
            SetNewRandomDirection();
            timeSinceLastChange = 0f;
        }
        Move();
    }
    void CheckForClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Poule"))
            {
                GameManager.instance.Coins += 100;
                Destroy(gameObject);
                Debug.Log("Objet désactivé !");
            }
        }
    }
    void SetNewRandomDirection()
    {
        targetDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
    }
    void Move()
    {
        transform.Translate(targetDirection * moveSpeed * Time.deltaTime);
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -maxX, maxX);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, -maxZ, maxZ);

        transform.position = clampedPosition;
        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); 
        }
    }
}
