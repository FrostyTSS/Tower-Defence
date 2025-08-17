using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Rendering;


//to do: Check testiomony/minigame/dialogue audio, beer bottle shooting minigame, new trailer. ADD SFX TO DOUG DIALOGUE
public class DialogueHolder : MonoBehaviour
{
    //public enum CurrentTheme { Bar, Slums };
    public static DialogueHolder instance { get; private set; } // makes it so we can get this script from anywhere, which is what stops the code from being messy parent getting

    public GameObject DialogueRunner; // uses this to quit out dialogue forcibly

    //text portrait is the small portrait in the dialogue box, NPC portrait is the big one
    public GameObject TextPortrait;
    public GameObject PlayerPortrait;
    public GameObject NPCPortrait;
    public GameObject BackgroundImage;

    //for the red emergency interjects, parts are used for the options now
    public GameObject InterruptPrefab;
    GameObject CurrentInterrupt;
    public Material interjectMaterial;

    //the two text strings yarn creates
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DialogueText;

    

    //The level theme, can be set with the LoadTheme script for when dialogueholder isn't reset on scene load.
 

    public GameObject SkipImage;
    public GameObject DiaLogButton;
    //Angry Material
   
    public GameObject nameInput;
    TMP_InputField inputField;

    //Set the font style for the dialogue text. Doesn't work for names.
    bool UseItalics = false;
    bool UseBold = false;

    public bool loadLevel = false;
    //the original typing speed to revert to later
    float OriginalTypeSpeed = 0;

    public GameObject UIBlockerForName;
    

    //Vertex Gradient code, kinda janky but its there if writers want to make some text extra funky
    int GradientMode = 0;
    Color GradientColour;
    Color GradientColour2;
    Color NameGradientColour;
    Color NameGradientColour2;
    UnityEngine.Color[] originatextcolours;
    UnityEngine.Color[] originanamecolours;

    //for options stuff with interject
    Transform ParentGroup;

    public Color currentColor;
    public Color greyColor;

    //This is to check if a yarn line has finished
    //public bool LineFinished;
    // Use this for initialization

    //only use this function for interrogation
    



  

    //For an angry theme, seperate command so we don't have to redo code. Wish yarn would support enums... wait, strings.

  

   


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

        if (NPCPortrait)
        {
            NPCPortrait.SetActive(false);
        }
        if (PlayerPortrait)
        {
            PlayerPortrait.SetActive(false);
        }
        if (BackgroundImage)
        {
            BackgroundImage.SetActive(false);
        }
        //DontDestroyOnLoad(gameObject);

        //this.gameObject.SetActive(false);
    }

   

   

   

    
}
