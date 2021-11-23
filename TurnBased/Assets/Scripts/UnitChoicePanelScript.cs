using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UnitChoicePanelScript : MonoBehaviour
{
    private CanvasGroup cg;
    private PlayerUnit currentUnit;
    private PlayerUnit[] allPlayerUnits;

    [SerializeField]
    private TextMeshProUGUI actionPointText;

    [SerializeField]
    private CanvasGroup magicDescriptionGroup;
    [SerializeField]
    private TextMeshProUGUI magicDescriptionText;

    private void Start()
    {
        cg = GetComponent<CanvasGroup>();
        HideUnitChoiceUI();
        HideMagicDescriptionUI();

        allPlayerUnits = FindObjectsOfType<PlayerUnit>();

        Subscribe();
    }

    private void Subscribe()
    {
        // Subscribe to on unit events
        foreach (PlayerUnit unit in allPlayerUnits)
        {
            unit.OnPlayerUnitSelected += Unit_OnUnitSelected;
            unit.OnPlayerUnitDeselected += Unit_OnUnitDeselected;
            unit.OnPlayerUnitActionsDone += Unit_OnUnitActionsDone;
            unit.OnPlayerUnitAction += Unit_OnUnitAction;
        }
    }

    private void Unit_OnUnitAction(object sender, PlayerUnit senderPU)
    {
        // When the unit takes an action
        UpdateAP(senderPU);
        SetMagicDescription();
    }

    private void Unit_OnUnitActionsDone(object sender, PlayerUnit senderPU)
    {
        //Shade the unit to mark it as done for the turn
        Debug.Log("This unit is done.");
        UpdateAP(senderPU);
        Invoke(nameof(HideUnitChoiceUI), 0.1f);
        Invoke(nameof(HideMagicDescriptionUI), 0.1f);
    }

    private void Unit_OnUnitDeselected(object sender, System.EventArgs e)
    {
        HideUnitChoiceUI();
        HideMagicDescriptionUI();
    }

    private void Unit_OnUnitSelected(object sender, PlayerUnit senderPU)
    {
        ShowUnitChoiceUI(senderPU);
    }

    public void ShowUnitChoiceUI(PlayerUnit senderPU)
    {
        cg.alpha = 1;
        cg.interactable = true;

        currentUnit = senderPU;

        UpdateAP(senderPU);
    }

    private void HideUnitChoiceUI()
    {
        cg.alpha = 0;
        cg.interactable = false;

        currentUnit = null;
    }

    private void UpdateAP(PlayerUnit playerUnit)
    {
        actionPointText.text = "AP: " + playerUnit.currentActionPoints.ToString();
    }

    private void ShowMagicDescriptionUI()
    {
        magicDescriptionGroup.alpha = 1;
        magicDescriptionGroup.interactable = true;

        SetMagicDescription();
    }

    private void HideMagicDescriptionUI()
    {
        magicDescriptionGroup.alpha = 0;
        magicDescriptionGroup.interactable = false;
    }

    private void SetMagicDescription()
    {        
        magicDescriptionText.text = currentUnit.tdc.magicDescription;  // description for the current terrain
    }

    public void OnMagicButtonSelected()
    {
        // Open magic description menu
        ShowMagicDescriptionUI();

        // Tell the current unit to start movement processing 
        currentUnit.MagicButtonSelected();
    }

    public void OnMoveButtonSelected()
    {
        // Tell the current unit to start movement processing 
        currentUnit.MovementButtonSelected();
    }
}
