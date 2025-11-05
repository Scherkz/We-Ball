using UnityEngine;

public class BuildingGhost : MonoBehaviour
{
    public BuildingData data;

    private Building currentBuilding;

    private void Awake()
    {
        if (data != null)
        {
            ShowBuilding(data);
        }
    }

    public void ShowBuilding(BuildingData building)
    {
        if (currentBuilding != null)
        {
            Destroy(currentBuilding.gameObject);
            currentBuilding = null;
        }

        data = building;

        currentBuilding = Instantiate(data.prefab).GetComponent<Building>();

        currentBuilding.transform.SetParent(transform, false);
        currentBuilding.name = data.name;
    }

    public void SetTint(Color tint)
    {
        currentBuilding.SetTint(tint);
    }
}
