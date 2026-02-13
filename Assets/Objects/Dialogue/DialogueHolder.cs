using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
//using UnityEditor.Rendering;


//to do: Check testiomony/minigame/dialogue audio, beer bottle shooting minigame, new trailer. ADD SFX TO DOUG DIALOGUE
public class DialogueHolder : MonoBehaviour
{
    //public enum CurrentTheme { Bar, Slums };
    public static DialogueHolder instance { get; private set; } // makes it so we can get this script from anywhere, which is what stops the code from being messy parent getting

    public DialogueRunner DialogueRunnerScript; // uses this to quit out dialogue forcibly

    //text portrait is the small portrait in the dialogue box, NPC portrait is the big one
    public GameObject TextPortrait;
    public GameObject PlayerPortrait;
    public GameObject NPCPortrait;
    public GameObject BackgroundImage;

  

    public YarnSpriteChange CurrentTalker;

    

    //The level theme, can be set with the LoadTheme script for when dialogueholder isn't reset on scene load.
 

   


    public List<CharacterInfo> CharacterInfoList;


    //This is to check if a yarn line has finished
    //public bool LineFinished;
    // Use this for initialization

    //only use this function for interrogation




    [YarnCommand("SwapCurrentTalker")]
    public void SwapCurrentTalkingSprite(bool Leftside)
    {
        if (Leftside)
        {
            CurrentTalker = PlayerPortrait.GetComponent<YarnSpriteChange>();
        }
        else
        {
            CurrentTalker = NPCPortrait.GetComponent<YarnSpriteChange>();
        }
    }

    //For an angry theme, seperate command so we don't have to redo code. Wish yarn would support enums... wait, strings.


    public void PortraitTalkCaller()
    {
        if (CurrentTalker)
        {
            CurrentTalker.MouthSwap();
        }
        else
        {CurrentTalker = PlayerPortrait.GetComponent<YarnSpriteChange>();
            CurrentTalker.MouthSwap();
        }
    }
    public void EnablePortraits()
    {
        if (NPCPortrait)
        {
            NPCPortrait.SetActive(true);
            if (NPCPortrait.GetComponent<YarnSpriteChange>())
            {
                NPCPortrait.GetComponent<YarnSpriteChange>().Mouth.enabled = true;
                NPCPortrait.GetComponent<YarnSpriteChange>().Blink.gameObject.SetActive(true); // also lazy here, since it's affected by an enable/disable image we gotta kill the whole game object
                NPCPortrait.GetComponent<YarnSpriteChange>().ResetNonPortraitItems();
            }
        }
        if (PlayerPortrait)
        {
            PlayerPortrait.SetActive(true);
            if (PlayerPortrait.GetComponent<YarnSpriteChange>())
            {
                PlayerPortrait.GetComponent<YarnSpriteChange>().Mouth.enabled = true;
                PlayerPortrait.GetComponent<YarnSpriteChange>().Blink.gameObject.SetActive(true);
                PlayerPortrait.GetComponent<YarnSpriteChange>().ResetNonPortraitItems(); // lazy so we enable it via the check in here
                //PlayerPortrait58.GetComponent<YarnSpriteChange>().Glasses.enabled = true;
            }
        }
        if (BackgroundImage)
        {
            BackgroundImage.SetActive(true);
        }
    }
    public void KillPortraits()
    {
        if (NPCPortrait)
        {
            NPCPortrait.SetActive(false);
            if (NPCPortrait.GetComponent<YarnSpriteChange>())
            {
                NPCPortrait.GetComponent<YarnSpriteChange>().Mouth.enabled = false;
                NPCPortrait.GetComponent<YarnSpriteChange>().Blink.gameObject.SetActive(false);
                NPCPortrait.GetComponent<YarnSpriteChange>().Glasses.enabled = false;
            }
        }
        if (PlayerPortrait)
        {
            PlayerPortrait.SetActive(false);
            if (PlayerPortrait.GetComponent<YarnSpriteChange>())
            {
                PlayerPortrait.GetComponent<YarnSpriteChange>().Mouth.enabled = false;
                PlayerPortrait.GetComponent<YarnSpriteChange>().Blink.gameObject.SetActive(false);
                PlayerPortrait.GetComponent<YarnSpriteChange>().Glasses.enabled = false;
            }
        }
        if (BackgroundImage)
        {
            BackgroundImage.SetActive(false);
        }
    }

    //set instance, if you don't do this it'll all come crashing down
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        KillPortraits();
        //DontDestroyOnLoad(gameObject);

        //this.gameObject.SetActive(false);
    }


    [YarnCommand("ChangeTypewriterSpeed")]
    public void ChangeTypewriterSpeed(int Speed)
    {
        if (DialogueRunnerScript && DialogueRunnerScript.dialogueViews.Length > 0)
        {
            DialogueRunnerScript.dialogueViews[0].GetComponent<LineView>().typewriterEffectSpeed = Speed;
        }
       
    }





}
