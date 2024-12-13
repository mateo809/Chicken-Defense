using UnityEngine;

public class SelectUiController : MonoBehaviour
{
    [SerializeField] private GameObject _Canvas;

    private void OnMouseEnter()
    {
        _Canvas.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        _Canvas.gameObject.SetActive(false);
    }
}
