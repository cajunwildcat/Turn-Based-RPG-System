using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    private static UIManager instance;
    public static UIManager Instance { get { return instance; } }

    private CharUIDetails[] charDetails;
    private Dictionary<string, int> charPos = new Dictionary<string, int>();

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        charDetails = new CharUIDetails[transform.GetChild(0).childCount];
        for (int i = 0; i < charDetails.Length; i++) {
            charDetails[i] = transform.GetChild(0).GetChild(i).GetComponent<CharUIDetails>();
        }
    }

    public void EnableCharDetails(List<PlayerCharacter> pcs) {
        for (int i = 0; i < pcs.Count; i++) {
            charPos.Add(pcs[i].Name, i);
            charDetails[i].Name.text = pcs[i].Name;
            UpdateHealth(pcs[i].Name, pcs[i].CurrHealth, pcs[i].MaxHealth);
            UpdateMP(pcs[i].Name, pcs[i].CurrMP, pcs[i].MaxMP);
            charDetails[i].gameObject.SetActive(true);
        }
    }

    public void UpdateHealth(string name, int currHealth, int maxHealth) {
        int index = charPos[name];
        charDetails[index].HealthText.text = $"HP {currHealth} / {maxHealth}";
        charDetails[index].HealthFill.fillAmount = (float)currHealth / maxHealth;
    }

    public void UpdateMP(string name, int currMP, int maxMP) {
        int index = charPos[name];
        charDetails[index].MPText.text = $"MP {currMP} / {maxMP}";
        charDetails[index].MPFill.fillAmount = (float)currMP / maxMP;
    }
}