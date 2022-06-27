using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueBaseClass : MonoBehaviour
{
    public bool finished { get; private set; }

    public IEnumerator WriteText(string input, TextMeshProUGUI text, float delay)
    {
        for (int i = 0; i < input.Length; i++)
        {
            text.text += input[i];
            yield return new WaitForSeconds(delay);
        }

        yield return new WaitUntil(() => Keyboard.current.fKey.isPressed);
        finished = true;
    }
}
