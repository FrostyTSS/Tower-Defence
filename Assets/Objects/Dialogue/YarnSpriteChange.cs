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

    public List<Sprite> SpriteList;

    //have the text box profile have a pixelised effect when swapping
    public bool UsePixelTransition = false;
    //Talking animation, loop through frames. 
    #region TalkAnim
    

    #endregion

    private void Start()
    {
       // SpriteSwap(0); why does it only work here
       
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




    


            //big command we use for swapping portraits
            [YarnCommand("spriteswap")]
    public void SpriteSwap(int SpriteID)
    {
        Debug.Log("Sprite swap");
        if (SpriteID < SpriteList.Count && SpriteID >= 0) // check sprite id is within range
        {
           
            this.gameObject.GetComponent<Image>().sprite = SpriteList[SpriteID];
            this.gameObject.GetComponent<Image>().overrideSprite = SpriteList[SpriteID];
            
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
