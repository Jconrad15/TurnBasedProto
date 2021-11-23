using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType { MAGIC, MOVE, WAIT };
public class EnemyUnit : Unit
{
    // Events
    public event EventHandler<EnemyUnit> OnEnemyUnitSelected;

    public event EventHandler<EnemyUnit> OnEnemyUnitSelectedforMovement;
    public event EventHandler<EnemyUnit> OnEnemyUnitDeselectedforMovement;

    public event EventHandler<EnemyUnit> OnEnemyUnitSelectedforMagic;
    public event EventHandler<EnemyUnit> OnEnemyUnitDeselectedforMagic;

    public event EventHandler<EnemyUnit> OnEnemyUnitAction;

    public event EventHandler<EnemyUnit> OnEnemyUnitActionsDone;
    public event EventHandler OnEnemyUnitDeselected;

    public event EventHandler<MagicCastEventArgs> OnEnemyMagicCast;

    private void Start()
    {
        allPlayerUnits = FindObjectsOfType<PlayerUnit>();
        allEnemyUnits = FindObjectsOfType<EnemyUnit>();

/*        movementCoroutine = MovementProcessing();
        magicCoroutine = MagicProcessing();
*/
        // Subscribe to on player magic event
        Subscribe();
    }

    private class Action
    {
        public Vector3 TargetLocation { get; set; }
        public ActionType Type { get; set; }
    }

    private void Subscribe()
    {
        // Subscribe to on unit events
        foreach (PlayerUnit unit in allPlayerUnits)
        {
            unit.OnPlayerMagicCast += Unit_OnMagicCast;
        }
    }

    private void UnSubscribe()
    {
        // UnSubscribe to on unit events
        foreach (PlayerUnit unit in allPlayerUnits)
        {
            unit.OnPlayerMagicCast -= Unit_OnMagicCast;
        }
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }

    private void Unit_OnMagicCast(object sender, MagicCastEventArgs e)
    {
        CheckHitByMagic(e);
    }

    public void TurnStartSetup()
    {
        // Determine action points
        currentActionPoints = maxActionPoints;

        GetTerrainData();

        StartCoroutine(UnitProcessing());
    }

    public IEnumerator UnitProcessing()
    {
        while (isUnitActive == true)
        {
            if (currentActionPoints > 0)
            {
                Action action = DetermineAction();

                if (action.Type == ActionType.MAGIC)
                {
                    MagicProcessing(action);
                }
                else if (action.Type == ActionType.MOVE)
                {
                    MovementProcessing(action);
                }
                else if (action.Type == ActionType.WAIT)
                {
                    WaitProcessing(action);
                }

                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                isUnitActive = false;
                OnEnemyUnitActionsDone?.Invoke(this, this);
                break;
            }
        }
    }

    private void WaitProcessing(Action action)
    {
        currentActionPoints -= 1;
    }

    private Action DetermineAction()
    {
        // The Enemy unit needs to determine what to do
        Action action = new Action();
        Action defaultAction = new Action();

        // If the enemy can attack a player unit, then do so
        foreach (PlayerUnit playerunit in allPlayerUnits)
        {
            if (CheckInRange(this.transform.position, playerunit.transform.position, tdc.magicRange))
            {
                action.TargetLocation = playerunit.transform.position;
                action.Type = ActionType.MAGIC;
                return action;
            }
        }

        Vector3 closestPlayerUnitLocation = new Vector3(5000, 5000, 5000);
        // If the enemy can move toward the closest player, then do so
        foreach (PlayerUnit playerunit in allPlayerUnits)
        {
            if ((this.transform.position - playerunit.transform.position).magnitude < closestPlayerUnitLocation.magnitude)
            {
                closestPlayerUnitLocation = playerunit.transform.position;
            }
        }

        Vector3 difference = (closestPlayerUnitLocation - this.transform.position).normalized;
        for (int i = (int)Mathf.Abs(difference.x); i <= 0; i--)
        {
            Vector3 movePosition = this.transform.position + (difference * movementRange);
            if (CheckInRange(this.transform.position, movePosition, movementRange))
            {
                action.TargetLocation = this.transform.position;
                action.Type = ActionType.MOVE;
                return action;
            }
        }

        defaultAction.TargetLocation = new Vector3(0, 0, 0);
        defaultAction.Type = ActionType.WAIT;
        return defaultAction;
    }

    private void MovementProcessing(Action action)
    {
        StartCoroutine(MoveUnit(action.TargetLocation));
        GetTerrainData();

        // Decrease unit action points
        currentActionPoints -= movementActionCost;
        Debug.Log("Enemy movement point used");

        // Call event function to un-highlight where the unit can move
        OnEnemyUnitDeselectedforMovement?.Invoke(this, this);

        // Call on action event
        OnEnemyUnitAction?.Invoke(this, this);
    }

    private void MagicProcessing(Action action)
    {
        // cast magic at the clicked location
        CastMagic(action.TargetLocation);

        // Decrease unit action points
        currentActionPoints -= magicActionCost;

        // Call function to un-highlight where the unit can move
        OnEnemyUnitDeselectedforMagic?.Invoke(this, this);

        // Call on action event
        OnEnemyUnitAction?.Invoke(this, this);
    }

    private void CastMagic(Vector3 location)
    {
        MagicCastEventArgs args = new MagicCastEventArgs();
        args.Location = location;
        args.TDCreator = tdc;
        OnEnemyMagicCast?.Invoke(this, args);
    }

    public void TurnIsOver()
    {
        StopAllCoroutines();
        OnEnemyUnitDeselected?.Invoke(this, EventArgs.Empty);
    }
}
