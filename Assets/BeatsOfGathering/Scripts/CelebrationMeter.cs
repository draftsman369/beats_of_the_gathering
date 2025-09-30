using UnityEngine;
using UnityEngine.UI;

public class CelebrationMeter : MonoBehaviour
{
    [Range(0f, 1f)] public float value = 0f;
    public ParticleSystem celebrationVFX;

    [Header("UI (assign one)")]
    public Slider slider;    // optional
    public Image fillImage;  // optional (uses Image.fillAmount)


    void Update()
    {
        ApplyUI();

        if (value >= 1f)
        {
            Debug.Log("🎉 Celebration Meter Full!");
            //Instantiate(celebrationVFX, transform.position, celebrationVFX.transform.rotation);
            GameManager.Instance.celebrationReached = true;
            //value = 0f; // reset after logging
            ApplyUI();
        }else
        {
            GameManager.Instance.celebrationReached = false;
        }
    }

    public void Add(float delta)
    {
        value = Mathf.Clamp01(value + delta);
        ApplyUI();
    }

    private void ApplyUI()
    {
        if (slider) slider.value = value;
        if (fillImage) fillImage.fillAmount = value;
    }


    public void ResetCelebrationMeter()
    {
        value = 0f;
        ApplyUI();
    }

}
