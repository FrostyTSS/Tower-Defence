using UnityEngine;
using System.Collections;

public class WaterDropSpawn : MonoBehaviour
{
    public float StartingTime = 0;
    public float TimeToAppear = 1;
    public float LerpDuration = 1.25f;
    public float LengthVisible = 1.0f;
    SpriteRenderer SpriteHolder;
    float Timer = 0;
    bool Appearing = false;
    Vector3 OriginalScale;
    public bool ChangeRotation = true;
    public bool ChangeScale = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpriteHolder = this.GetComponent<SpriteRenderer>();
        Color CurrentColour = SpriteHolder.color;
        CurrentColour.a = 0;
        SpriteHolder.color = CurrentColour;
        OriginalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime; 
        if (Timer >= TimeToAppear && Appearing == false)
        {
            Appearing = true;
            if (ChangeScale)
            {
                transform.localScale *= Random.Range(0.65f, 1.35f);
            }
            if (ChangeRotation)
            {
                transform.localEulerAngles = new Vector3(90, 0, Random.Range(-360, 360));
            }
            StartCoroutine("Appear");
        }
    }


    IEnumerator Appear() // cuz i'm too lazy to do it in update with counters
    {
        // suspend execution for 5 seconds
        
        float timeElapsed = 0;
        Color CurrentColour = SpriteHolder.color;
       
        while (timeElapsed < LerpDuration)
        {

            CurrentColour.a = Mathf.Lerp(0, 1, timeElapsed / LerpDuration);
            SpriteHolder.color = CurrentColour;
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        CurrentColour.a = 1;
        SpriteHolder.color = CurrentColour;

        yield return new WaitForSeconds(LengthVisible);

        timeElapsed = 0;

        while (timeElapsed < LerpDuration)
        {

            CurrentColour.a = Mathf.Lerp(1, 0, timeElapsed / LerpDuration);
            SpriteHolder.color = CurrentColour;
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        CurrentColour.a = 0;
        SpriteHolder.color = CurrentColour;


        Timer = 0;
        transform.localScale = OriginalScale;
        Appearing = false;
    }
}
