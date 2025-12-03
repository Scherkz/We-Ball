using UnityEngine;

public abstract class SpecialShot : MonoBehaviour
{
    protected GameObject currentSpecializedShotVFX;
    public abstract void Init(PlayerController playerController, Player player, Rigidbody2D body);

    protected void DisableSpecialShotVFX()
    {
        currentSpecializedShotVFX.SetActive(false);
    }

    protected void EnableSpecialShotVFX()
    {
        currentSpecializedShotVFX.SetActive(true);
    }
}
