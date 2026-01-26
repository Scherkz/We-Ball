using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    [SerializeField] private GameObject buildingGhostPrefab;
    [SerializeField] private float radius = 3f;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private AnimationCurve antiBuildingChance;

    private Transform anchor;

    public void SpawnBuildings(BuildingData[] buildings, int buildingSpawnCount, float buildGridOccupationPercentage)
    {
        // center anchor on the screen
        var screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        var worldPos = Camera.main.ScreenToWorldPoint(screenCenter);
        worldPos.z = 0;
        anchor.position = worldPos;

        EnsureEnoughBuildingsGhosts(buildingSpawnCount);

        BuildingData[] realBuildings = System.Array.FindAll(buildings, b => !b.isAntiBuilding);
        BuildingData[] antiBuildings = System.Array.FindAll(buildings, b => b.isAntiBuilding);

        var antiBuildingSelectionCount = 0;
        var angleStep = 2.0f * Mathf.PI / buildingSpawnCount;
        for (var i = 0; i < buildingSpawnCount; i++)
        {
            var buildingGhost = anchor.GetChild(i).GetComponent<BuildingGhost>();

            var angle = i * angleStep;
            var circlePos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
            buildingGhost.transform.localPosition = circlePos;

            var randomBuildingData = GetRandomBuildingData(realBuildings, antiBuildings, buildGridOccupationPercentage, antiBuildingSelectionCount);
            buildingGhost.ShowBuilding(randomBuildingData, true);

            if (randomBuildingData.isAntiBuilding)
                antiBuildingSelectionCount++;
        }
    }

    private BuildingData GetRandomBuildingData(BuildingData[] realBuildings, BuildingData[] antiBuildings, float buildGridOccupationPercentage, int antiBuildingSelectionCount)
    {
#if UNITY_EDITOR
        // fast path when testing a specific building
        if (realBuildings.Length == 1 && antiBuildings.Length == 0)
        {
            return realBuildings[0];
        }

        if (antiBuildings.Length == 1 && realBuildings.Length == 0)
        {
            return antiBuildings[0];
        }
#endif

        var chance = antiBuildingChance.Evaluate(buildGridOccupationPercentage);
        chance *= Mathf.Pow(0.5f, antiBuildingSelectionCount); // each selected anti building halfes the probability of selecting another

        if (Random.value < chance)
        {
            return antiBuildings[Random.Range(0, antiBuildings.Length)];
        }

        return realBuildings[Random.Range(0, realBuildings.Length)];
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