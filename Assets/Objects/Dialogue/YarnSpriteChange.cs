using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;

public class YarnSpriteChange : MonoBehaviour
{



    //public OnClickTalk SpriteHolder;

    //Talking stuff for sprite lipflaps etc. Currently unused.

    public CharacterInfo CurrentCharacterInfo;
    public int CurrentMouthIndex = 0;
    public int CurrentDefaultMouthIndex = 0;
    public Image Mouth;
    public Image Blink;
    public Image Glasses;
    float MouthswapTimer = 1;

    //have the text box profile have a pixelised effect when swapping
    public bool UsePixelTransition = false;
    //Talking animation, loop through frames. 
    #region TalkAnim
    

    #endregion

    private void Awake()
    {
       // SpriteSwap(0); why does it only work here
       if (CurrentCharacterInfo)
        {
            ResetNonPortraitItems();
        }
    }


    private void FixedUpdate()
    {
        //if (Mouth != null && CurrentCharacterInfo && DialogueHolder.instance.DialogueRunnerScript)
       // {
           // MouthswapTimer += Time.deltaTime;
            //if (MouthswapTimer >= 0.45f)
           // {
           //     MouthSwap();
           //     MouthswapTimer = 0;
           // }
       // }
    }


    public void MouthSwap()
    {
        CurrentMouthIndex++;
        if (CurrentMouthIndex >= CurrentCharacterInfo.MouthSpriteList.Count)
        {
            CurrentMouthIndex = CurrentCharacterInfo.FirstTalkingMouthSprite;
        }
        Mouth.sprite = CurrentCharacterInfo.MouthSpriteList[CurrentMouthIndex];
        //yield return new WaitForSeconds(0.5f);
        
    }
    public void MouthReset()
    {
        
       CurrentMouthIndex = CurrentDefaultMouthIndex;
        if (Mouth)
        {
            Mouth.sprite = CurrentCharacterInfo.MouthSpriteList[CurrentMouthIndex];
        }
        //yield return new WaitForSeconds(0.5f);

    }


    [YarnCommand("SwapDefaultMouthID")]
    public void SwapCurrentTalkingSprite(int MouthID)
    {
        CurrentDefaultMouthIndex = MouthID;
    }




    //Pixelisation lerp for when you change text portraits. looked weird with big portraits though.
    IEnumerator Pixelisation()
    {
        float timeElapsed = 0;
        float PixelValue = 1;
       

        //Debug.Log("Lrp work pls");

        while (timeElapsed < 0.35f)
        {
            PixelValue = Mathf.Lerp(1, 0, timeElapsed / 0.35f);
           //ColourMask = Mathf.Lerp(0, 15, timeElapsed / 1.2f);
            timeElapsed += Time.deltaTime;
           // Debug.Log("Pixel" + PixelValue);
            this.gameObject.GetComponent<Image>().material.SetFloat("_Pixelation", PixelValue);
           // this.gameObject.GetComponent<Image>().material.colo
          yield return null;
        }

  this.gameObject.GetComponent<Image>().material.SetFloat("_Pixelation", 0);
        //valueToLerp = endValue;
    }

    [YarnCommand("ResetBlinkMouth")]
    public void ResetNonPortraitItems()
    {
        Debug.Log("ResetPos");
        Mouth.rectTransform.anchoredPosition = CurrentCharacterInfo.MouthPos;
        Blink.rectTransform.anchoredPosition = CurrentCharacterInfo.BlinkPos;
        Glasses.rectTransform.anchoredPosition = CurrentCharacterInfo.GlassesPos;

        Mouth.rectTransform.sizeDelta = CurrentCharacterInfo.MouthScale;
        Blink.rectTransform.sizeDelta = CurrentCharacterInfo.BlinkScale;
        Glasses.rectTransform.sizeDelta = CurrentCharacterInfo.GlassesScale;

        Blink.sprite = CurrentCharacterInfo.BlinkSprite;
        Mouth.sprite = CurrentCharacterInfo.MouthSpriteList[CurrentDefaultMouthIndex];
        if (CurrentCharacterInfo.GlassesSprite)
        {
            Glasses.enabled = true;
            Glasses.sprite = CurrentCharacterInfo.GlassesSprite;
        }
        else
        {
            Glasses.enabled = false;
        }

    }
    [YarnCommand("CharacterSwap")]
    public void CharacterSwap(int ID)
    {
        CurrentCharacterInfo = DialogueHolder.instance.CharacterInfoList[ID];
        

        ResetNonPortraitItems();
        SpriteSwap(0);
    }
    


            //big command we use for swapping portraits
            [YarnCommand("SpriteSwap")]
    public void SpriteSwap(int SpriteID)
    {
        Debug.Log("Sprite swap");
        if (SpriteID < CurrentCharacterInfo.PoseSpriteList.Count && SpriteID >= 0) // check sprite id is within range
        {
           
            this.gameObject.GetComponent<Image>().sprite = CurrentCharacterInfo.PoseSpriteList[SpriteID];
            this.gameObject.GetComponent<Image>().overrideSprite = CurrentCharacterInfo.PoseSpriteList[SpriteID];
            
                //pixel transition
                if (UsePixelTransition)
                {
                    //Debug.Log("HI");
                    StartCoroutine(Pixelisation());
                }
                
            
            /*
            else
            {
                Debug.Log("Cant find portrait mat" + this.gameObject.GetComponent<Image>().material.name + this.gameObject.name);
            }
            */
           
        }
        else
        {
            Debug.Log("Cant handle sprite"); // warning message
        }

        //gervince stuff
        // Add a copy of SpriteHolder to the list
        

    }

   
  
    

}
