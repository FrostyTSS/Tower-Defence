using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static LevelInfo;

[CreateAssetMenu(fileName = "DifficultyList", menuName = "MapGameplayInfo/GlobalDifficultyList", order = 1)]
public class DifficultyList : ScriptableObject // global scriptable objects, huh..? NEVERMIDN THEY CONSTANTLY BITCH AT YOU
{

    [SerializeField]
    public List<DifficultyInfo> GlobalDifficultyList = new List<DifficultyInfo>();

}
