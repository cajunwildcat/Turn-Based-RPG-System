using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, ICombatant {
    [SerializeField]
    Image healthBarFill;

    public string Name { get; private set; }
    [SerializeField]
    private int maxHealth;
    private int currentHealth;
    [SerializeField]
    private int str;
    [SerializeField]
    private int def;
    [SerializeField]
    private int spd;

    public int CurrHealth => currentHealth;
    public int Strength => str;
    public int Defense => def;
    public int Speed => spd;

    private List<EnemyAttack> attacks = new List<EnemyAttack>() {
        new EnemyAttack {
            weight = 1,
            targetType = TargetType.RandomEnemy,
            targetCount = 1,
            damageType = DamageType.Sword,
            power = 20,
            hitCount = 1
        }
    };
    private float sumAttackWeights = 0;

    private SpriteRenderer sr;

    void Start() {
        currentHealth = maxHealth;
        foreach (EnemyAttack attack in attacks) {
            sumAttackWeights += attack.weight;
        }
        Name = gameObject.name;
        sr = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int amount, DamageType damageType) {
        currentHealth -= Mathf.Max(0, amount - def);
        healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        if (currentHealth <= 0) Die();
    }

    public void Die() {
        CombatManager.Instance.RemoveCombatant(this);
        Color c = sr.color;
        c.a = 0.5f;
        sr.color = c;
    }

    public IEnumerator GenerateTurn() {
        float attackRoll = UnityEngine.Random.Range(0, sumAttackWeights);
        EnemyAttack chosenAttack = new EnemyAttack { };
        foreach (EnemyAttack attack in attacks) {
            attackRoll -= attack.weight;
            if (attackRoll <= 0) {
                chosenAttack = attack;
                break;
            }
        }
        List<ICombatant> targets = new List<ICombatant>();
        switch (chosenAttack.targetType) {
            case TargetType.Self:
                targets.Add(this);
                break;
            case TargetType.Ally:
                throw new NotImplementedException();
                switch (chosenAttack.targetProirity) {
                    case TargetProirity.LowestHealth:
                        targets.Add(CombatManager.ChooseLowestHealthTarget(CombatManager.Instance.GetAllies(this)));
                        break;
                    case TargetProirity.HighestHealth:
                        targets.Add(CombatManager.ChooseHighestHealthTarget(CombatManager.Instance.GetAllies(this)));
                        break;
                }
            case TargetType.Enemy:
                throw new NotImplementedException();
                switch (chosenAttack.targetProirity) {
                    case TargetProirity.LowestHealth:
                        targets.Add(CombatManager.ChooseLowestHealthTarget(CombatManager.Instance.GetEnemies(this)));
                        break;
                    case TargetProirity.HighestHealth:
                        targets.Add(CombatManager.ChooseHighestHealthTarget(CombatManager.Instance.GetEnemies(this)));
                        break;
                }
            case TargetType.All:
                targets.AddRange(CombatManager.Instance.AllCombatants);
                break;
            case TargetType.AllAllies:
                targets.AddRange(CombatManager.Instance.GetAllies(this));
                break;
            case TargetType.AllEnemies:
                targets.AddRange(CombatManager.Instance.GetEnemies(this));
                break;
            case TargetType.RandomAlly:
                targets.Add(CombatManager.ChooseRandomTarget(CombatManager.Instance.GetAllies(this)));
                break;
            case TargetType.RandomEnemy:
                targets.Add(CombatManager.ChooseRandomTarget(CombatManager.Instance.GetEnemies(this)));
                break;
        }

        float moveSpeed = 2.8f;
        Vector3 targetPos = transform.position;
        targetPos.x += 0.7f;
        while (transform.position != targetPos) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        targetPos.x -= 0.7f;
        while (transform.position != targetPos) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        foreach (ICombatant target in targets) {
            for (int i = 0; i < chosenAttack.hitCount; i++) {
                int totalDamage = (int)UnityEngine.Random.Range(str * chosenAttack.power * 0.9f, str * chosenAttack.power * 1.1f);
                if (chosenAttack.canCrit && UnityEngine.Random.Range(0, 100) < 1) {
                    totalDamage *= 2;
                }
                target.TakeDamage(totalDamage, chosenAttack.damageType);
                Debug.Log($"{Name} attacks {target.Name} for {totalDamage}");
            }
        }
        yield return null;
    }
}