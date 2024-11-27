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
                        LoadBuildingData(selectedBuilding.buildingData);
                        infoPanel.SetActive(true);
                    }
                }
            }
        }
    }

    private void LoadBuildingData(BuildingScriptableObject buildingData)
    {
        if (buildingData != null)
        {
            buildNameText.text = $"Name: {buildingData.BuildName} (Lvl {buildingData.Level})";
            buildingImage.sprite = buildingData.sprite;
            costText.text = $"Cost: {buildingData.Cost} Gold";
            attackSpeedText.text = $"Attack Speed: {buildingData.Attackspeed:F2}";
            damageText.text = $"Damage: {buildingData.Damage}";
            rangeText.text = $"Range: {buildingData.Range:F2}";
            upgradeButton.interactable = GameManager.instance.Coins >= buildingData.Cost;
        }
    }

    private void UpgradeBuilding()
    {
        if (selectedBuilding == null || selectedBuilding.buildingData == null) return;

        BuildingScriptableObject buildingData = selectedBuilding.buildingData;

        if (GameManager.instance.Coins >= buildingData.Cost)
        {
            GameManager.instance.Coins -= buildingData.Cost;

            buildingData.Level++;
            buildingData.Cost += buildingData.Cost * buildingData.Level;
            buildingData.Attackspeed *= 0.9f; 
            buildingData.Damage += 10; 
            buildingData.Range += 0.5f; 

            Debug.Log($"Building {selectedBuilding.buildingID} upgraded to level {buildingData.Level}");

            LoadBuildingData(buildingData);
        }
        else
        {
            Debug.Log("Not enough gold to upgrade!");
        }
    }
}
