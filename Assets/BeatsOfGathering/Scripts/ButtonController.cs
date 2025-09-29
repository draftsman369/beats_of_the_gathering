using UnityEngine;

public class ButtonController : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    public Sprite buttonDefaultSprite;
    public Sprite buttonPressedSprite;
    public KeyCode activationKey;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(activationKey))
        {
            spriteRenderer.sprite = buttonPressedSprite;
        }
        if (Input.GetKeyUp(activationKey))
        {
            spriteRenderer.sprite = buttonDefaultSprite;
        }
    }
}
