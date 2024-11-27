using UnityEngine;

public class BuildingComponent : MonoBehaviour
{
    public int buildingID;
    public BuildingScriptableObject buildingData; 

    private void Awake()
    {
        if (buildingData != null)
        {
            buildingData = Instantiate(buildingData);
        }
    }
}
