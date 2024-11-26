using UnityEngine;

public class CursorMouse : MonoBehaviour
{
    [SerializeField] private Texture2D _cursorEnter;
    [SerializeField] private Texture2D _cursorExit;

    void Start()
    {
        Cursor.SetCursor(_cursorEnter, Vector2.zero, CursorMode.ForceSoftware);
    }

    private void OnMouseEnter()
    {
        Cursor.SetCursor(_cursorExit, Vector2.zero, CursorMode.ForceSoftware);

    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(_cursorEnter, Vector2.zero, CursorMode.ForceSoftware);
    }

}
