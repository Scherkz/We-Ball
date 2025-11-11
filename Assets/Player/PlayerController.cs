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

    private Rigidbody2D body;

    private Vector2 aimInput;
    private Vector2 lockedAim;

    private bool isCharging = false;
    private float chargeTimer = 0f;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        GetComponent<Renderer>().material.color = Random.ColorHSV(0, 1, 1, 1, 1, 1);
    }

    public void Aim(InputAction.CallbackContext context)
    {
        if (!isCharging)
        {
            aimInput = -1 * context.ReadValue<Vector2>();
        }
    }

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
}