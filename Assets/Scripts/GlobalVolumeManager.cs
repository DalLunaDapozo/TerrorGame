using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class GlobalVolumeManager : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [SerializeField] private VolumeProfile gameVolume;
   
    private MadnessManager madness;

    private float currentTimer;
    public float redScreenRate;

    public string profileName;

    public Color color;

    [SerializeField] ColorAdjustments colorAdjustments;

    public int interpolationFramesCount = 45; // Number of frames to completely interpolate between the 2 positions
    int elapsedFrames = 0;

    private void OnDisable()
    {
        colorAdjustments.colorFilter.Override(Color.white);
    }

    private void Awake()
    {
        try { madness = GameObject.Find("MadnessManager").GetComponent<MadnessManager>(); }
        catch { Debug.Log("Madness not found"); }

        gameVolume = volume.sharedProfile;

        ColorAdjustments cA;
        
        if (gameVolume.TryGet<ColorAdjustments>(out cA))
        {
            colorAdjustments = cA;
        }

        color = Color.white;

    }


    private void Update()
    {
        
        colorAdjustments.colorFilter.Override(color);
       

        if (madness.madnessOn == true && madness.canDie && color.r <= 1.7f)
            color.r += redScreenRate;
        else 
        {
            if (color.r > 1f)
                color.r -= redScreenRate;
        }

        
    }
}