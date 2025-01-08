using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelNavigator : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private Button leftArrowButton;  
    [SerializeField] private Button rightArrowButton; 
    [SerializeField] private List<GameObject> items;  
    private int currentIndex = 0;                    

    private void Start()
    {
        UpdateNavigationButtons();
        UpdateActiveItem();
    }
    public void OnLeftArrowClicked()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateActiveItem();
            UpdateNavigationButtons();
        }
    }
    public void OnRightArrowClicked()
    {
        if (currentIndex < items.Count - 1)
        {
            currentIndex++;
            UpdateActiveItem();
            UpdateNavigationButtons();
        }
    }
    private void UpdateActiveItem()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].SetActive(i == currentIndex);
        }
    }
    private void UpdateNavigationButtons()
    {
        if (leftArrowButton != null)
        {
            leftArrowButton.interactable = currentIndex > 0;
        }
        if (rightArrowButton != null)
        {
            rightArrowButton.interactable = currentIndex < items.Count - 1;
        }
    }
}
