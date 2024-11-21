using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

public class TooltipScript : MonoBehaviour
{
    public static TooltipScript instance;

    public TextMeshProUGUI tooltipText;
    public GameObject tooltipPanel;
    public Dictionary<RectTransform, GameObject> objectsToTooltip = new();
    [SerializeField] private List<RectTransform> _objectToAddToDictionary = new();
    public List<string> infoTexts;
    public Vector2 fixedPosition = new Vector2(100, 250);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        foreach (var objectToAdd in _objectToAddToDictionary)
        {
            objectsToTooltip.Add(objectToAdd, objectToAdd.gameObject);
        }
    }
    private void Start()
    {
        tooltipPanel.SetActive(false);
    }

    private void Update()
    {
        bool isHovering = false;
        foreach (var rectTransform in objectsToTooltip.Keys)
        {
            if (rectTransform.gameObject.activeInHierarchy && IsMouseOverUIElement(rectTransform))
            {
                int index = GetIndexFromDictionaryKey(rectTransform);
                tooltipPanel.SetActive(true);
                tooltipText.text = infoTexts[index];
                tooltipPanel.transform.position = fixedPosition;

                isHovering = true;
                break;
            }
        }
        if (!isHovering)
        {
            tooltipPanel.SetActive(false);
        }
    }

    private bool IsMouseOverUIElement(RectTransform rectTransform)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint);
        return rectTransform.rect.Contains(localPoint);
    }

    private int GetIndexFromDictionaryKey(RectTransform rectTransform)
    {
        int index = 0;
        foreach (var key in objectsToTooltip.Keys)
        {
            if (key == rectTransform)
                return index;
            index++;
        }
        return -1; // Si la clé n'existe pas
    }
}
