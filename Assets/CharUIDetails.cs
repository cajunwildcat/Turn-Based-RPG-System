using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharUIDetails : MonoBehaviour {
    public TMP_Text Name => transform.GetChild(0).GetComponent<TMP_Text>();
    public Image BoostFill => transform.GetChild(1).GetChild(0).GetComponent<Image>();
    public TMP_Text HealthText => transform.GetChild(3).GetComponent<TMP_Text>();
    public Image HealthFill => transform.GetChild(4).GetComponent<Image>();
    public TMP_Text MPText => transform.GetChild(5).GetComponent<TMP_Text>();
    public Image MPFill => transform.GetChild(6).GetComponent<Image>();
}