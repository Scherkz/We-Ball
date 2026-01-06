using UnityEngine;

public abstract class SpecialShot : MonoBehaviour
{
    protected GameObject currentSpecializedShotVFX;
    public abstract void Init(PlayerController playerController, Player player, Rigidbody2D body);

    protected void ToggleSpecialShotVFX(bool enable)
    {
        if (currentSpecializedShotVFX == null) return;

        currentSpecializedShotVFX.SetActive(enable);
    }

    protected void OnDestroy()
    {
        if (currentSpecializedShotVFX == null) return;

        Destroy(currentSpecializedShotVFX);
    }
}
