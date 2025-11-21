using UnityEngine;

public class Windmill : Building
{
    public float rotationSpeed = -15;

    private void FixedUpdate()
    {
        // reuse rotation anchor because we need to set it to allow rotation
        rotationAnchor.Rotate(new Vector3(0, 0, rotationSpeed * Time.fixedDeltaTime));
    }

    public override Rotation GetNextRotation(bool _clockWise)
    {
        return rotation switch
        {
            Rotation.Degree0 => Rotation.Degree180,
            Rotation.Degree180 => Rotation.Degree0,
            _ => Rotation.Degree0,
        };
    }

    protected override void RotateSelf(BuildingData _buildingData, Rotation rotation)
    {
        switch (rotation)
        {
            default:
            case Rotation.Degree0:
                rotationSpeed = -Mathf.Abs(rotationSpeed); // default rotation is to the right
                break;
            case Rotation.Degree180:
                rotationSpeed = Mathf.Abs(rotationSpeed);
                break;
        }
    }
}
