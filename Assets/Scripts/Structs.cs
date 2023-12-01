using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnemyAttack {
    public float weight;
    public TargetType targetType;
    public TargetProirity targetProirity;
    public int targetCount;
    public DamageType damageType;
    public int power;
    public int hitCount;
    public bool canCrit;
}

public struct PlayerAttack {
    public TargetType targetType;
    public TargetProirity targetProirity;
    public int targetCount;
    public DamageType damageType;
    public int power;
    public int hitCount;
    public bool canCrit;
    public int manaCost;
    public int maxTurnCooldown;
    public int currentTurnCooldown;
}