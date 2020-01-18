using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tactic-RPG/Class Sheet")]
public class CharacterClass : ScriptableObject
{
    [SerializeField] public string className = "Untitled";
    [SerializeField] public int baseMoveRange;
    [SerializeField] public int baseJumpHeight;

    [SerializeField] public Ability[] classAbilities;    
}
