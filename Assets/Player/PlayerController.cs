using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    public Action OnSwing;

    [SerializeField] private float shootForce = 10f;
    [SerializeField] private float arrowLength = 3f;

    [SerializeField] private float maxChargeTime = 1;
    [SerializeField] private float maxChargeMultiplier = 2f;

    [SerializeField] private bool invertedControls = true;
    
    [Header("AimArrow")] 
    [SerializeField] private Transform aimArrow;
    [SerializeField] private float aimArrowMaxLengthMultiplier = 1.5f;

    private Rigidbody2D body;
    private GameObject partyHat;

    private Vector2 aimInput;

    private bool isCharging = false;
    private float chargeTimer = 0f;

    private bool isSpecialShotEnabled = false;
    private bool specialShotAvailable = false;

    private bool firstShotTakenAfterRoundStart = false;

    public Action GetAssignedSpecialShot;
    public Action HasAvailableSpecialShot;
    public Action<Collision2D> BallCollisionEvent;
    public Action<bool> OnSpecialShotStateChange;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        partyHat = transform.Find("PartyHat").gameObject;

        GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1);
    }

    private void Start()
    {
        ResetSelf();
    }

    public void ResetSelf()
    {
        partyHat.SetActive(false);

        body.angularVelocity = 0;
        body.totalTorque = 0;
        body.linearVelocity = Vector2.zero;
        body.totalForce = Vector2.zero; 
    }

    public void TogglePartyHat(bool enable)
    {
        partyHat.SetActive(enable);
    }

    public void Aim(InputAction.CallbackContext context)
    {
        var aimDirection = context.ReadValue<Vector2>();
        aimInput = invertedControls ? -aimDirection : aimDirection;
     }

    public void ToggleSpecialShot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!specialShotAvailable) return;

            isSpecialShotEnabled = !isSpecialShotEnabled;

            // TODO: Replace with animation or particle effect in the future
            Debug.Log($"{transform.parent.name} Special Shot enabled toggled to: " + isSpecialShotEnabled);
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isCharging = true;
            chargeTimer = 0f;
        }

        if (context.canceled && isCharging)
        {
            if (aimInput.sqrMagnitude < 0.01f)
            {
                isCharging = false;
                chargeTimer = 0f;
                aimArrow.gameObject.SetActive(false);
                return;
            }

            float chargePercent = chargeTimer / maxChargeTime;
            float chargeMultiplier = maxChargeMultiplier * chargePercent;
            body.AddForce(chargeMultiplier * shootForce * aimInput.normalized, ForceMode2D.Impulse);

            isCharging = false;
            chargeTimer = 0f;
            aimInput = Vector2.zero;
            aimArrow.gameObject.SetActive(false);

            OnSwing?.Invoke();
            firstShotTakenAfterRoundStart = true;
        }
    }

    public void CancelShotAndHideArrow()
    {
        isCharging = false;
        chargeTimer = 0f;
        aimInput = Vector2.zero;

        if (aimArrow != null)
            aimArrow.gameObject.SetActive(false);
    }

    public void SetColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }

    private void Update()
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

        var dir = input.normalized;
        var chargePercent = isCharging ? (chargeTimer / maxChargeTime) : 0f;
        var lengthMultiplier = Mathf.Lerp(1f, aimArrowMaxLengthMultiplier, chargePercent);
        var scaledLength = arrowLength * lengthMultiplier;

        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        aimArrow.rotation = Quaternion.Euler(0, 0, angle);

        aimArrow.localScale = new Vector3(scaledLength, 1f, 1f);
    }

    // Ball collision events are used for special shots or powerups
    private void OnCollisionEnter2D(Collision2D collision)
    {
        BallCollisionEvent?.Invoke(collision);
    }

    public void SetSpecialShotAvailability(bool available)
    {
        specialShotAvailable = available;
    }

    public void ResetSpecialShotEnabled()
    {
        this.isSpecialShotEnabled = false;
    }

    public bool IsSpecialShotEnabled()
    {
        return isSpecialShotEnabled;
    }

    public bool IsFirstShotTakenAfterRoundStart()
    {
        return firstShotTakenAfterRoundStart;
    }
}