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
            if (hit.collider.CompareTag("Build"))
            {
                BuildingComponent buildingComponent = hit.collider.GetComponent<BuildingComponent>();
                if (buildingComponent != null)
                {
                    if (selectedBuilding != buildingComponent)
                    {
                        selectedBuilding = buildingComponent;
                        if (selectedBuilding.instanceData == null)
                        {
                            selectedBuilding.instanceData = new BuildingInstanceData(selectedBuilding.buildingData);
                        }

                        LoadBuildingData(selectedBuilding.instanceData);
                        infoPanel.SetActive(true);
                    }
                }
            }
        }
    }

    private void LoadBuildingData(BuildingInstanceData instanceData)
    {
        if (instanceData != null)
        {
            buildNameText.text = $"Name: {instanceData.BuildName} (Lvl {instanceData.Level})";
            buildingImage.sprite = instanceData.sprite;
            costText.text = $"Cost: {instanceData.Cost} Gold";
            attackSpeedText.text = $"Attack Speed: {instanceData.Attackspeed:F2}";
            damageText.text = $"Damage: {instanceData.Damage}";
            rangeText.text = $"Range: {instanceData.Range:F2}";
            upgradeButton.interactable = GameManager.instance.Coins >= instanceData.Cost;
        }
    }

    private void UpgradeBuilding()
    {
        if (selectedBuilding == null || selectedBuilding.instanceData == null) return;

        BuildingInstanceData instanceData = selectedBuilding.instanceData;

        if (GameManager.instance.Coins >= instanceData.Cost)
        {
            GameManager.instance.Coins -= instanceData.Cost;

            instanceData.Level++;
            instanceData.Cost += instanceData.Cost * instanceData.Level;
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

