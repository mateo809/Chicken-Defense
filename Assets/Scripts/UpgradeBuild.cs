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

    private BuildingComponent selectedBuilding;

    private void Start()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(UpgradeBuilding);
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
                    if (selectedBuilding != buildingComponent)
                    {
                        selectedBuilding = buildingComponent;

                        if (selectedBuilding.buildingData == null)
                        {
                            if (selectedBuilding.buildingData != null)
                            {
                                Debug.Log($"BuildingData found: {selectedBuilding.buildingData.BuildName}, Cost: {selectedBuilding.buildingData.Cost}, AttackSpeed: {selectedBuilding.buildingData.Attackspeed}");
                            }
                            else
                            {
                                Debug.LogError("BuildingData is null when trying to create instance data.");
                            }
                        }
                        LoadBuildingData(selectedBuilding);
                        infoPanel.SetActive(true);
                    }
                }
            }

        }
    }

    private void LoadBuildingData(BuildingComponent instanceData)
    {
        if (instanceData != null)
        {
            Debug.Log($"Loading data for: {instanceData.BuildName}, Level: {instanceData.Level}, Cost: {instanceData.Cost}");

            buildNameText.text = $"Name: {instanceData.BuildName} (Lvl {instanceData.Level})";
            buildingImage.sprite = instanceData.sprite;
            costText.text = $"Cost: {instanceData.Cost} Gold";
            attackSpeedText.text = $"Attack Speed: {instanceData.Attackspeed:F2}";
            damageText.text = $"Damage: {instanceData.Damage}";
            rangeText.text = $"Range: {instanceData.Range:F2}";
            upgradeButton.interactable = GameManager.instance.Coins >= instanceData.Cost;
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
        if (instanceData.Level == 10)
        {
            return;
        }
 
        if (GameManager.instance.Coins >= instanceData.Cost)
        {
            GameManager.instance.Coins -= instanceData.Cost;
            instanceData.Level++;
            instanceData.Cost *= 2;
            instanceData.Attackspeed *= 0.9f;
            instanceData.Damage += 10;
            instanceData.Range += 0.5f;

            Debug.Log($"Building {selectedBuilding.buildingID} upgraded to level {instanceData.Level}");

            LoadBuildingData(instanceData);
        }
        else
        {
            Debug.Log("Not enough gold to upgrade!");
        }
    }
}

