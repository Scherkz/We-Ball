using UnityEngine;

public class Ramp : Building
{
    protected override void RotateSelf(BuildingData buildingData, Rotation rotation)
    {
        switch (this.rotation)
        {
            default:
            case Rotation.Degree0:
                rotationAnchor.localScale = Vector3.one;
                rotationAnchor.localPosition = Vector3.zero;
                break;
            case Rotation.Degree90:
                rotationAnchor.localScale = new Vector3(-1, 1, 1);
                rotationAnchor.localPosition = new Vector3(buildingData.cellCount.x, 0, 0);
                break;
            case Rotation.Degree180:
                rotationAnchor.localScale = new Vector3(-1, -1, 1);
                rotationAnchor.localPosition = new Vector3(buildingData.cellCount.x, buildingData.cellCount.y, 0);
                break;
            case Rotation.Degree270:
                rotationAnchor.localScale = new Vector3(1, -1, 1);
                rotationAnchor.localPosition = new Vector3(0, buildingData.cellCount.y, 0);
                break;
        }
    }
}
