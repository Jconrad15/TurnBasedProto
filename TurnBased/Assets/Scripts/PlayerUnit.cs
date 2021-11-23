using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
    public Color highlightColor;

    [SerializeField]
    private UnitChoicePanelScript unitChoicePanel;

    // Events
    public event EventHandler<PlayerUnit> OnPlayerUnitSelected;

    public event EventHandler<PlayerUnit> OnPlayerUnitSelectedforMovement;
    public event EventHandler<PlayerUnit> OnPlayerUnitDeselectedforMovement;

    public event EventHandler<PlayerUnit> OnPlayerUnitSelectedforMagic;
    public event EventHandler<PlayerUnit> OnPlayerUnitDeselectedforMagic;

    public event EventHandler<PlayerUnit> OnPlayerUnitAction;

    public event EventHandler<PlayerUnit> OnPlayerUnitActionsDone;
    public event EventHandler OnPlayerUnitDeselected;

    public event EventHandler<MagicCastEventArgs> OnPlayerMagicCast;

    private void Start()
    {
        // Get all enemy units
        allEnemyUnits = FindObjectsOfType<EnemyUnit>();

        // Get all player units
        allPlayerUnits = FindObjectsOfType<PlayerUnit>();

        // Subscribe to events
        Subscribe();
    }

    private void Subscribe()
    {
        // Subscribe to on enemy events
        foreach (EnemyUnit unit in allEnemyUnits)
        {
            unit.OnEnemyMagicCast += Unit_OnEnemyMagicCast;
        }
    }

    private void UnSubscribe()
    {
        // UnSubscribe to on enemy events
        foreach (EnemyUnit unit in allEnemyUnits)
        {
            unit.OnEnemyMagicCast -= Unit_OnEnemyMagicCast;
        }
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }

    private void Unit_OnEnemyMagicCast(object sender, MagicCastEventArgs e)
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
            if (currentActionPoints <= 0)
            {
                OnPlayerUnitActionsDone?.Invoke(this, this);
                break;
            }

            if (Input.GetMouseButtonDown(0))
            {
                // determine if this unit is selected
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.CompareTag("playerUnit") &&
                        hit.transform.position == this.transform.position)
                    {
                        // tell the unit choice panel that the unit was selected
                        OnPlayerUnitSelected?.Invoke(this, this);

                        // also stop other coroutines from being perfromed
                        if (movementCoroutine != null)
                        {
                            StopCoroutine(movementCoroutine);
                        }
                        if (magicCoroutine != null)
                        {
                            StopCoroutine(magicCoroutine);
                        }
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                DeselectAll();

            }
            yield return null;
        }
    }

    private void DeselectAll()
    {
        OnPlayerUnitDeselected?.Invoke(this, EventArgs.Empty);
        OnPlayerUnitDeselectedforMovement?.Invoke(this, this);
        OnPlayerUnitDeselectedforMagic?.Invoke(this, this);

        if (isPerformingMagic == true)
        {
            StopCoroutine(magicCoroutine);
            isPerformingMagic = false;
        }

        if (isMoving == true)
        {
            StopCoroutine(movementCoroutine);
            isMoving = false;
        }
    }

    // Starts via UnitChoicePanelScript when button pressed
    public void MovementButtonSelected()
    {
        UpdateUnitLists();
        
        if (isPerformingMagic == true)
        {
            StopCoroutine(magicCoroutine);
            OnPlayerUnitDeselectedforMagic?.Invoke(this, this);
            isPerformingMagic = false;
        }
        
        // if the movement coroutine is not currently called, then call it
        if (isMoving == false)
        {
            movementCoroutine = MovementProcessing();
            StartCoroutine(movementCoroutine);

            // Call function to highlight where the unit can move
            OnPlayerUnitSelectedforMovement?.Invoke(this, this);
        }
    }

    private IEnumerator MovementProcessing()
    {
        isMoving = true;
        while (isMoving == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // determine if the ray hits the ground
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // if clicked on ground
                    if (hit.transform.CompareTag("Ground"))
                    {
                        // and if ground is empty
                        if (CheckUnitPresent(hit.transform.position) == false)
                        {
                            // Determine if the distance is within the movement range
                            // and not at the current position
                            if (CheckInRange(this.transform.position, hit.transform.position, movementRange) &&
                                this.transform.position != hit.transform.position)
                            {
                                StartCoroutine(MoveUnit(hit.transform.position));
                                GetTerrainData();
                                isMoving = false;

                                // Decrease unit action points
                                currentActionPoints -= movementActionCost;
                                Debug.Log("movement point used");

                                // Call event function to un-highlight where the unit can move
                                OnPlayerUnitDeselectedforMovement?.Invoke(this, this);

                                // Call on action event
                                OnPlayerUnitAction?.Invoke(this, this);

                            }
                            else
                            {
                                Debug.Log("Out of movement range.");
                            }
                        }
                        else
                        {
                            Debug.Log("Location is full.");
                        }



                    }
                }
            }
            yield return null;
        }
    }

    // Starts via UnitChoicePanelScript when button pressed
    public void MagicButtonSelected()
    {
        UpdateUnitLists();
        
        if (isMoving == true)
        {
            StopCoroutine(movementCoroutine);
            OnPlayerUnitDeselectedforMovement?.Invoke(this, this);
            isMoving = false;
        }
        
        // if the magic coroutine is not currently called, then call it
        if (isPerformingMagic == false)
        {
            magicCoroutine = MagicProcessing();
            StartCoroutine(magicCoroutine);

            // Call function to highlight where the unit can move
            OnPlayerUnitSelectedforMagic?.Invoke(this, this);
        }
    }

    private IEnumerator MagicProcessing()
    {
        isPerformingMagic = true;
        while (isPerformingMagic == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // determine if the ray hits the ground
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.CompareTag("Ground") || hit.transform.CompareTag("enemyUnit"))
                    {
                        // Determine if the distance is within the magic range
                        if (CheckInRange(this.transform.position, hit.transform.position, tdc.magicRange))
                        {
                            // cast magic at the clicked location
                            CastMagic(hit.transform.position);

                            // Decrease unit action points
                            currentActionPoints -= magicActionCost;

                            isPerformingMagic = false;
                            // Call function to un-highlight where the unit can move
                            OnPlayerUnitDeselectedforMagic?.Invoke(this, this);

                            // Call on action event
                            OnPlayerUnitAction?.Invoke(this, this);
                        }
                        else
                        {
                            Debug.Log("Not in range");
                        }
                    }
                }
            }
            yield return null;
        }

    }

    private void CastMagic(Vector3 location)
    {
        MagicCastEventArgs args = new MagicCastEventArgs();
        args.Location = location;
        args.TDCreator = tdc;
        OnPlayerMagicCast?.Invoke(this, args);
    }

    public void TurnIsOver()
    {
        StopAllCoroutines();
        OnPlayerUnitDeselected?.Invoke(this, EventArgs.Empty);
    }

}
