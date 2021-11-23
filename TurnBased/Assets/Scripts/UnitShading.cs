using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitShading : MonoBehaviour
{
    private BattleController battleController; 
    private PlayerUnit attachedPlayerUnit;

    private Material material;
    private Color standardColor;

    // Start is called before the first frame update
    private void Start()
    {
        battleController = FindObjectOfType<BattleController>();
        
        attachedPlayerUnit = GetComponent<PlayerUnit>();

        material = GetComponent<Renderer>().material;
        standardColor = material.color;

        Subscribe();
    }

    private void Subscribe()
    {
        attachedPlayerUnit.OnPlayerUnitActionsDone += Unit_OnUnitActionsDone;

        battleController.OnPlayerTurn += BattleController_OnPlayerTurn;
    }

    private void BattleController_OnPlayerTurn(object sender, System.EventArgs e)
    {
        ShadeUnit(false);
    }

    private void Unit_OnUnitActionsDone(object sender, PlayerUnit playerUnit)
    {
        ShadeUnit(true, playerUnit);
    }

    private void ShadeUnit(bool isShaded, PlayerUnit playerUnit = null)
    {
        if (isShaded)
        {
            material.color = playerUnit.highlightColor;
        }
        else
        {
            material.color = standardColor;
        }
    }

}
