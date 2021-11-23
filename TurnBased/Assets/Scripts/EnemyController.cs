using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private BattleController battleController;
    private bool isEnemyTurn = false;

    private EnemyUnit[] allEnemyUnits;

    // Start is called before the first frame update
    void Start()
    {
        battleController.OnEnemyTurn += BattleController_OnEnemyTurn;

        allEnemyUnits = GetComponentsInChildren<EnemyUnit>();
    }

    private void BattleController_OnEnemyTurn(object sender, System.EventArgs e)
    {
        StartCoroutine(StartEnemyTurn());
    }

    private IEnumerator StartEnemyTurn()
    {
        // It is the enemys turn
        Debug.Log("Enemy turn");
        
        // Set the player turn bool in this 
        isEnemyTurn = true;

/*      // Set the player turn bool in each player unit
        foreach (EnemyUnit unit in allEnemyUnits)
        {

            unit.isUnitActive = true;
            unit.TurnStartSetup();
            while (unit.isUnitActive == true)
            {
                yield return null;
            }
        }*/
        yield return new WaitForSeconds(0.2f);
        EndTurn();
    }

    public void EndTurn()
    {
        // This is called when the next turn button is pushed
        Debug.Log("End Enemy Turn.");
        isEnemyTurn = false;

        // Set the player turn bool in each player unit
        foreach (EnemyUnit unit in allEnemyUnits)
        {
            unit.isUnitActive = false;
            unit.TurnIsOver();
        }

        // Tell the battle controller to go to the next turn
        battleController.NextTurn();
    }
}
