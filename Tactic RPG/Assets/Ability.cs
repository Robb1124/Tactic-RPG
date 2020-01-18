using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tactic-RPG/Ability Sheet")]
public class Ability : ScriptableObject
{
    [SerializeField] public int abilityID;
    [SerializeField] public string abilityName;
    [TextArea(3, 6)]
    [SerializeField] public string abilityDescription;
    [Tooltip("Animation string must match a trigger in the animator")]
    [SerializeField] public string animationString = "Attack";
    [SerializeField] public AttackConfig attackConfig;
}
