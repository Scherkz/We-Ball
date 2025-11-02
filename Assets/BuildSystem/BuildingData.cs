using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/BuildingData")]
public class BuildingData : ScriptableObject
{
    public new string name;
    public Sprite sprite;

    public Vector2Int cellCount = Vector2Int.one;
}
