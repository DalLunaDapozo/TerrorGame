using UnityEngine;

public class SkullsManager : MonoBehaviour
{
    public Candle[] candles;
    public Skull[] skulls;

    [SerializeField] Ritual ritualCircle;

    public event System.EventHandler OneKeyCandleLighted;

    public bool firstCandleOn;
    public bool thirdCandleOn;
    public bool heartOn;

    public bool allCandlesOn;

    public event System.EventHandler OnHeartEvent;

    [SerializeField] private GameObject heart;

    private void Start()
    {
        heart.SetActive(false);
    }

    private void Update()
    {
        for (int i = 0; i < candles.Length; i++)
        {
            if (candles[i].lightIsOn)
            {
                skulls[i].isOn = true;
            }

            if (skulls[i].isOn == true)
            {
                allCandlesOn = true;
            }
          
        }

        if (skulls[0].isOn && !firstCandleOn)
        {
            OneKeyCandleLighted?.Invoke(this, System.EventArgs.Empty);
            firstCandleOn = true;
        }
       
        if (skulls[2].isOn && !thirdCandleOn)
        {
            OneKeyCandleLighted?.Invoke(this, System.EventArgs.Empty);
            thirdCandleOn = true;
        }


        if (allCandlesOn && !heartOn)
        {
            OnHeartEvent?.Invoke(this, System.EventArgs.Empty);
            heart.SetActive(true);
            heartOn = true;
        }
    }

}
