using UnityEngine;

public class SkullsManager : MonoBehaviour
{
    public Candle[] candles;
    public Skull[] skulls;

    private void Update()
    {
        for (int i = 0; i < candles.Length; i++)
        {
            if (candles[i].lightIsOn)
            {
                skulls[i].isOn = true;
            }
        }
    }

}
