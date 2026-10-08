
using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeInput : MonoBehaviour
{
    public void ResetDirection() => CurrentInputDirection = Vector2.right;
    private SnakeControls controls;
    public Vector2 CurrentInputDirection {get;private set;} = Vector2.right;

    private void Awake()
    {
        controls = new SnakeControls();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Move.performed += OnMovePerformed;  
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMovePerformed;
        controls.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        Vector2 chosenDir = Vector2.zero;
        if(Mathf.Abs(input.x)> Mathf.Abs(input.y))
        {
            chosenDir = input.x > 0 ? Vector2.right : Vector2.left;
        }
        else if (Mathf.Abs(input.y) > 0)
        {
            chosenDir = input.y > 0 ? Vector2.up : Vector2.down;
        }


        if(chosenDir != Vector2.zero && SnakeMovement.Instance != null)
        {
            SnakeMovement.Instance.AddDirectionInput(chosenDir);
        }
    }
    
}
