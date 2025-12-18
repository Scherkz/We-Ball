using UnityEngine;
using UnityEngine.InputSystem;

public class CloneInputForwarder : MonoBehaviour
{
    [SerializeField] private string aimActionName = "Aim";
    [SerializeField] private string shootActionName = "Shoot";
    [SerializeField] private string toggleSpecialShotActionName = "ToggleSpecialShot";

    private PlayerInput playerInput;
    private PlayerController target;

    private InputAction aim;
    private InputAction shoot;
    private InputAction toggleSpecial;

    public void Init(PlayerInput input, PlayerController targetController)
    {
        playerInput = input;
        target = targetController;

        if (playerInput == null || playerInput.actions == null || target == null)
            return;

        aim = playerInput.actions.FindAction(aimActionName, throwIfNotFound: true);
        shoot = playerInput.actions.FindAction(shootActionName, throwIfNotFound: true);
        toggleSpecial = playerInput.actions.FindAction(toggleSpecialShotActionName, throwIfNotFound: false);

        aim.performed += ForwardAim;
        aim.canceled += ForwardAim;

        shoot.started += ForwardShoot;
        shoot.canceled += ForwardShoot;

        if (toggleSpecial != null)
            toggleSpecial.performed += ForwardToggleSpecial;
    }

    private void OnDestroy()
    {
        if (aim != null)
        {
            aim.performed -= ForwardAim;
            aim.canceled -= ForwardAim;
        }

        if (shoot != null)
        {
            shoot.started -= ForwardShoot;
            shoot.canceled -= ForwardShoot;
        }

        if (toggleSpecial != null)
            toggleSpecial.performed -= ForwardToggleSpecial;
    }

    private void ForwardAim(InputAction.CallbackContext context)
    {
        if (target != null)
            target.Aim(context);
    }

    private void ForwardShoot(InputAction.CallbackContext context)
    {
        if (target != null)
            target.Shoot(context);
    }

    private void ForwardToggleSpecial(InputAction.CallbackContext context)
    {
        if (target != null)
            target.ToggleSpecialShot(context);
    }
}