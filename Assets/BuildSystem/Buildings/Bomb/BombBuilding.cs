using UnityEngine;

public class BombBuilding : Building
{
    [SerializeField] private float destroyTimer;
    [SerializeField] private AudioSource explosionBombSfx;

    public override void Init()
    {
        base.Init();

        if (explosionBombSfx != null)
            explosionBombSfx.Play();

        Destroy(gameObject, destroyTimer);
    }
}
