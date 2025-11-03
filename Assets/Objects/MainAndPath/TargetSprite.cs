using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TargetSprite : MonoBehaviour
{
    public Sprite GoodTarget;
    public Sprite BadTarget;
     SpriteRenderer TargetSpriteToChange;
    Image TargetImageToChange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (TargetSpriteToChange == null)
        {
            TargetSpriteToChange = this.GetComponent<SpriteRenderer>();
        }
        if (TargetImageToChange == null)
        {
            TargetImageToChange = this.GetComponent<Image>();
        }
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwapTargetIcon(int value)
    {
        if (TargetSpriteToChange)
        {
            switch (value)
            {
                case 0:
                    TargetSpriteToChange.sprite = GoodTarget;
                    break;

                case 1:
                    TargetSpriteToChange.sprite = BadTarget;
                    break;
            }
        }
        else if (TargetImageToChange)
        {
            switch (value)
            {
                case 0:
                    TargetImageToChange.sprite = GoodTarget;
                    break;

                case 1:
                    TargetImageToChange.sprite = BadTarget;
                    break;
            }
        }

    }
}
