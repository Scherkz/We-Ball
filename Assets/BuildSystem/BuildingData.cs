using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/BuildingData")]
public class BuildingData : ScriptableObject
{
    public new string name;
    public GameObject prefab;

    public bool isAntiBuilding = false;
    public bool isInvisibleBuilding = false;
    public Vector2Int cellCount = Vector2Int.one;

    public bool isOccupationRotationDependent = true;
    
    [HideInInspector][SerializeField] private bool[] bitmask;

    public bool UsesCell(int x, int y)
    {
        var index = x + y * cellCount.x;
        return bitmask[index];
    }

}
