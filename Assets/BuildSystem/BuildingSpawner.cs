using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    [SerializeField] private GameObject buildingGhostPrefab;
    [SerializeField] private float radius = 3f;
    [SerializeField] private float rotationSpeed = 30f;

    [SerializeField] private int roundsBeforeAntiBuildings = 1;

    private Transform anchor;

    public void SpawnBuildings(BuildingData[] buildings, int buildingCount, int currentRound)
    {
        // center anchor on the screen
        var screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        var worldPos = Camera.main.ScreenToWorldPoint(screenCenter);
        worldPos.z = 0;
        anchor.position = worldPos;

        EnsureEnoughBuildingsGhosts(buildingCount);

        var angleStep = 2.0f * Mathf.PI / buildingCount;
        for (var i = 0; i < buildingCount; i++)
        {
            var buildingGhost = anchor.GetChild(i).GetComponent<BuildingGhost>();

            var angle = i * angleStep;
            var circlePos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
            buildingGhost.transform.localPosition = circlePos;

            var randomBuildingData = GetRandomBuildingData(buildings, currentRound);
            buildingGhost.ShowBuilding(randomBuildingData, true);
        }
    }

    private BuildingData GetRandomBuildingData(BuildingData[] buildings, int currentRound)
    {
        var nonAntiBuildings = System.Array.FindAll(buildings, b => !b.isAntiBuilding);
        
        if (currentRound <= roundsBeforeAntiBuildings)
        {
            return nonAntiBuildings[Random.Range(0, nonAntiBuildings.Length)];
        }

        return buildings[Random.Range(0, buildings.Length)];
    }

    private void Awake()
    {
        anchor = transform.Find("Anchor");
    }

    private void Update()
    {
        anchor.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        for (int i = 0; i < anchor.childCount; i++)
        {
            anchor.GetChild(i).transform.rotation = Quaternion.identity;
        }
    }

    private void EnsureEnoughBuildingsGhosts(int buildingCount)
    {
        if (anchor.childCount > buildingCount)
        {
            for (var i = anchor.childCount - 1; i >= buildingCount; i--)
            {
                Destroy(anchor.GetChild(i).gameObject);
            }
        }
        else if (anchor.childCount < buildingCount)
        {
            for (var i = anchor.childCount; i < buildingCount; i++)
            {
                Instantiate(buildingGhostPrefab, anchor);
            }
        }

        for (int i = 0; i < anchor.childCount; i++)
        {
            anchor.GetChild(i).gameObject.SetActive(true);
        }
    }
}