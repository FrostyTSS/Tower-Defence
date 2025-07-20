using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;
using UnityEngine.SceneManagement;


public class OnClickTalk : MonoBehaviour
{
    /***
     * 
     * In Proximity && (hovering) outline get brighter colour - glows pulse
     * Out of Proximity (hovering) outline is red/other color - no glow
     * Out of proximity (not hovering) - outline size is 0
     * In Proximity && notHovering - outline gets bigger, not glowing
     * 
     * 
     * InProximity bool + OnMouseOver, OnMouseExit trigger
     * 
     ***/


    // Start is called before the first frame update
    //bool OnClickActiveLock = false; // locks it, otherwise it overrides buttons
  DialogueRunner dialoguerun; //DialogueRunner, gotten from DialogueHolder. just makes it easier to type, sacrifces a bit of ram for loading speed.
    public string DialogueFile; // What dialogue node we want to launch on click.
    //public Transform stopLocation;

    public List<Sprite> SpritePortaitList; // sprites to load while talking
  

    
    //public bool WalkTo = true;
    //bool isHover = false;



    //public Renderer attachedRenderer;

    //UnityEngine.Color darkYellow = new UnityEngine.Color(0.6981132f, 0.3302856f, 0.1152545f, 0.9f);
    //UnityEngine.Color brightYellow = new UnityEngine.Color(1, 0.7752187f, 0.0f, 0.9f);
    //UnityEngine.Color boldRed = new UnityEngine.Color(0.396f, 0.017f, 0.0283f, 0.9f);
    
   
    
    private void Awake()
    {
      
    }

    private void Start()
    {

        //set dialoguerunner component just to make it easier to write scirpts
        if (DialogueHolder.instance)
        {
            dialoguerun = DialogueHolder.instance.DialogueRunner.GetComponent<DialogueRunner>();

           
        }
        else
        {
            Debug.Log("NOT SET UP RIGHT, MAY CAUSE ISSUE");
        }

        if (SpritePortaitList.Count == 1) // debug for random issue
        {
            SpritePortaitList.Add(SpritePortaitList[0]);
        }
    }

    public void SetButtonOverlayOff() 
    {
        gameObject.SetActive(false);
    }
   
