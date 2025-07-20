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
    public GameObject NPCPortrait;

    //for the red emergency interjects, parts are used for the options now
    public GameObject InterruptPrefab;
    GameObject CurrentInterrupt;
    public Material interjectMaterial;

    //the two text strings yarn creates
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DialogueText;

    //what we get sprites and which dialogue node to start from.
    public OnClickTalk SpriteHolder;

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
        //DontDestroyOnLoad(gameObject);

        //this.gameObject.SetActive(false);
    }

    private void Start()
    {
        //Rion options stuff
      //  ParentGroup = GameObject.Find("OptionsBackground").transform;
     //   ParentGroup.gameObject.SetActive(false);

       
        //inputField.enabled = false;
        if (loadLevel)
        {
            SetForLevel();

        }


     

    }

    //rion stuff
    public void SetForInterrogation()
    {
        DialogueRunner = FindFirstObjectByType<DialogueRunner>().gameObject;
        NPCPortrait = GameObject.Find("PortraitImage");

        SpriteHolder = FindFirstObjectByType<OnClickTalk>();
    }
    public void SetForLevel()
    {
        DialogueRunner = FindFirstObjectByType<DialogueRunner>().gameObject;
        TextPortrait = GameObject.Find("Portrait_Character");
        NPCPortrait = GameObject.Find("NpcPortrait");

        NameText = GameObject.Find("CharacterNameImage").GetComponentInChildren<TextMeshProUGUI>();
        DialogueText = GameObject.Find("TextBoxContainer").GetComponentInChildren<TextMeshProUGUI>();
    }


    //Change colours for the dialogue text and name text, using RGB values. the gradients use vertex colours and are kinda jank. Maybe condense later.
    #region TextColour

    [YarnCommand("colourname")]
    public void ChangeNameColour(float R, float G, float B)
    {

        NameText.color = new Color(R, G, B);

    }

    [YarnCommand("colournamegradient")]
    public void ChangeNameColourGradient(float R, float G, float B, float R2, float G2, float B2)
    {

        originanamecolours = NameText.mesh.colors;

        NameGradientColour = new Color(R, G, B);
        NameGradientColour2 = new Color(R2, G2, B2);
        if (GradientMode < 3)
        {
            GradientMode += 1;
        }
        GradientApply();



    }

    [YarnCommand("colourtextgradient")]
    public void ChangeTextColourGradient(float R, float G, float B, float R2, float G2, float B2)
    {

        originatextcolours = DialogueText.mesh.colors;

        GradientColour = new Color(R, G, B);
        GradientColour2 = new Color(R2, G2, B2);
        if (GradientMode < 3)
        {
            GradientMode += 2;
        }
        GradientApply();



    }

    [YarnCommand("disablegradient")]
    public void ChangeGradient()
    {
        //reset mesh
        /*
        for (int c = 0; c < DialogueText.text.Length; c++)
        {
            DialogueText.mesh.colors[c] = originatextcolours[c];
        }

        for (int c = 0; c < NameText.text.Length; c++)
        {
            NameText.mesh.colors[c] = originanamecolours[c];
        }
        */
        NameText.ForceMeshUpdate();
        DialogueText.ForceMeshUpdate();
        GradientMode = 0;
        GradientApply();



    }

    public void GradientApply()
    {
        NameText.ForceMeshUpdate();
        DialogueText.ForceMeshUpdate();
        UnityEngine.Color[] textcolors;
        switch (GradientMode)
        {

            case 0:


                break;

            case 1:
                textcolors = NameText.mesh.colors;
                for (int c = 0; c < NameText.text.Length; c++)
                {
                    NameText.ForceMeshUpdate();
                    // UnityEngine.Color[] textcolors = NameText.mesh.colors;

                    textcolors[4 * c] = NameGradientColour;
                    textcolors[4 * c + 1] = NameGradientColour;
                    textcolors[4 * c + 2] = NameGradientColour2;
                    textcolors[4 * c + 3] = NameGradientColour2;
                    //NameText.color = new Color(R, G, B);
                    NameText.mesh.colors = textcolors;
                    NameText.UpdateGeometry(NameText.mesh, 0);
                    Debug.Log("wowie");
                }
                break;



            case 2:
                DialogueText.ForceMeshUpdate();
                // UnityEngine.Color[] textcolors = NameText.mesh.colors;
                textcolors = DialogueText.mesh.colors;
                for (int c = 0; c < DialogueText.text.Length; c++)
                {
                    textcolors[4 * c] = GradientColour;
                    textcolors[4 * c + 1] = GradientColour;
                    textcolors[4 * c + 2] = GradientColour2;
                    textcolors[4 * c + 3] = GradientColour2;
                    //NameText.color = new Color(R, G, B);
                    DialogueText.mesh.colors = textcolors;
                    DialogueText.UpdateGeometry(DialogueText.mesh, 0);
                }
                break;


            case 3:


                textcolors = NameText.mesh.colors;
                for (int c = 0; c < NameText.text.Length; c++)
                {
                    NameText.ForceMeshUpdate();
                    // UnityEngine.Color[] textcolors = NameText.mesh.colors;

                    textcolors[4 * c] = NameGradientColour;
                    textcolors[4 * c + 1] = NameGradientColour;
                    textcolors[4 * c + 2] = NameGradientColour2;
                    textcolors[4 * c + 3] = NameGradientColour2;
                    //NameText.color = new Color(R, G, B);
                    NameText.mesh.colors = textcolors;
                    NameText.UpdateGeometry(NameText.mesh, 0);
                    Debug.Log("wowie");
                }

                DialogueText.ForceMeshUpdate();
                // UnityEngine.Color[] textcolors = NameText.mesh.colors;
                textcolors = DialogueText.mesh.colors;
                for (int c = 0; c < DialogueText.text.Length; c++)
                {
                    textcolors[4 * c] = GradientColour;
                    textcolors[4 * c + 1] = GradientColour;
                    textcolors[4 * c + 2] = GradientColour2;
                    textcolors[4 * c + 3] = GradientColour2;
                    //NameText.color = new Color(R, G, B);
                    DialogueText.mesh.colors = textcolors;
                    DialogueText.UpdateGeometry(DialogueText.mesh, 0);
                }

                break;


        }

    }

    [YarnCommand("colourtext")]
    public void ChangetextColour(float R, float G, float B)
    {

        DialogueText.color = new Color(R, G, B);

    }






    [YarnCommand("colourtextandname")]
    public void ChangeTextAndNameColour(float R, float G, float B)
    {
        NameText.color = new Color(R, G, B);
        DialogueText.color = new Color(R, G, B);

    }
    #endregion

    //rion stuff
    public IEnumerator LoadSceneAfterFade(string scenename)
    {
        yield return new WaitForSeconds(1.5f);
        //save all variables
       
        SceneManager.LoadScene(scenename);
    }

    //load a scene including fade
    [YarnCommand("loadscene")]
    public void LoadScene(string scenename)
    {

       


    }

    //set typing speed, and if the text is bold or italics. 

    #region TextSpeedAndFormat
    [YarnCommand("settextspeed")]
    public void SetTextSpeed(float Speed)
    {
      //  FindFirstObjectByType<LineView>().typewriterEffectSpeed = Speed;

    }


    [YarnCommand("toggleitalics")]
    public void ToggleItalics()
    {
        if (UseItalics)
        {
            DialogueText.fontStyle = FontStyles.Normal;
            UseItalics = false;
        }
        else
        {
            DialogueText.fontStyle = FontStyles.Italic;
            UseItalics = true;
        }

    }



    [YarnCommand("togglebold")]
    public void ToggleBold()
    {
        if (UseBold)
        {
            DialogueText.fontStyle = FontStyles.Normal;
            UseBold = false;
        }
        else
        {
            DialogueText.fontStyle = FontStyles.Bold;
            DialogueText.fontWeight = FontWeight.Heavy;
            UseBold = true;
        }

    }
