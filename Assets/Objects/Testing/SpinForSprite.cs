using UnityEngine;

public class SpinForSprite : MonoBehaviour
{
    public float Speed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, Speed, 0), Space.World);
        //transform.SetLocalPositionAndRotation(transform.localPosition, Quaternion.Euler(0, (transform.rotation.eulerAngles.y + Time.deltaTime) * Speed, 0));
    }
}
