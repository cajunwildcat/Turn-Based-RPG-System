using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour {
    private static CombatManager instance;
    public static CombatManager Instance {
        get {
            if (instance == null) {
                Debug.LogError("There is not an active CombatManager object!");
                return null;
            }
            return instance;
        }
    }

    private List<ICombatant> enemies = new List<ICombatant>();
    private List<ICombatant> playerChars = new List<ICombatant>();
    private List<ICombatant> activeCombatants = new List<ICombatant>();
    public List<ICombatant> AllCombatants { get { return activeCombatants; } }

    public bool Ready = true;
    public List<GameObject> TestEnemies;
    public List<GameObject> TestPlayerChars;

    private List<Vector3> pcPositions = new List<Vector3>();
    public Vector3 ActivePCPosition => pcPositions.Last();
    private List<Vector3> enemyPositions = new List<Vector3>();
    private Transform EnemyTargetSelectCanvas => transform.GetChild(2);
    private EventSystem enemyTargetSelectES;
    private List<Button> enemyTargetSelects;
    private List<Navigation> enemyTargetSelectsDefault;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Debug.LogWarning("A second combat manager was created, make sure there is only one.");
            Destroy(gameObject);
        }
    }


    void Start() {
        Transform PCSlots = transform.GetChild(0);
        for (int i = 0; i < PCSlots.childCount; i++) {
            pcPositions.Add(PCSlots.GetChild(i).position);
        }
        Transform EnemySlots = transform.GetChild(1);
        for (int i = 0; i < EnemySlots.childCount; i++) {
            enemyPositions.Add(EnemySlots.GetChild(i).position);
        }
        enemyTargetSelects = new List<Button>(EnemyTargetSelectCanvas.GetComponentsInChildren<Button>());
        enemyTargetSelectsDefault = new List<Navigation>(enemyTargetSelects.Select(x => x.navigation));
        enemyTargetSelectES = EnemyTargetSelectCanvas.GetComponentInChildren<EventSystem>();

        StartCombat();
    }


    void Update() {
       if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
       if (Input.GetKeyDown(KeyCode.Alpha2)) {
            Time.timeScale = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Time.timeScale = 1;
        }
    }

    [ContextMenu("Start Combat")]
    public void StartCombat() {
        Debug.Log("Starting combat with test teams");
        StartCoroutine(StartCombat(TestEnemies.Select(x => (ICombatant)x.GetComponent<Enemy>()).ToList(), TestPlayerChars.Select(x => (ICombatant)x.GetComponent<PlayerCharacter>()).ToList()));
    }

    public IEnumerator StartCombat(List<ICombatant> enemies, List<ICombatant> playerChars) {
        ResetEnemyTargettingNav();
        this.enemies = enemies;
        this.playerChars = playerChars;

        activeCombatants.Clear();
        activeCombatants.AddRange(enemies);
        activeCombatants.AddRange(playerChars);

        for (int i = 0; i < enemies.Count; i++) {
            ((MonoBehaviour)enemies[i]).transform.position = enemyPositions[i];
            enemyTargetSelects[i].gameObject.SetActive(true);
        }

        int turnNum = 1;
        //player characters can be brought back from 0 HP, so Count will never decrease so instead check if any character is alive
        while (enemies.Where(x => x.CurrHealth > 0).Any() && playerChars.Where(x => x.CurrHealth > 0).Any()) {
            GetTurnOrder();

            for (int i = 0; i < activeCombatants.Count; i++) {
                yield return StartCoroutine(activeCombatants[i].GenerateTurn());
                yield return new WaitForSeconds(1 / Time.timeScale);
            }
            turnNum++;
        }

        Debug.Log("Combat has ended");
        yield return null;
    }

    void GetTurnOrder() {
        activeCombatants = new List<ICombatant>(from combatant in activeCombatants
                                             orderby combatant.Speed * Random.Range(0.9f,1.1f) descending
                                             select combatant);
    }

    public void RemoveCombatant(ICombatant combatant) {
        enemyTargetSelects[enemies.IndexOf(combatant)].gameObject.SetActive(false);
        //enemies.Remove(combatant);
        activeCombatants.Remove(combatant);
    }

    public List<ICombatant> GetAllies(ICombatant combatant) {
        if (enemies.IndexOf(combatant) != -1) {
            return enemies;
        }
        else if (playerChars.IndexOf(combatant) != -1) {
            return playerChars;
        }
        else {
            Debug.LogError("Combatant is not in combat!");
            return null;
        }
    }

    public List<ICombatant> GetEnemies(ICombatant combatant) {
        if (enemies.IndexOf(combatant) != -1) {
            return playerChars;
        }
        else if (playerChars.IndexOf(combatant) != -1) {
            return enemies;
        }
        else {
            Debug.LogError("Combatant is not in combat!");
            return null;
        }
    }

    #region Enemy Targeting Nav
    public void ActivateEnemySelector(System.Action<int> callback) {
        enemyTargetSelectES.SetSelectedGameObject(enemyTargetSelects.Find(x => x.gameObject.activeSelf).gameObject);
        foreach (Button b in enemyTargetSelects) {
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(() => callback(b.name[0] - '0'));
        }
        EnemyTargetSelectCanvas.gameObject.SetActive(true);
    }
    public void DeactivateEnemySelector() {
        EnemyTargetSelectCanvas.gameObject.SetActive(false);
    }

    //Top left - 0
    //Middle Left - 1
    //Bottom Left - 2
    //Top Right - 3
    //Middle Right - 4
    //Bottom Right - 5
    private void ResetEnemyTargettingNav() {
        for (int i = 0; i < enemyTargetSelects.Count; i++) {
            //enemyTargetSelects[i].navigation = enemyTargetSelectsDefault[i];
            enemyTargetSelects[i].gameObject.SetActive(false);
        }
    }
    
    #endregion

    #region Targeting Methods
    public static List<ICombatant> ChooseTargets(List<ICombatant> targets, int targetCount) {
        int[] targetIndices = new int[targetCount];
        foreach (int i in targetIndices) {
            targetIndices[i] = Random.Range(0, targets.Count);
        }
        List<ICombatant> chosenTargets = new List<ICombatant>();
        foreach (int i in targetIndices) {
            chosenTargets.Add(targets[i]);
        }
        return chosenTargets;
    }
    public static List<ICombatant> ChooseUniqueTargets(List<ICombatant> targets, int targetCount) {
        if (targetCount > targets.Count) {
            targetCount = targets.Count;
        }
        List<ICombatant> chosenTargets = new List<ICombatant>();
        while (chosenTargets.Count < targetCount) {
            int targetIndex = Random.Range(0, targets.Count);
            if (!chosenTargets.Contains(targets[targetIndex])) {
                chosenTargets.Add(targets[targetIndex]);
            }
        }
        return chosenTargets;
    }
    public static ICombatant ChooseRandomTarget(List<ICombatant> targets) {
        return targets[Random.Range(0, targets.Count)];
    }
    public static ICombatant ChooseLowestHealthTarget(List<ICombatant> targets) {
        List<ICombatant> sortedTargets = new List<ICombatant>(targets);
        sortedTargets.Sort((x, y) => x.CurrHealth.CompareTo(y.CurrHealth));
        return sortedTargets[0];
    }
    public static ICombatant ChooseHighestHealthTarget(List<ICombatant> targets) {
        List<ICombatant> sortedTargets = new List<ICombatant>(targets);
        sortedTargets.Sort((x, y) => y.CurrHealth.CompareTo(x.CurrHealth));
        return sortedTargets[0];
    }
    #endregion
}