#endregion

    //If there is audio on the sprite holder, take that and play it from the dialogue holder. used if you want to have the noise of a drink falling over or something in dialogue.
    [YarnCommand("playaudio")]
    public void PlayAudio(int clipnum)
    {
        Debug.Log("SWAP TO AUDIOMANAGER LIKE (PlaySFX AudioManager name");
       /*
        if (SpriteHolder.AudioHolder != null && clipnum > 0 && clipnum < SpriteHolder.AudioHolder.m_Clips.Length && this.gameObject.GetComponent<AudioSource>())
        {
            //Debug.Log("VOICE  ACTING !!!");
            this.gameObject.GetComponent<AudioSource>().PlayOneShot(SpriteHolder.AudioHolder.GetClip(clipnum));
        }
        else
        {
            //Debug.Log("oops, no audio");
        }
       */
    }

    [YarnCommand("CheckName")]
    public void CheckPlayerName() 
    {
      
    }


    //rion audio
    [YarnCommand("optionPlayAudio")]
    public void OptionPlayAudio(int clipnum)
    {
        CurrentInterrupt.GetComponent<Button>().onClick.AddListener(() => PlayAudio(clipnum));
    }

    
    [YarnCommand("playtestimonyaudio")]
    public void PlayTestimonyAudio()
    {
        int clipnum = UnityEngine.Random.Range(0, 2);

      
    }



   

    [YarnCommand("DHwarp")]
    public void WarpDHFunc()
        {
        
        
        


    }

    //OBSELETE set who holds the sprite, using the object name set in OnClick() in OnClickTalk. Not really needed since it's set in OnClick() directly now, but for edge cases it can still be used.
    [YarnCommand("setspriteholder")]
    public void SetSpriteHolder(string ObjectName)
    {

        if (SpriteHolder != null)
        {
            if (GameObject.Find(ObjectName))
            {
                SpriteHolder = GameObject.Find(ObjectName).GetComponent<OnClickTalk>(); // this is the TERRIBLE way I was doing it back when dialogueholder wasn't an instance, then just.. didn't change it for months. but now we can ignore this.
                Debug.Log("Spriteholder is: " + SpriteHolder);
            }

            // Debug.Log("Spriteholder is: " + SpriteHolder);

            if (SpriteHolder == null)
            {
                // fix grab
                Debug.Log("ITS NULL");
                return;
                // set again
            }

            /*
            Debug.Log("run audio block");
            if (this.gameObject.GetComponent<AudioSource>() == null)
            {
                if (this.gameObject.GetComponent<AudioWrapper>() == null)
                {
                    gameObject.AddComponent<AudioWrapper>();
                    Debug.Log("Added audiowrapper");
                }
                this.GetComponent<AudioWrapper>().m_SoundSetting = SpriteHolder.AudioHolder;
                this.GetComponent<AudioWrapper>().m_Source = this.GetComponent<AudioSource>();
                this.GetComponent<AudioWrapper>().m_Source.playOnAwake = false;
                
                Debug.Log("setup audio");
            }
            */
            //if audio wraper == null?
            // Debug.Log("Spriteholder is: " + SpriteHolder);

            // Add a copy of SpriteHolder to the list
            //if (NPCMap.instance)
            //{
            //    NPCMap.instance.AddNPC(SpriteHolder.DialogueFile, SpriteHolder.SpritePortaitList[1]);
            //}
          
        }
    }
    //give an item to the player, with the spriteholder object also holding items.
  

  

    //misc yarn command to delete the object you clicked on earlier.
    [YarnCommand("deleteObject")]
    public void DeleteObject()
    {


        Destroy(SpriteHolder.gameObject);
    }
    //in a world where yarn commands could handle overrides..

    [YarnCommand("swapanimator")]
    public void SwapAnimator()
    {

       // SpriteHolder.gameObject.GetComponentInChildren<SpriteRenderer>().gameObject.GetComponent<Animator>().runtimeAnimatorController = SpriteHolder.SecondAnimationSet;
        
        
    



        // Destroy(SpriteHolder.gameObject);
    }

    [YarnCommand("swapsprite")]
    public void SwapWorldSprite(int deathid)
    {
        if (SpriteHolder.gameObject.GetComponentInChildren<SpriteRenderer>().gameObject.GetComponent<Animator>())
        {
            SpriteHolder.gameObject.GetComponentInChildren<SpriteRenderer>().gameObject.GetComponent<Animator>().enabled = false;
        }
        
        SpriteHolder.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = SpriteHolder.SpritePortaitList[deathid];
        SpriteHolder.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = SpriteHolder.SpritePortaitList[deathid];

       

        // Destroy(SpriteHolder.gameObject);
    }
    [YarnCommand("swapspriteandlock")]
    public void KillCharacter(int deathid)
    {
        Debug.Log("kill character");
        if (SpriteHolder.gameObject.GetComponentInChildren<SpriteRenderer>().gameObject.GetComponent<Animator>())
        {
            SpriteHolder.gameObject.GetComponentInChildren<SpriteRenderer>().gameObject.GetComponent<Animator>().enabled = false;
        }
       
        SpriteHolder.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = SpriteHolder.SpritePortaitList[deathid];
        SpriteHolder.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = SpriteHolder.SpritePortaitList[deathid];
       
        SpriteHolder.enabled = false;
       
       // Destroy(SpriteHolder.gameObject);
    }

    [YarnCommand("lock")]
    public void KillCharacterNoSpriteSwap()
    {
        
        SpriteHolder.enabled = false;
       

        // Destroy(SpriteHolder.gameObject);
    }
    [YarnCommand("disappearlock")]
    public void KillCharacterInvisible()
    {
        if (SpriteHolder)
        {
            SpriteHolder.GetComponentInChildren<SpriteRenderer>().sprite = null;
            SpriteHolder.GetComponentInChildren<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            SpriteHolder.GetComponentInChildren<SpriteRenderer>().enabled = false;
            SpriteHolder.enabled = false;
           
        }
        // Destroy(SpriteHolder.gameObject);
    }


    //Cleans up dialogue and fades it out. fade is what actually kills it though
    [YarnCommand("cleanupdialogue")]
    public void Cleanup()
    {
        if (DialogueHolder.instance && DialogueHolder.instance.transform.parent && DialogueHolder.instance.transform.parent.GetComponent<Animator>())
        {
            DialogueHolder.instance.transform.parent.GetComponent<Animator>().Play("Base Layer.MoveOut", 0, 0.0f);
        }
        
        SpriteHolder = null;

        //if (GetComponentInChildren<LineView>() && GetComponentInChildren<LineView>().GetComponent<AudioWrapper>())
        //{
           // GetComponentInChildren<LineView>().GetComponent<AudioWrapper>().TypeLock = true;
       // }

        StartCoroutine(Fade());


    }

    //force ends dialogue, can get a bit buggy if done twice in a row
    [YarnCommand("forceend")]
    public void ForceEnd()
    {
        
        if (DialogueRunner.GetComponent<DialogueRunner>().IsDialogueRunning == true)
        {
            DialogueRunner.GetComponent<DialogueRunner>().Stop();
        }

       //these two need to be called for it to clean up fully
        EnableCollision();
        Cleanup();

    }

    //alow clicking on the talking object again
    [YarnCommand("reenablecollision")]
    public void EnableCollision()
    {
       
       
        if (SpriteHolder && SpriteHolder.gameObject.GetComponent<BoxCollider2D>())
        {
            //SpriteHolder.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            SpriteHolder.bEnableCollision = true;
        }

    }

    //slowly fade out the text UI before stopping dialogue
    private IEnumerator Fade()
    {
        if (DialogueHolder.instance.transform.parent && DialogueHolder.instance.transform.parent.GetComponent<Animator>())
        {
            //float temp = DialogueHolder.instance.transform.parent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(DialogueHolder.instance.transform.parent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + 0.15f); //delay for the animation then give it some leeway
        }
       // this.gameObject.SetActive(false);

       
       
        if (DialogueRunner.GetComponent<DialogueRunner>().IsDialogueRunning == true)
        {
            DialogueRunner.GetComponent<DialogueRunner>().Stop();
        }
    }

    //spawn dialogue options, branched from original interject code.


    void SharedInterjectCode(string text, string node, bool isOption)
    {
        ParentGroup.gameObject.SetActive(true);

        

       

    }
        [YarnCommand("OptionButton")]
    public void SpawnInterjectObject(string text, string node, bool isOption)
    {
        SharedInterjectCode(text, node, isOption);
    }

    

   
    /// Function to flip booleans, IsShowingOption causes the click to skip function to not trigger when true, useContinue flips whether to show the continue button when the line has finished running
    /// </summary>
    [YarnCommand("ToggleSkipOff")]
    public void ToggleSkipOff() 
    {
        Debug.Log("Skip and Continue Functions are off");
       // FindAnyObjectByType<TypeWriteSkip>().IsShowingOptions = true;
       // FindAnyObjectByType<LineView>().useContinue = false;
        //Debug.Log(FindAnyObjectByType<LineView>().useContinue);
    }

    [YarnCommand("ToggleSkipOn")]
    public void ToggleSkipOn()
    {
        //FindAnyObjectByType<TypeWriteSkip>().IsShowingOptions = false;
        //FindAnyObjectByType<LineView>().useContinue = true;
        
    }

    [YarnCommand("ToggleContinueOff")]
    public void ToggleContinueOff()  
    {
        if (GameObject.Find("ContinueImage").activeInHierarchy == true)
        {
            GameObject.Find("ContinueImage").SetActive(false);
        }
    }


   public void DestroyInterjectObject()
    {
        
    }

    public void DestroyAllOptions() 
    {
        //ButtonNodeRedirect[] options = FindObjectsByType<ButtonNodeRedirect>(FindObjectsSortMode.None);

        // BIG CHANGE MAYBE ITS WRONG

      //  for (int i = 0; i < options.Length; i++) 
       // {
         //   options[i].KillThisNOW();
          //options[i].StartCoroutine(options[i].FadeOut());
       // }


      //  FindFirstObjectByType<LineView>().typewriterEffectSpeed = OriginalTypeSpeed;
        ParentGroup.gameObject.SetActive(false);
    }

    public void InterjectMovement() 
    {
        // Interject should flash and scale, perhaps move?
    }

    //write a new function like visited that checks if a node has started at any point 
    [YarnCommand("setVisited")]
    public void ForceVisit(string nodename) 
    {
        Debug.Log("attempting a force visit");
        //bool temp;
        float temp;
        FindAnyObjectByType<InMemoryVariableStorage>().TryGetValue("$Yarn.Internal.Visiting." + nodename, out temp);



        Debug.Log(temp);

        if ((int)temp == 0) 
        {
        FindAnyObjectByType<InMemoryVariableStorage>().SetValue("$Yarn.Internal.Visiting." + nodename, 1);
        }

    }


    [YarnCommand("CheckSeen")]
    public void CheckOption(bool hasVisited) 
    {
        // if the node has been visited turn the image off, if it hasnt been visited turn it on
        // example if hasVisited (node) is true, image will be enabled false
        CurrentInterrupt.transform.GetChild(1).GetComponent<Image>().enabled = !hasVisited; 
    }



    
}
