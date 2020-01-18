using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetPattern
{
    SingleTarget,
    AllTargetInAim,
    Cross,
    StraightLine,
    DiagonalLine,
    StandardAoe
}

public enum AimPattern
{
    Standard,
    Melee, //comme standard mais pas impacted par des stats de bonus range ?
    StraightLine,
    DiagonalLine,
    AroundSky, //carre autour du caster
    AroundGround,
    Global
}

public enum BuffType
{
    Grunt,
    Stun
}

public enum BuffsApplication
{
    TargetsOnly,
    CasterOnly,
    AllAllies,
    AllEnemies
}

[System.Serializable]
public class AttackConfig
{
    [Header("-Attack Settings-")]
    [Tooltip("Le pattern pour les tiles selectionnable")]
    public AimPattern aimPattern; //comment les tiles selectionnable sont choisi
    [Tooltip("Le pattern pour les tiles touched par le spell")]
    public TargetPattern targetPattern; //Comment le spell fait effect (aoe, singletarget.. le pattern de aoe surtout)
    [Tooltip("Est-ce que ce spell peut toucher le caster? Non applicable pour les Straight Line et Diagonal Line Target Pattern")]
    public bool selfCastAllowed = false;
    public int maxAttackRange = 1;
    public int minAttackRange = 1;
    [Tooltip("Le aoe factor utilise pour certain target pattern (Cross, AoE)")]
    public int maxAttackSize = 1;
    [Tooltip("Le min attack size est utilise pour certain target pattern (Cross, AoE), par exemple si on veut faire un trou safe dans le aoe, 0 = pas d'effet, 1 = la target tile est sauve etc.")]
    public int minAttackSize = 0;
    [Tooltip("Quel difference de height avec le caster le spell peut toucher")]
    public int attackHeight = 0;
    [Tooltip("Quel damage on va venir rajouter a l'attaque (ou enlever) en plus des degats de base")]
    public int bonusAttackDamage = 0;
    [Tooltip("Quel pourcentage du base damage l'ability fait. (Split en different type de damage plus tard ?)")]
    public float damageModifier = 1f;

    [Header("-Buffs Settings-")]
    public bool isThereBuffs = false;
    public BuffType[] buffsToApply;
    public BuffsApplication[] howToApplyBuffs;
    public int[] buffsTurnDuration;
}
