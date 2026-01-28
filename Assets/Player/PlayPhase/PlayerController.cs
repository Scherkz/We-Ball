using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Serializable]
    private class SurfaceSfx
    {
        public PhysicsMaterial2D material;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        public float minHitSpeed = 1.5f;
    }

    public Action OnSwing;
    public Action onSurrenderConfirmed;

    [SerializeField] private float defaultLinearDamping = 0.1f;

    [SerializeField] private float shootForce = 10f;
    [SerializeField] private float maxChargeTime = 1f;

    [SerializeField] private bool invertedControls = true;

    [Header("Audio")]
    [SerializeField] private AudioSource shootSfx;
    [SerializeField] private AudioSource activateSpecialShotSfx;
    [SerializeField] private AudioSource deactivateSpecialShotSfx;
    [SerializeField] private AudioSource surfaceHitAudioSource;
    [SerializeField] private SurfaceSfx[] surfaceSfx;

    [Header("AimArrow")]
    [SerializeField] private Transform aimArrowAnchor;
    [SerializeField] private SpriteRenderer aimArrowFill;
    [SerializeField] private Gradient aimChargeColor;

    [Header("Collisions")]
    [Range(0, 10f)]
    [SerializeField] private float maximalCollisionRange = 7f;

    private int buildingsInsideTrigger = 0;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool debugSurfacePhysics;