    //manually launch a dialogue node, useful for if you don't want any NPCs or the player object in the scene, and just dialogue.
    public void ManualLaunch(string NodeName)
    {
        if (dialoguerun == null) 
        {
            dialoguerun = DialogueHolder.instance.DialogueRunner.GetComponent<DialogueRunner>();
        }
        dialoguerun.VariableStorage.SetValue("$CurrentObjectName", this.gameObject.name); // OBSELETE DUE TO SWAPPING TO GETTING THE SPRITE HOLDER DIRECTLY ON CLICK
        
        DialogueHolder.instance.gameObject.SetActive(true);
        DialogueHolder.instance.NPCPortrait.GetComponent<UnityEngine.UI.Image>().sprite = SpritePortaitList[0]; // set default portraits to the first sprite
        if (DialogueHolder.instance.NPCPortrait.GetComponent<Animator>()) // assign animator if there is any
        {
          //  DialogueHolder.instance.NPCPortrait.GetComponent<Animator>().runtimeAnimatorController = AnimationControllerToUse;
        }

       


        DialogueHolder.instance.SpriteHolder = this;
        StartCoroutine(DisableColliderAndStartDialogueNode(NodeName)); // start up
         
        dialoguerun.StartDialogue(NodeName);
        // tell yarn to wake up and load it
        if (gameObject.GetComponent<BoxCollider2D>())
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }

    }

    //same as above but for when you actually click on a target, and not just activate from another script, mostly same code as above
    public void OnClick()
    {
        DialogueHolder.instance.DialogueRunner.GetComponent<DialogueRunner>().Stop(); // stop currently running dialogue if its still left over
   
        DialogueHolder.instance.TextPortrait.GetComponent<YarnSpriteChange>().UsePixelTransition = false;


        //AGAIN this section is obselete due to changes below, like setting spriteholder directly. kept here for legacy reasons/
        dialoguerun.VariableStorage.SetValue("$CurrentObjectName", this.gameObject.name); 
      
        string testVariable;
        dialoguerun.VariableStorage.TryGetValue("$CurrentObjectName", out testVariable);
        Debug.Log(testVariable);


        DialogueHolder.instance.SpriteHolder = this; // replacement for above
        DialogueHolder.instance.gameObject.SetActive(true);
        DialogueHolder.instance.NPCPortrait.GetComponent<UnityEngine.UI.Image>().sprite = SpritePortaitList[0];
        if (DialogueHolder.instance.NPCPortrait.GetComponent<Animator>())
        {
            //DialogueHolder.instance.NPCPortrait.GetComponent<Animator>().runtimeAnimatorController = AnimationControllerToUse;
        }
      
        DialogueHolder.instance.transform.parent.GetComponent<Animator>().Play("Base Layer.Movein", 0, 0.0f);

        
       

        //dialoguerun.VariableStorage.TryGetValue("$testVariable", out testVariable);

        //  DialogueSystem.instance.StartDialogue(this.gameObject);

    }

    float fMaxTimer = 2.0f;
    float fTimer = 0.0f;
    [HideInInspector] public bool bEnableCollision = false;

    // Manual Timer to prevent double clicking glitch :D
    public void Update()
    {

        if (bEnableCollision == true)
        {
            fTimer += Time.deltaTime;

            if (fMaxTimer < fTimer)
            {
                this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
                fTimer = 0.0f;
                bEnableCollision = false;
            }
        }
       
    }

    public IEnumerator EnableCollision(float deltaTime)
    {
        yield return new WaitForSeconds(deltaTime);

        this.GetComponent<BoxCollider2D>().enabled = true;
    }


    void OnMouseExit()  
    {
        // isHover = false;
        // Out of proximity(not hovering) -outline size is 0
      
        //attachedRenderer.sharedMaterial.SetFloat("_OutlineSize", 0.0f);
    }

    private void OnMouseDown()
    {
 
        //i don't remember making it this awful

        
                                        OnClick();

                         

            #region OldAIWalk
            //if (WalkTo)
            //{
            //    this.gameObject.GetComponent<PathToInteract>().StartCoroutine("ActivateWhenNear");
            //}
            //else
            //{
            //    if (inProximity()) 
            //    {
            //        OnClick();
            //    }
            //}
            // OnClickActiveLock = true;
            // Inventory.instance.gameObject.GetComponent<PlayerMovement>().canMove = false;
            //OnClick();
            #endregion
        
    }

    IEnumerator DisableColliderAndStartDialogueNode(string dialogueNode) // disable collider of the object you clicked on just in case, then fade in the text portrait
    {

        yield return new WaitForSeconds(0.05f);
        dialoguerun.Stop();
        dialoguerun.StartDialogue(dialogueNode);
        if (gameObject.GetComponent<BoxCollider2D>())
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }

        yield return new WaitForSeconds(1.25f);
      
        DialogueHolder.instance.TextPortrait.GetComponent<YarnSpriteChange>().UsePixelTransition = true; // enable funny pixelisation when swapping small text portrait


    }



    IEnumerator DisableColliderAndStartDialogue() // disable collider of the object you clicked on just in case, then fade in the text portrait
    {
      
        yield return new WaitForSeconds(0.05f);
        dialoguerun.Stop();
        Debug.Log("running the node from here");
        dialoguerun.StartDialogue(DialogueFile);
        if (gameObject.GetComponent<BoxCollider2D>())
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }

        yield return new WaitForSeconds(1.25f);
      
        DialogueHolder.instance.TextPortrait.GetComponent<YarnSpriteChange>().UsePixelTransition = true; // enable funny pixelisation when swapping small text portrait


    }


  



   
}