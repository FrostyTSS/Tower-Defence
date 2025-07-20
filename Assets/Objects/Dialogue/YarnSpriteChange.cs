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

    public float DelayBetweenFrames = 0.3f;
    public int StartAnimRange = 0;
    public int EndAnimRange = 0;
    public int ResultStartAnimRange = 0;
    public int ResultEndAnimRange = 0;
    public float ResultDelayBetweenFrames = 0;
    int CurrentAnimFrame = 0;
    public bool UseTalkAnim = false;
    Coroutine CurrentAnimCoroutine;

    //have the text box profile have a pixelised effect when swapping
    public bool UsePixelTransition = false;
    //Talking animation, loop through frames. 
    #region TalkAnim
    public void TalkAnimStart()
    {
        // Debug.Log("Anim start");
        if (UseTalkAnim)
        {
            StopAllCoroutines();
            CurrentAnimFrame = StartAnimRange;
            //StopCoroutine(CurrentAnimCoroutine);
            CurrentAnimCoroutine = StartCoroutine(TalkAnim());
        }
       
    }

    public void TalkAnimEnd()
    {
        // Debug.Log("Lol");
        if (UseTalkAnim)
        {
            StopCoroutine(CurrentAnimCoroutine);
            StopCoroutine(CurrentAnimCoroutine);
            this.gameObject.GetComponent<Image>().overrideSprite = DialogueHolder.instance.SpriteHolder.SpritePortaitList[ResultStartAnimRange];
            CurrentAnimCoroutine = StartCoroutine(ResultTalkAnim());
        }
    }

    public IEnumerator TalkAnim()
    {
        //Debug.Log(CurrentAnimFrame);
     
        this.gameObject.GetComponent<Image>().overrideSprite = DialogueHolder.instance.SpriteHolder.SpritePortaitList[CurrentAnimFrame];
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(DelayBetweenFrames);
        CurrentAnimFrame++;
        if (CurrentAnimFrame > EndAnimRange)
        {
            CurrentAnimFrame = StartAnimRange;
        }
       
       
     CurrentAnimCoroutine =  StartCoroutine(TalkAnim());



    }


    public IEnumerator ResultTalkAnim()
    {
        //Debug.Log(CurrentAnimFrame);

        this.gameObject.GetComponent<Image>().overrideSprite = DialogueHolder.instance.SpriteHolder.SpritePortaitList[CurrentAnimFrame];
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(ResultDelayBetweenFrames - 0.12f);
        CurrentAnimFrame++;
        if (CurrentAnimFrame > ResultEndAnimRange)
        {
            CurrentAnimFrame = ResultStartAnimRange;
        }


        CurrentAnimCoroutine = StartCoroutine(ResultTalkAnim());



    }

    [YarnCommand("enabletalking")]
    public void EnableTalking(bool value)
    {
        UseTalkAnim = value;
        // Debug.Log("Talk value is " + UseTalkAnim);


    }

    #endregion

    private void Start()
    {
       // SpriteSwap(0); why does it only work here
       if (this.gameObject.GetComponent<OnClickTalk>()) // only for interrgation
        {
            DialogueHolder.instance.SpriteHolder = this.gameObject.GetComponent<OnClickTalk>();
        }
    }












    //Set properties of the shader of the portraits. Changes what it does based on the material name which shouldn't change for the text portrait.
    #region PortraitProperties

    [YarnCommand("setportraitscale")]
    public void SetPortraitScale(float scale)
    {
        
        // Debug.Log(this.gameObject.GetComponent<Image>().sprite);
        //  Debug.Log(this.gameObject.name);


    }


    [YarnCommand("setportraittiling")]
    public void SetPortraitTiling(float x, float y, float z, float w)
    {
       
        // Debug.Log(this.gameObject.GetComponent<Image>().sprite);
        //  Debug.Log(this.gameObject.name);


    }



    [YarnCommand("setportraittilingw")]
    public void SetPortraitTilingW(float w)
    {
        
        // Debug.Log(this.gameObject.GetComponent<Image>().sprite);
        //  Debug.Log(this.gameObject.name);


    }



    #endregion


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




    [YarnCommand("colourspriteswap")]
    public void ColourSpriteSwap(float r, float g, float b)
    {
        Debug.Log("ColourSwap");
        if (DialogueHolder.instance.SpriteHolder) // check sprite id is within range
        {

            this.gameObject.GetComponent<Image>().color = new Color(r, g, b);



        }
    }


            //big command we use for swapping portraits
            [YarnCommand("spriteswap")]
    public void SpriteSwap(int SpriteID)
    {
        Debug.Log("Sprite swap");
        if (DialogueHolder.instance.SpriteHolder && SpriteID < DialogueHolder.instance.SpriteHolder.SpritePortaitList.Count && SpriteID >= 0) // check sprite id is within range
        {
           
            this.gameObject.GetComponent<Image>().sprite = DialogueHolder.instance.SpriteHolder.SpritePortaitList[SpriteID];
            this.gameObject.GetComponent<Image>().overrideSprite = DialogueHolder.instance.SpriteHolder.SpritePortaitList[SpriteID];
            if (this.gameObject.GetComponent<Image>().material.name == "Portrait")
            {

              //if a portrait text sprite thing, gotta set the material texture too
                //its one of these lol
                this.gameObject.GetComponent<Image>().material.SetTexture("Character", DialogueHolder.instance.SpriteHolder.SpritePortaitList[SpriteID].texture);
                this.gameObject.GetComponent<Image>().material.SetTexture("_Character", DialogueHolder.instance.SpriteHolder.SpritePortaitList[SpriteID].texture);



                //pixel transition
                if (UsePixelTransition)
                {
                    //Debug.Log("HI");
                    StartCoroutine(Pixelisation());
                }
                
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

   
    //talking animation, just cycle through frames to "talk" currently unused though.
    [YarnCommand("setanimrange")]
    public void SpriteSwapRange(int startrange, int endrange, float delay, int resultstartrange, int resultendrange, float resultdelay)
    {
      //  Debug.Log(SpriteHolder.SpritePortaitList.Count + "for count");
        if (DialogueHolder.instance.SpriteHolder.SpritePortaitList.Count > 0 && endrange < DialogueHolder.instance.SpriteHolder.SpritePortaitList.Count && resultendrange < DialogueHolder.instance.SpriteHolder.SpritePortaitList.Count)
        {
            StartAnimRange = startrange;
            EndAnimRange = endrange;
            DelayBetweenFrames = delay;

            ResultStartAnimRange = resultstartrange;
            ResultEndAnimRange = resultendrange;
            ResultDelayBetweenFrames = resultdelay;
        }
        else
        {
            Debug.Log("Either out of range or dm taylor and call him an idiot");
        }
       


    }


    //play animation, currently unused
    [YarnCommand("playanimator")]
    public void PlayAnim(string animname)
    {
        //Debug.Log("Test later");
        if (this.gameObject.GetComponent<Animator>())
        {
            this.gameObject.GetComponent<Animator>().Play("Base Layer" + animname, 0, 0.0f);
        }
        else
        {
            Debug.Log("oops");
        }


    }


   


    

}
