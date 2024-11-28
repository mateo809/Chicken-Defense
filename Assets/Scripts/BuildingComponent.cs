using UnityEngine;
public class BuildingComponent : MonoBehaviour
{
    public int buildingID;
    public BuildingScriptableObject buildingData;

    public string BuildName;
    public int Level;
    public int Cost;
    public float Attackspeed;
    public int Damage;
    public float Range;
    public Sprite sprite;

    public void InitializeStats(BuildingScriptableObject data)
    {
        BuildName = data.BuildName;
        Level = 1;
        Cost = data.Cost;
        Attackspeed = data.Attackspeed;
        Damage = data.Damage;
        Range = data.Range;
        sprite = data.sprite;

        Debug.Log($"BuildingInstanceData created: {BuildName}, Cost: {Cost}, AttackSpeed: {Attackspeed}, Damage: {Damage}, Range: {Range}");
    }
}

