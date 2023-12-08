using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhasesUI : MonoBehaviour
{
    public TMP_Text Phase;

    public string PhaseName;

    // Start is called before the first frame update
    void Start()
    {
        Leaf.UIUpdate += UpdateUI;
    }

    private void UpdateUI(string _name)
    {
        Phase.text = _name;
    }

    private void OnDisable()
    {
        Leaf.UIUpdate -= UpdateUI;
    }

}
