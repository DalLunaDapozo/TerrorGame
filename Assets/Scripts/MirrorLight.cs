using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

public class MirrorLight : MonoBehaviour
{
    [SerializeField] private Light2D lighter;

    private Light2D lightMirror;

    private void Awake()
    {
        lightMirror = GetComponent<Light2D>();
    }

    private void Update()
    {
        lightMirror.intensity = lighter.intensity;
    }

}
