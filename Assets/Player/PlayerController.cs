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

    private Rigidbody2D body;
    private LineRenderer aimLine;

    private Vector2 aimInput;
    private Vector2 lockedAim;

    private bool isCharging = false;
    private float chargeTimer = 0f;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        GetComponent<Renderer>().material.color = Random.ColorHSV(0, 1, 1, 1, 1, 1);

        // Aim line
        aimLine = GetComponent<LineRenderer>();
        aimLine.positionCount = 2;
        aimLine.startWidth = 0.1f;
        aimLine.endWidth = 0.05f;
        aimLine.material = new Material(Shader.Find("Sprites/Default"));
        aimLine.startColor = Color.yellow;
        aimLine.endColor = Color.red;
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
            aimLine.enabled = false;
            aimLine.startColor = Color.yellow;
        }
    }

    void Update()
    {
        // Handle visuals
        if (aimInput.sqrMagnitude > 0.01f)
            ShowAimArrow(aimInput);
        else
            aimLine.enabled = false;

        if (isCharging)
        {
            chargeTimer += Time.deltaTime;
            chargeTimer = Mathf.Min(chargeTimer, maxChargeTime);
            float chargePercent = chargeTimer / maxChargeTime;
            aimLine.startColor = Color.Lerp(Color.yellow, Color.red, chargePercent);
        }
    }

    private void ShowAimArrow(Vector2 input)
    {
        aimLine.enabled = true;
        Vector2 dir = input.normalized;
        float scaledLength = arrowLength * input.magnitude;
        Vector2 start = body.position;
        Vector2 end = start + dir * scaledLength;
        aimLine.SetPosition(0, start);
        aimLine.SetPosition(1, end);
    }
}
