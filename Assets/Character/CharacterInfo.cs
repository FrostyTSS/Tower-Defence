using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CharacterInfo", menuName = "CharacterInfo", order = 1)]
public class CharacterInfo : ScriptableObject
{

    public List<Sprite> PoseSpriteList;
    public List<Sprite> MouthSpriteList;
    public int FirstTalkingMouthSprite;
    public Sprite BlinkSprite;
    public Sprite GlassesSprite;
    public Vector2 BlinkPos;
    public Vector2 MouthPos;
    public Vector2 GlassesPos;
    public Vector2 BlinkScale;
    public Vector2 MouthScale;
    public Vector2 GlassesScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
