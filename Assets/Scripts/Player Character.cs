using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour, ICombatant {
    private int maxHealth = 1000;
    private int currentHealth;
    [SerializeField]
    private int str;
    [SerializeField]
    private int def;
    [SerializeField]
    private int spd = 100;
    private int maxMP = 100;
    private int currentMP;
    public string Name {get; private set;}
    public int CurrHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public int MaxMP => maxMP;
    public int CurrMP => currentMP;
    public int Strength => str;
    public int Defense => def;
    public int Speed => spd;

    private Dictionary<string, bool> statuses = new Dictionary<string, bool>() {
        {"defending", false }
    };

    private List<PlayerAttack> abilities = new List<PlayerAttack>() {

    };

    [SerializeField]
    private GameObject TurnUI;
    private string turnAction;
    private SpriteRenderer sr;

    public void Die() {
        Color c = sr.color;
        c.a = 0.5f;
        sr.color = c;
    }

    public void TakeDamage(int amount, DamageType damageType) {
        if (statuses["defending"]) {
            currentHealth -= Mathf.Max(1, amount - def * 5);
        }
        else {
            currentHealth -= amount - def;
        }
        currentHealth = Mathf.Max(0, currentHealth);
        UIManager.Instance.UpdateHealth(name, currentHealth, maxHealth);
        if (currentHealth == 0) {
            Die();
        }
    }

    [ContextMenu("Take Turn")]
    public void TakeTurn() {
        StartCoroutine(GenerateTurn());
    }

    public IEnumerator GenerateTurn() {
        Vector3 startingPos = transform.position;
        float distance = Vector3.Distance(transform.position, CombatManager.Instance.ActivePCPosition);
        float time = 1.0f;
        float speed = distance / time;
        while (transform.position != CombatManager.Instance.ActivePCPosition) {
            transform.position = Vector3.MoveTowards(transform.position, CombatManager.Instance.ActivePCPosition, speed * Time.deltaTime);
            yield return null;
        }
        turnAction = null;
        int actionTarget = -1;
        TurnUI.SetActive(true);
        yield return new WaitUntil(() => turnAction != null);
        switch (turnAction) {
            case "Attack":
                TurnUI.SetActive(false);
                CombatManager.Instance.ActivateEnemySelector((int index) => actionTarget = index);
                yield return new WaitUntil(() => actionTarget != -1);
                CombatManager.Instance.DeactivateEnemySelector();
                int totalDamage = (int)(str * 20 * Random.Range(0.9f, 1.1f));
                ICombatant target = CombatManager.Instance.GetEnemies(this)[actionTarget];
                target.TakeDamage(totalDamage, DamageType.Sword);
                Debug.Log($"{Name} attacks {target.Name} for {totalDamage}");
                break;
            case "Abilities":
                //TODO: turn on sub menu of abilities
                break;
            case "Defend":
                //TODO: apply defending status
                break;
            case "Item":
                break;
            case "Run":
                break;
        }
        Debug.Log($"Player chose {turnAction} action");

        while (transform.position != startingPos) {
            transform.position = Vector3.MoveTowards(transform.position, startingPos, speed * Time.deltaTime);
            yield return null;
        }

        yield return null;
    }

    void Start() {
        currentHealth = maxHealth;
        currentMP = maxMP;
        Name = gameObject.name;
        sr = GetComponent<SpriteRenderer>();
    }


    void Update() {

    }

    public void SetTurnAction(string action) {
        turnAction = action;
    }
}