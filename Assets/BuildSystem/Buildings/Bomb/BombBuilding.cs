using UnityEngine;

public class BombBuilding : Building
{
    [SerializeField] private float destroyTimer;

    public override void Init()
    {
        base.Init();
        Invoke(nameof(DestroySelf), destroyTimer);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
