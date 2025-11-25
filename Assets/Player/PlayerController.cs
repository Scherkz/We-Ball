using NUnit.Framework.Internal.Commands;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    public float shootForce = 10f;
    public float arrowLength = 3f;

    public float maxChargeTime = 2f;
    public float maxChargeMultiplier = 2f;

    [Header("AimArrow")]
    public Transform aimArrow;
    public float aimArrowMaxLengthMultiplier = 1.5f;
    
    public Action OnSwing;

    private Rigidbody2D body;
    private GameObject partyHat;

    private Vector2 aimInput;
    private Vector2 lockedAim;

    private bool isCharging = false;
    private float chargeTimer = 0f;

    private bool isSpecialShotEnabled = false;
    private SpecialShotType specialShotType= SpecialShotType.PushAway;

    private string playerName="undefined";

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        partyHat = transform.Find("PartyHat").gameObject;

        GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1);

        isSpecialShotEnabled = false;
        specialShotType = SpecialShotType.PushAway;
    }

    private void Start()
    {
        partyHat.SetActive(false);
    }

    public void TogglePartyHat(bool enable)
    {
        partyHat.SetActive(enable);
    }

    public void Aim(InputAction.CallbackContext context)
    {
        if (!isCharging)
        {
            aimInput = -1 * context.ReadValue<Vector2>();
        }
    }
    public void ToggleSpecialShot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isSpecialShotEnabled = !isSpecialShotEnabled;
            Debug.Log($"{playerName} Special Shot enabled toggled to: " + isSpecialShotEnabled);
            Debug.Log($"{playerName} Current Special Shot Type: " + specialShotType.ToString());
        }
    }

    //public void SetSpecialShotEnabled(bool enabled)
    //{
    //    isSpecialShotEnabled = enabled;
    //    Debug.Log($"{playerName} Special Shot explicitly set to: {isSpecialShotEnabled}");
    //}

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isCharging = true;
            chargeTimer = 0f;

            lockedAim = aimInput;
            if (lockedAim.sqrMagnitude < 0.01f) return;
        }

        if (context.canceled && isCharging)
        {
            float chargePercent = chargeTimer / maxChargeTime;
            float chargeMultiplier = maxChargeMultiplier * chargePercent;
            body.AddForce(chargeMultiplier * shootForce * lockedAim.normalized, ForceMode2D.Impulse);

            isCharging = false;
            chargeTimer = 0f;
            aimInput = Vector2.zero;
            aimArrow.gameObject.SetActive(false);
            
            OnSwing?.Invoke();
        }
    }

    void Update()
    {
        // Handle visuals
        if (aimInput.sqrMagnitude > 0.01f)
            ShowAimArrow(aimInput);
        else
            aimArrow.gameObject.SetActive(false);

        if (isCharging)
        {
            chargeTimer += Time.deltaTime;
            chargeTimer = Mathf.Min(chargeTimer, maxChargeTime);
        }
    }

    private void ShowAimArrow(Vector2 input)
    {
        aimArrow.gameObject.SetActive(true);

        Vector2 dir = input.normalized;
        float chargePercent = isCharging ? (chargeTimer / maxChargeTime) : 0f;
        float lengthMultiplier = Mathf.Lerp(1f, aimArrowMaxLengthMultiplier, chargePercent);
        float scaledLength = arrowLength * lengthMultiplier;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        aimArrow.rotation = Quaternion.Euler(0, 0, angle);

        aimArrow.localScale = new Vector3(scaledLength, 1f, 1f);
    }

    // Handle collision for special shots
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"{playerName}: IsSpecialShotEnabled: {isSpecialShotEnabled}");
        if (!isSpecialShotEnabled) return;
        PushAwayImpact(collision);
        
    }

    private void PushAwayImpact(Collision2D collision) {
        Debug.Log("PushAwayImpact triggered");
        Vector2 impactPosition = collision.transform.position;
        // Get all rigidbodies in a radius
        float maximalImpactRange = 20f;
        float maximalImpactForce = 10f;
        Collider2D[] overlappingBallColliders = Physics2D.OverlapCircleAll(impactPosition, maximalImpactRange);
        foreach (Collider2D ballCollider in overlappingBallColliders)
        {
            // Apply force away from impact position
            Rigidbody2D ballBody = ballCollider.GetComponent<Rigidbody2D>();
            if(ballBody!=null && ballBody!=this.body) {
                Vector2 pushDirection = (ballBody.position - impactPosition).normalized;
                float distance = Vector2.Distance(ballBody.position, impactPosition);
                float forceMagnitude = Mathf.Lerp(maximalImpactForce, 0f, distance / maximalImpactRange);
                ballBody.AddForce(pushDirection * forceMagnitude, ForceMode2D.Impulse);
            }
        }
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }
}