using UnityEngine;

public class SpinForSprite : MonoBehaviour
{
    public float Speed = 5f;
    public bool Spin = false;
   public float Timer = 0; // make private later
    public float TimeToSpin = 5f;
    public GameObject SwordSwingObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        SwordSwingObj.SetActive(false);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Spin)
        {
            Timer += Time.fixedDeltaTime;
            //Debug.Log(Timer);
            transform.Rotate(new Vector3(0, Speed, 0), Space.World);
            if (Timer >= TimeToSpin)
            {
                StopSpin();
            }
        }
        //transform.SetLocalPositionAndRotation(transform.localPosition, Quaternion.Euler(0, (transform.rotation.eulerAngles.y + Time.deltaTime) * Speed, 0));
    }

    public void StartSpin()
    {
        SwordSwingObj.SetActive(true);
        Spin = true;
        this.GetComponent<BaseTower>().RotateToShoot = false;
    }

    public void StopSpin()
    {
        
        Spin = false;
        this.GetComponent<BaseTower>().RotateToShoot = true;
        SwordSwingObj.SetActive(false);
        Timer = 0;
    }
}
