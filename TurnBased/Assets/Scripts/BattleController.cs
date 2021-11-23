using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST };

public class BattleController : MonoBehaviour
{

    public event EventHandler OnPlayerTurn;
    public event EventHandler OnEnemyTurn;

    private BattleState state;
    
    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        // Do stuff for the setup
        Debug.Log("setup");
        
        yield return new WaitForSeconds(1f);

        // Code to determine who goes first.  For now, player goes first

        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        // Player perfroms their turn
        // Raise event that states that is is the player's turn
        
        state = BattleState.PLAYERTURN;
        OnPlayerTurn?.Invoke(this, EventArgs.Empty);
    }

    public void StartEnemyTurn()
    {
        // Enemy performs their turn
        // Raise event that states that is is the enemy's turn

        Debug.Log("Raise Enemy turn event");

        state = BattleState.ENEMYTURN;
        OnEnemyTurn?.Invoke(this, EventArgs.Empty);
    }

    private void EndBattle(bool playerWin, bool enemyWin)
    {
        if (state == BattleState.WON)
        {
            Debug.Log("You won the battle!");
        }
        else if (state == BattleState.LOST)
        {
            Debug.Log("You were defeated.");
        }
    }

    public void NextTurn(bool playerWin = false, bool enemyWin = false)
    {
        Debug.Log("Next turn is triggered");

        if (playerWin == true || enemyWin == true)
        {
            EndBattle(playerWin, enemyWin);  // Need to tell EndBattle if win or lose
        }
        else
        {
            if (state == BattleState.PLAYERTURN)
            {
                StartEnemyTurn();
            }
            else if (state == BattleState.ENEMYTURN)
            {
                StartPlayerTurn();
            }
            else
            {
                Debug.LogError("Something bad happened with the turn order.");
            }
        }
    }

}
