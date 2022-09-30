using System;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    private bool interactPressed = false;
    private bool submitPressed = false;

    private bool testButtonPressed = false;

    public event EventHandler test_button_pressed;

    private static InputManager instance;
    private PlayerInput input;

    public bool canInteract;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Input Manager in the scene.");
        }
        instance = this;

        input = new PlayerInput();
        canInteract = true;
    }

    public static InputManager GetInstance()
    {
        return instance;
    }

    public void InteractButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactPressed = true;
        }
        else if (context.canceled)
        {
            interactPressed = false;
        }
    }

    public void TestButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            testButtonPressed = true;
            test_button_pressed?.Invoke(this, EventArgs.Empty);
        }
        else if (context.canceled)
        {
            testButtonPressed = false;
        }
    }

    public void SubmitPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            submitPressed = true;
        }
        else if (context.canceled)
        {
            submitPressed = false;
        }
    }

    public void DisableInput()
    {
        canInteract = false;
    }
    public void EnableInput()
    {
        canInteract = true;
    }


    public bool GetInteractPressed()
    {
        bool result = interactPressed;
        interactPressed = false;
        return result;
    }

    public bool GetSubmitPressed()
    {
        bool result = submitPressed;
        submitPressed = false;
        return result;
    }

    public void RegisterSubmitPressed()
    {
        submitPressed = false;
    }

}