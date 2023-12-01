using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface ICombatant {
    public string Name { get; }
    public int CurrHealth { get; }
    public int MaxHealth { get; }
    public int Strength { get; }
    public int Defense { get; }
    public int Speed { get; }
    public void TakeDamage(int amount, DamageType damageType);
    public void Die();
    public IEnumerator GenerateTurn();
}