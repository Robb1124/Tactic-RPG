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
