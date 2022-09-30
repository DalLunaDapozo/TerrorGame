using UnityEngine;

public class Skull : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public bool isOn;

    private void Update()
    {
        anim.SetBool("isOn", isOn);
    }


}
