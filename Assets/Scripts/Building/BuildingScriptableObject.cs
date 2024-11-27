using UnityEngine;

[CreateAssetMenu(fileName = "BuildingScriptableObject", menuName = "Scriptable Objects/BuildingScriptableObject")]
public class BuildingScriptableObject : ScriptableObject
{
    public int Level;
    public int Cost;
    public Sprite sprite;
    public string BuildName;
    public float Attackspeed;
    public int Damage;
    public float Range;
    public GameObject BuildPrefab;
}



