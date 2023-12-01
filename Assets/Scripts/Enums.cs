using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType {
    //Physical Damages
    Sword = 0,
    Axe = 1,
    Bow = 2,
    Lance = 3,
    Staff = 4,
    Dagger = 5,
    //Magic damage
    Fire = 6,
    Wind = 7,
    Ice = 8,
    Lightning = 9,
    Dark = 10,
    Holy = 11,
    //Healing
    Heal = 50,
    //Unique damage
    Special = 99
}

public enum TargetType {
    Self = 1,
    Ally = 2,
    Enemy = 3,
    All = 4,
    AllAllies = 5,
    AllEnemies = 6,
    RandomAlly = 7,
    RandomEnemy = 8,
}

public enum TargetProirity {
    LowestHealth = 1,
    HighestHealth = 2
}

public enum PlayerTurnStatus {
    Waiting = 0,
    Completed = 1,
    SettingActive = 2,
    WaitingForInput = 3,
    ExecutingAction = 4
}