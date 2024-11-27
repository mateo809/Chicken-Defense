using UnityEngine;
using UnityEngine.UI;
public class BuildingComponent : MonoBehaviour
{
    public int buildingID;
    public BuildingScriptableObject buildingData;

    [HideInInspector]
    public BuildingInstanceData instanceData;
}

[System.Serializable]
public class BuildingInstanceData
{
    public string BuildName;
    public int Level;
    public int Cost;
    public float Attackspeed;
    public int Damage;
    public float Range;
    public Sprite sprite;

    public BuildingInstanceData(BuildingScriptableObject buildingData)
    {
        BuildName = buildingData.BuildName;
        Level = buildingData.Level;
        Cost = buildingData.Cost;
        Attackspeed = buildingData.Attackspeed;
        Damage = buildingData.Damage;
        Range = buildingData.Range;
        sprite = buildingData.sprite;
    }
}
