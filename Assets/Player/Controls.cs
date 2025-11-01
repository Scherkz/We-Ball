using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;




[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
public class Controlls : MonoBehaviour
{

    public InputAction shootAction;
       
    public InputAction aimAction;

    private Vector2 aimInput;

    private Rigidbody2D body;
    private LineRenderer lineRenderer;

    public float shootForce = 10f;
    public float arrowLength = 3f;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        lineRenderer = GetComponent<LineRenderer>();



        lineRenderer.positionCount = 2; 
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.red;
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        aimInput = -1 * aimAction.ReadValue<Vector2>();


        if (aimInput.sqrMagnitude > 0.01f)
        {
            ShowAimArrow(aimInput);
        }
        else
        {
            lineRenderer.enabled = false;
        }



        if (shootAction.WasPerformedThisFrame())
        {
            body.AddForce(aimInput.normalized * shootForce * aimInput.magnitude, ForceMode2D.Impulse);
        }
    }


    private void ShowAimArrow(Vector2 input)
    {
        lineRenderer.enabled = true;

        Vector2 direction = input.normalized;
        float scaledLength = arrowLength * input.magnitude;
        Vector2 startPos = body.position;
        Vector2 endPos = startPos + direction * scaledLength;


        lineRenderer.SetPosition(0, new Vector3(startPos.x, startPos.y, 0f));
        lineRenderer.SetPosition(1, new Vector3(endPos.x, endPos.y, 0f));
    }

}
