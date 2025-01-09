using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeBuild : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI buildNameText;
    [SerializeField] private Image buildingImage;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button destroyButton;
    [SerializeField] private GameObject level10IconPrefab;  
    private GameObject level10IconInstance;             


    private BuildingComponent selectedBuilding;

    private void Start()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(UpgradeBuilding);
        if (destroyButton != null)
            destroyButton.onClick.AddListener(DestroyBuilding);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DetectBuildingClick();
        }
    }

    private void DetectBuildingClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.CompareTag("Build"))
            {
                BuildingComponent buildingComponent = hit.collider.GetComponent<BuildingComponent>();
                if (buildingComponent != null)
                {
                    if (selectedBuilding == buildingComponent && infoPanel.activeSelf)
                    {
                        Debug.Log("Same building clicked, panel is already open.");
                        return;
                    }

                    selectedBuilding = buildingComponent;

                    if (selectedBuilding.buildingData == null)
                    {
                        Debug.LogError("BuildingData is null when trying to create instance data.");
                    }
                    else
                    {
                        Debug.Log($"BuildingData found: {selectedBuilding.buildingData.BuildName}, Cost: {selectedBuilding.buildingData.Cost}, AttackSpeed: {selectedBuilding.buildingData.Attackspeed}");
                    }

                    LoadBuildingData(selectedBuilding);
                    infoPanel.SetActive(true);
                }

            }
        }
        else
        {
            infoPanel.SetActive(false);
            selectedBuilding = null;
            ResetUpgradeButton(); 
        }
        if (hit.collider.gameObject.CompareTag("Jeep"))
        {
            GameManager.instance._tutoPanel.SetActive(true);
            GameManager.instance._colonel.gameObject.SetActive(false);
        }
    }

    private void LoadBuildingData(BuildingComponent instanceData)
    {
        if (instanceData != null)
        {
            Debug.Log($"Loading data for: {instanceData.BuildName}, Level: {instanceData.Level}, Cost: {instanceData.Cost}");

            buildNameText.text = $"Name: {instanceData.BuildName} (Lvl {instanceData.Level})";
            buildingImage.sprite = instanceData.sprite;
            costText.text = $"Price: {instanceData.Cost} Gold";
            attackSpeedText.text = $"Attack Speed: {instanceData.Attackspeed:F2}";
            damageText.text = $"Damage: {instanceData.Damage}";
            rangeText.text = $"Range: {instanceData.Range:F2}";
            if (instanceData.Level >= 10)
            {
                upgradeButton.interactable = false;
                upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Max Level";
            }
            else
            {
                upgradeButton.interactable = GameManager.instance.Coins >= instanceData.Cost;
                upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade";
            }
        }
        else
        {
            Debug.LogError("InstanceData is null in LoadBuildingData.");
        }
    }

    private void UpgradeBuilding()
    {
        if (selectedBuilding == null || selectedBuilding.buildingData == null) return;
        BuildingComponent instanceData = selectedBuilding;

        if (instanceData.Level >= 10)
        {
            Debug.Log("Maximum level reached, cannot upgrade further.");
            return;
        }

        if (GameManager.instance.Coins >= instanceData.Cost)
        {
            GameManager.instance.ShowFeedback("-" + instanceData.Cost);
            GameManager.instance.UpdateUI();
            GameManager.instance.Coins -= instanceData.Cost;
            instanceData.Level++;
            instanceData.Cost *= 2;
            instanceData.Attackspeed *= 0.9f;
            instanceData.Damage += 5;
            instanceData.Range += 0.25f;

            Debug.Log($"Building {selectedBuilding.buildingID} upgraded to level {instanceData.Level}");

            LoadBuildingData(instanceData);
        }
        else
        {
            Debug.Log("Not enough gold to upgrade!");
        }
    }

    private void DestroyBuilding()
    {
        if (selectedBuilding == null) return;
        BuildingComponent instanceData = selectedBuilding;
        GameManager.instance.Coins += instanceData.Cost;
        GameManager.instance.ShowSecondFeedback("+" + instanceData.Cost);
        Debug.Log($"Destroying building {selectedBuilding.buildingID}");
        Destroy(selectedBuilding.gameObject);
        selectedBuilding = null;
        infoPanel.SetActive(false);
        ResetUpgradeButton();
        GameManager.instance.UpdateUI();
    }

    private void ResetUpgradeButton()
    {
        if (upgradeButton != null)
        {
            upgradeButton.interactable = false;
            upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade";
        }
    }
}
