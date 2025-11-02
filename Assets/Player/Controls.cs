using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
public class Controlls : MonoBehaviour
{
    public enum ShootMode
    {
        Instant,
        Charged,
        Timing
    }

    [Header("Input Actions")]
    public InputAction shootAction;
    public InputAction aimAction;

    [Header("Settings")]
    public ShootMode shootMode = ShootMode.Instant;
    public float shootForce = 10f;
    public float arrowLength = 3f;

    [Header("Charged Mode Settings")]
    public float maxChargeTime = 2f;
    public float maxChargeMultiplier = 2f;

    [Header("Timing Mode Settings")]
    public float timingBarWidth = 2f;
    public float selectorSpeed = 2f;
    public float perfectPowerMultiplier = 2f;
    public Color selectorColor = Color.white;

    private Vector2 aimInput;
    private Vector2 lockedAim;

    private Rigidbody2D body;
    private LineRenderer aimLine;
    private LineRenderer timingBar;

    private bool isCharging = false;
    private float chargeTimer = 0f;

    private bool isTiming = false;
    private float selectorPosition = 0f;
    private int selectorDirection = 1;

    private GameObject selectorDot;
    private SpriteRenderer selectorSprite;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        // Aim line
        aimLine = GetComponent<LineRenderer>();
        aimLine.positionCount = 2;
        aimLine.startWidth = 0.1f;
        aimLine.endWidth = 0.05f;
        aimLine.material = new Material(Shader.Find("Sprites/Default"));
        aimLine.startColor = Color.yellow;
        aimLine.endColor = Color.red;

        // Timing bar
        GameObject barObj = new GameObject("TimingBar");
        barObj.transform.SetParent(transform);
        timingBar = barObj.AddComponent<LineRenderer>();
        timingBar.positionCount = 2;
        timingBar.startWidth = 0.08f;
        timingBar.endWidth = 0.08f;
        timingBar.material = new Material(Shader.Find("Sprites/Default"));
        timingBar.textureMode = LineTextureMode.Stretch;
        timingBar.enabled = false;

        // Selector dot
        selectorDot = new GameObject("SelectorDot");
        selectorDot.transform.SetParent(transform);
        selectorSprite = selectorDot.AddComponent<SpriteRenderer>();
        selectorSprite.sprite = GenerateCircleSprite(0.15f, selectorColor);
        selectorDot.SetActive(false);
    }

    private void OnEnable()
    {
        shootAction.Enable();
        aimAction.Enable();
    }

    private void OnDisable()
    {
        shootAction.Disable();
        aimAction.Disable();
    }

    private void Update()
    {
        if (!isTiming && !isCharging)
            aimInput = -1 * aimAction.ReadValue<Vector2>();

        if (aimInput.sqrMagnitude > 0.01f)
            ShowAimArrow(aimInput);
        else
            aimLine.enabled = false;

        switch (shootMode)
        {
            case ShootMode.Instant:
                HandleInstantShoot();
                break;
            case ShootMode.Charged:
                HandleChargedShoot();
                break;
            case ShootMode.Timing:
                HandleTimingShoot();
                break;
        }
    }

    private void HandleInstantShoot()
    {
        if (shootAction.WasPerformedThisFrame())
        {
            body.AddForce(aimInput.normalized * shootForce * aimInput.magnitude, ForceMode2D.Impulse);
        }
    }

    private void HandleChargedShoot()
    {
        if (shootAction.WasPressedThisFrame())
        {
            isCharging = true;
            chargeTimer = 0f;
            
            lockedAim = aimInput;
            if (lockedAim.sqrMagnitude < 0.01f) return;
        }

        if (isCharging)
        {
            chargeTimer += Time.deltaTime;
            chargeTimer = Mathf.Min(chargeTimer, maxChargeTime);
            float chargePercent = chargeTimer / maxChargeTime;
            aimLine.startColor = Color.Lerp(Color.yellow, Color.red, chargePercent);
        }

        if (shootAction.WasReleasedThisFrame() && isCharging)
        {
            float chargePercent = chargeTimer / maxChargeTime;
            float chargeMultiplier = maxChargeMultiplier * chargePercent;
            body.AddForce(lockedAim.normalized * shootForce * chargeMultiplier, ForceMode2D.Impulse);

            isCharging = false;
            chargeTimer = 0f;
            aimLine.startColor = Color.yellow;
        }
    }

    private void HandleTimingShoot()
    {
        if (shootAction.WasPressedThisFrame())
        {
            if (!isTiming)
            {
                lockedAim = aimInput;
                if (lockedAim.sqrMagnitude < 0.01f) return;

                isTiming = true;
                selectorPosition = 0f;
                selectorDirection = 1;

                Vector2 barCenter = body.position + Vector2.up * 0.5f;
                Vector2 left = barCenter + Vector2.left * (timingBarWidth / 2f);
                Vector2 right = barCenter + Vector2.right * (timingBarWidth / 2f);
                Vector2 middle = barCenter;

                timingBar.enabled = true;
                timingBar.positionCount = 3; 
                timingBar.SetPosition(0, left);
                timingBar.SetPosition(1, middle);
                timingBar.SetPosition(2, right);

                
                float alpha = 1.0f;
                Gradient gradient = new Gradient();
                gradient.SetKeys(
                    new GradientColorKey[] {
                    new GradientColorKey(Color.red, 0.0f),
                    new GradientColorKey(Color.green, 0.5f),
                    new GradientColorKey(Color.red, 1.0f)
                    },
                    new GradientAlphaKey[] {
                    new GradientAlphaKey(alpha, 0.0f),
                    new GradientAlphaKey(alpha, 1.0f)
                    }
                );

                timingBar.colorGradient = gradient;
                selectorDot.SetActive(true);
                selectorDot.transform.position = left; 
            }
            else
            {
                float distanceFromCenter = Mathf.Abs(selectorPosition - 0.5f) * 2f;
                float centerProximity = 1f - distanceFromCenter;
                float powerMultiplier = Mathf.Lerp(0.5f, perfectPowerMultiplier, centerProximity);
                body.AddForce(lockedAim.normalized * shootForce * lockedAim.magnitude * powerMultiplier, ForceMode2D.Impulse);

                timingBar.enabled = false;
                selectorDot.SetActive(false);
                isTiming = false;
            }
        }

        if (isTiming)
        {
            selectorPosition += selectorDirection * selectorSpeed * Time.deltaTime;
            if (selectorPosition >= 1f)
            {
                selectorPosition = 1f;
                selectorDirection = -1;
            }
            else if (selectorPosition <= 0f)
            {
                selectorPosition = 0f;
                selectorDirection = 1;
            }

            Vector2 left = timingBar.GetPosition(0);
            Vector2 right = timingBar.GetPosition(2);
            Vector2 pos = Vector2.Lerp(left, right, selectorPosition);
            selectorDot.transform.position = pos;
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

    private Sprite GenerateCircleSprite(float radius, Color color)
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x - size / 2f) / (size / 2f);
                float dy = (y - size / 2f) / (size / 2f);
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                pixels[y * size + x] = dist <= 1f ? color : Color.clear;
            }
        }
        tex.SetPixels(pixels);
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}