#endif

    private Rigidbody2D body;
    private TrailRenderer trailRenderer;
    private GameObject partyHat;

    private Vector2 aimInput;

    private float shootInputStartTime;
    private bool isCharging;
    private float aimArrowMaxFillValue;

    private bool isSpecialShotEnabled = false;
    private bool specialShotAvailable = false;

    public Action HasAvailableSpecialShot;
    public Action<Collision2D> BallEnterCollisionEvent;
    public Action<Collider2D> BallExitBuildingTriggerEvent;
    public Action<bool> OnToggleSpecialShotActivation;
    public Action<bool> OnToggleSpecialShotVFX;

    private bool resetOnStart = true;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        partyHat = transform.Find("PartyHat").gameObject;

        GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1);

        aimArrowMaxFillValue = aimArrowFill.sprite.bounds.size.x;
    }

    private void Start()
    {
        if (resetOnStart)
        {
            ResetSelf();
        }
    }

    public void ResetSelf()
    {
        partyHat.SetActive(false);
        aimArrowAnchor.gameObject.SetActive(false);

        body.angularVelocity = 0;
        body.totalTorque = 0;
        body.linearVelocity = Vector2.zero;
        body.totalForce = Vector2.zero;

        trailRenderer.Clear();
    }

    public void ResetSpecialShotSpecifics()
    {
        this.transform.gameObject.layer = LayerMask.NameToLayer("Player");
        buildingsInsideTrigger = 0;
    }

    public void TogglePartyHat(bool enable)
    {
        partyHat.SetActive(enable);
    }

    public void Aim(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // reset the charge when starting to aim
            shootInputStartTime = Time.realtimeSinceStartup;
        }
        else if (context.canceled)
        {
            CancelShotAndHideArrow();
        }

        var aimDirection = context.ReadValue<Vector2>();
        aimInput = invertedControls ? -aimDirection : aimDirection;
    }

    public void ToggleSpecialShot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!specialShotAvailable) return;

            if (buildingsInsideTrigger > 0 && gameObject.layer == LayerMask.NameToLayer("GhostBall"))
                return;

            isSpecialShotEnabled = !isSpecialShotEnabled;

            var sfx = isSpecialShotEnabled ? activateSpecialShotSfx : deactivateSpecialShotSfx;
            if (sfx)
                sfx.Play();

            OnToggleSpecialShotActivation?.Invoke(isSpecialShotEnabled);
            OnToggleSpecialShotVFX?.Invoke(isSpecialShotEnabled);
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // context.duration always reads 0 therefore we have to do it manually
            shootInputStartTime = Time.realtimeSinceStartup;
            isCharging = true;
        }

        if (context.canceled && isCharging)
        {
            if (aimInput.sqrMagnitude < 0.01f)
            {
                aimArrowAnchor.gameObject.SetActive(false);
                return;
            }

            var inputDuration = Time.realtimeSinceStartup - shootInputStartTime;
            var chargeTime = Mathf.Min(inputDuration, maxChargeTime);
            var chargeAmount = chargeTime / maxChargeTime;
            body.AddForce(chargeAmount * shootForce * aimInput.normalized, ForceMode2D.Impulse);

            if (shootSfx != null)
                shootSfx.Play();

            aimInput = Vector2.zero;
            aimArrowAnchor.gameObject.SetActive(false);
            isCharging = false;

            OnSwing?.Invoke();
        }
    }

    public void Surrender(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onSurrenderConfirmed?.Invoke();
        }
    }

    public void CancelShotAndHideArrow()
    {
        isCharging = false;
        aimInput = Vector2.zero;

        if (aimArrowAnchor != null)
            aimArrowAnchor.gameObject.SetActive(false);
    }

    public void SetColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }

    public void DontResetOnStart(bool value)
    {
        resetOnStart = value;
    }

    private void Update()
    {
        // Handle visuals
        if (aimInput.sqrMagnitude > 0.01f)
            ShowAimArrow(aimInput);
        else
            aimArrowAnchor.gameObject.SetActive(false);
    }

    private void ShowAimArrow(Vector2 input)
    {
        aimArrowAnchor.gameObject.SetActive(true);

        var dir = input.normalized;
        var chargeDuration = Time.realtimeSinceStartup - shootInputStartTime;
        var chargeAmount = isCharging ? chargeDuration / maxChargeTime : 0f;

        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        aimArrowAnchor.rotation = Quaternion.Euler(0, 0, angle);

        aimArrowFill.size = new Vector2(Mathf.Lerp(0, aimArrowMaxFillValue, chargeAmount), aimArrowFill.size.y);
        aimArrowFill.color = aimChargeColor.Evaluate(chargeAmount);
    }

    // Ball collision events are used for special shots
    // All players within a certain range will register a collision in the area
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ApplyFrictionFromSurface(collision.collider);

        // Impact from current player position
        Vector2 impactPosition = transform.position;
        // Get all rigidbodies in a radius
        Collider2D[] overlappingColliders = Physics2D.OverlapCircleAll(impactPosition, maximalCollisionRange);
        foreach (Collider2D overlappingCollider in overlappingColliders)
        {
            // Only affect player rigidbodies
            if (!overlappingCollider.CompareTag("Player")) continue;
            Rigidbody2D ballBody = overlappingCollider.GetComponent<Rigidbody2D>();
            if (ballBody != null)
            {
                PlayerController overlappingPlayerController = overlappingCollider.GetComponent<PlayerController>();
                overlappingPlayerController.BallEnterCollisionEvent?.Invoke(collision);
            }
        }

        PlaySurfaceHitSound(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        body.linearDamping = defaultLinearDamping;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Building"))
        {
            buildingsInsideTrigger++;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.layer != LayerMask.NameToLayer("Building")) return;

        buildingsInsideTrigger--;

        // The ball has to exit every building before firing the exit event so it does not get stuck 
        if (buildingsInsideTrigger <= 0)
        {
            buildingsInsideTrigger = 0;
            BallExitBuildingTriggerEvent?.Invoke(collider);
        }
    }

    private void ApplyFrictionFromSurface(Collider2D collider)
    {
        if (collider != null)
        {
            PhysicsMaterial2D material = collider.sharedMaterial;

            if (material != null)
            {
                body.linearDamping = material.friction;
            }
            else
            {
                body.linearDamping = defaultLinearDamping;
            }

#if UNITY_EDITOR
            if (debugSurfacePhysics)
            {
                if (material != null)
                {
                    Debug.Log("Applying friction from surface: " + collider.sharedMaterial.name);
                }
                else
                {
                    Debug.Log("Applying default friction");
                }
            }
#endif
        }
    }

    public void SetSpecialShotAvailability(bool available)
    {
        specialShotAvailable = available;
    }

    public void DisableSpecialShot()
    {
        this.isSpecialShotEnabled = false;
    }

    public bool IsSpecialShotEnabled()
    {
        return isSpecialShotEnabled;
    }

    public float GetMaximalCollisionRange()
    {
        return maximalCollisionRange;
    }

    private void PlaySurfaceHitSound(Collision2D collision)
    {
        if (surfaceHitAudioSource == null || surfaceSfx == null || surfaceSfx.Length == 0)
            return;

        var mat = collision.collider.sharedMaterial;
        if (mat == null)
            if (collision.rigidbody != null)
                mat = collision.rigidbody.sharedMaterial;

        if (mat == null)
            return;

        var hitSpeed = collision.relativeVelocity.magnitude;

        for (int i = 0; i < surfaceSfx.Length; i++)
        {
            var entry = surfaceSfx[i];
            if (entry == null || entry.material == null || entry.clip == null)
                continue;

            if (entry.material != mat)
                continue;

            if (hitSpeed < entry.minHitSpeed)
                break;

            surfaceHitAudioSource.PlayOneShot(entry.clip, entry.volume);
            break;
        }
    }
}