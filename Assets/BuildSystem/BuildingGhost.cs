using UnityEngine;

public class BuildingGhost : MonoBehaviour
{
    public BuildingData data;
    public Building currentBuilding;

    [SerializeField] private string sortingLayerName;
    [SerializeField] private int sortingLayerOrder;

    private void Awake()
    {
        if (data != null)
        {
            ShowBuilding(data);
        }
    }

    public void ShowBuilding(BuildingData building, bool centerBuilding = false)
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
        currentBuilding.SetRenderingOrder(SortingLayer.NameToID(sortingLayerName), sortingLayerOrder);
        
        if (centerBuilding)
        {
            currentBuilding.transform.localPosition = new Vector3(data.cellCount.x, data.cellCount.y, 0) * -0.5f;
        }
    }

    public void SetTint(Color tint)
    {
        currentBuilding.SetTint(tint);
    }
    
    // is called via Unity's messaging system
    private void OnBuildingSelectedMessage(PlayerBuildController buildController)
    {
        buildController.SetBuildingData(data);
        gameObject.SetActive(false);
    }
}
