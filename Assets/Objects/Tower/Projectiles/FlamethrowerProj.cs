using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FlamethrowerProj : ProjectileBase
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
      
        


    }

   

    

    public override void OnProjHit()
    {
        if (ProjTargetObj && ProjOwner.GetComponent<FlamethrowerScript>())
        {

            
            FlamethrowerScript ProjFlamethrowerOwner = ProjOwner.GetComponent<FlamethrowerScript>();

            
               // Debug.Log("Modified Speed = " + "Target Speed: " + ProjTargetObj.MaxSpeed + " and SpeedMod is" + ProjFlamethrowerOwner.SpeedModifier + "and finally, our total speed is: " + (ProjTargetObj.MaxSpeed *= ProjFlamethrowerOwner.SpeedModifier));
                ProjTargetObj.StartCoroutine(ProjTargetObj.PercentageSlowdown(ProjFlamethrowerOwner.SpeedModifier, ProjFlamethrowerOwner.SpeedModifierLength, 0.25f, ProjFlamethrowerOwner));
            
           
        }
    }

   

    // Update is called once per frame
   
}
