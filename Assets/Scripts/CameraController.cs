using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float ZoomSpeed;
    public float MinZoom;
    public float MaxZoom;
    public float MinX, MaxX, MinZ, MaxZ;

    private float _maxSpeed = 50f;
    private float _accelerationRate = 20f;
    private float _speed = 30f;
    private float _border = 50.0f;
    private float _accelerationX = 5f;
    private float _accelerationZ = 5f;

    private Camera _cam;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        _cam.transform.position = new Vector3(-9.4f, 39.3f, -50.4f); // Position initiale de la caméra
    }

    void Update()
    {
        MoveCamera();
        HandleZoom();
    }

    private void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (_cam.orthographic)
        {
            _cam.orthographicSize -= scrollInput * ZoomSpeed;
            _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, MinZoom, MaxZoom);
        }
        else
        {
            _cam.fieldOfView -= scrollInput * ZoomSpeed;
            _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView, MinZoom, MaxZoom);
        }
    }

    private void MoveCamera()
    {
        if (_cam != null)
        {
            Vector3 newPosition = _cam.transform.position;

            // Déplacement avec les touches ZQSD (vers la gauche/droite sur l'axe X)
            if (Input.GetKey(KeyCode.Q)) // Déplacement à gauche
            {
                _accelerationX += _accelerationRate * Time.deltaTime;
                _accelerationX = Mathf.Min(_accelerationX, _maxSpeed);
                newPosition.x -= _accelerationX * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.D)) // Déplacement à droite
            {
                _accelerationX += _accelerationRate * Time.deltaTime;
                _accelerationX = Mathf.Min(_accelerationX, _maxSpeed);
                newPosition.x += _accelerationX * Time.deltaTime;
            }
            else
            {
                _accelerationX = 0f; // Arrêter l'accélération horizontale si aucune touche n'est appuyée
            }

            // Déplacement avec les touches ZQSD (vers le haut/bas sur l'axe Z)
            if (Input.GetKey(KeyCode.Z)) // Déplacement vers le haut (avant sur l'axe Z)
            {
                _accelerationZ += _accelerationRate * Time.deltaTime;
                _accelerationZ = Mathf.Min(_accelerationZ, _maxSpeed);
                newPosition.z += _accelerationZ * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.S)) // Déplacement vers le bas (arrière sur l'axe Z)
            {
                _accelerationZ += _accelerationRate * Time.deltaTime;
                _accelerationZ = Mathf.Min(_accelerationZ, _maxSpeed);
                newPosition.z -= _accelerationZ * Time.deltaTime;
            }
            else
            {
                _accelerationZ = 0f; // Arrêter l'accélération verticale si aucune touche n'est appuyée
            }

            // Clamper les limites de la caméra pour éviter de sortir de la zone définie
            // Ici, les limites peuvent être négatives pour permettre le mouvement dans les deux directions
            newPosition.x = Mathf.Clamp(newPosition.x, MinX, MaxX);
            newPosition.z = Mathf.Clamp(newPosition.z, MinZ, MaxZ);

            _cam.transform.position = newPosition;
        }
    }
}
