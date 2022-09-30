using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer outline;

    private Animator anim;

    public bool correct_spot;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void HideOutline()
    {
        anim.Play("outline_disappear");
    }
    public void ShowOutline()
    {
        anim.Play("outline_appear");
    }
    

}
