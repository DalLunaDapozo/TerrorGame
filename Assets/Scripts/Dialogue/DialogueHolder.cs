using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DialogueSystem
{
    public class DialogueHolder : MonoBehaviour
    {
        private InputManager inputManager;
        public event System.EventHandler actionButtonPressed;

        private void Awake()
        {
            StartCoroutine(DialogueSequence());
            inputManager = new InputManager();
            inputManager.Keyboard.Action.performed += OnActionButtonPressed;
        }

        private IEnumerator DialogueSequence()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Deactivate();
                transform.GetChild(i).gameObject.SetActive(true);
                yield return new WaitUntil(() => transform.GetChild(i).GetComponent<DialogueLine>().finished);
            }

            gameObject.SetActive(false);
        }

        private void Deactivate()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    
        private void OnActionButtonPressed(InputAction.CallbackContext callbackContext)
        {
            actionButtonPressed?.Invoke(this, System.EventArgs.Empty);
        }
    }
}

