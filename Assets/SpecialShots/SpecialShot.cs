using UnityEngine;

public abstract class SpecialShot : MonoBehaviour
{
    protected GameObject currentSpecializedShotVFX;
    public abstract void Init(PlayerController playerController, Player player, Rigidbody2D body);

    protected void DisableSpecialShotVFX()
    {
        if (currentSpecializedShotVFX == null) return;
        
        currentSpecializedShotVFX.SetActive(false);
    }

    protected void EnableSpecialShotVFX()
    {
        if (currentSpecializedShotVFX == null) return;
        
        currentSpecializedShotVFX.SetActive(true);
    }

    protected void OnDestroy()
    {
        if (currentSpecializedShotVFX == null) return;
        
        Destroy(currentSpecializedShotVFX);
    }
}
