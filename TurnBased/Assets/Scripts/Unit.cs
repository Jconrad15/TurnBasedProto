using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;

    public int maxHealth;
    public int currentHealth;

    public int movementRange;

    public int maxActionPoints;
    public int currentActionPoints;
    public int yOffset;

    public bool isUnitActive = false;

    // Coroutine Variables
    protected IEnumerator movementCoroutine;
    protected IEnumerator magicCoroutine;

    protected bool isPerformingMagic = false;
    protected bool isMoving = false;

    // Variables to grab from the current terrain location
    public TerrainDataCreator tdc;

    // Design Variables
    protected int movementActionCost = 1;
    protected int magicActionCost = 1;
    private float movementSpeed = 5f;

    // Get references to all player units
    public PlayerUnit[] allPlayerUnits;

    // Get references to all enemy units
    public EnemyUnit[] allEnemyUnits;

    public class MagicCastEventArgs : EventArgs
    {
        public Vector3 Location { get; set; }
        public TerrainDataCreator TDCreator { get; set; }
    }

    protected bool CheckInRange(Vector3 startPosition, Vector3 targetPosition, int range)
    {
        float zDistance = Mathf.Abs(targetPosition.z - startPosition.z);
        float xDistance = Mathf.Abs(targetPosition.x - startPosition.x);
        float movementDistance = xDistance + zDistance;
        if (movementDistance <= range)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected IEnumerator MoveUnit(Vector3 targetPosition)
    {
        Vector3 targetXAxis = new Vector3(targetPosition.x, yOffset, transform.position.z);
        targetPosition = new Vector3(targetPosition.x, yOffset, targetPosition.z);

        // Move along x axis
        float step = (movementSpeed / (transform.position - targetXAxis).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step; // Goes from 0 to 1, incrementing by step each time
            transform.position = Vector3.Lerp(transform.position, targetXAxis, t); // Move object closer to target
            yield return new WaitForFixedUpdate(); // Leave the routine and return here in the next frame
        }

        transform.position = targetXAxis;

        // Move along y axis
        step = (movementSpeed / (transform.position - targetPosition).magnitude) * Time.fixedDeltaTime;
        t = 0;
        while (t <= 1.0f)
        {
            t += step; // Goes from 0 to 1, incrementing by step each time
            transform.position = Vector3.Lerp(transform.position, targetPosition, t); // Move object closer to target
            yield return new WaitForFixedUpdate(); // Leave the routine and return here in the next frame
        }

        transform.position = targetPosition;
    }

    protected void CheckIsDead()
    {
        if (currentHealth <= 0)
        {
            IsDead();
        }
    }

    protected void IsDead()
    {
        //Play any animations here.
        Destroy(gameObject);
    }

    protected void CheckHitByMagic(MagicCastEventArgs e)
    {
        // If at the magic target location
        if (transform.position.x == e.Location.x &&
            transform.position.z == e.Location.z)
        {
            HitByMagic(e);
        }
        else
        {
            // Do nothing, was not hit
        }
    }

    protected void HitByMagic(MagicCastEventArgs e)
    {
        currentHealth -= e.TDCreator.magicDamage;
        Debug.Log("currentHealth " + currentHealth);
        CheckIsDead();
    }

    protected void UpdateUnitLists()
    {
        // Get all enemy units
        allEnemyUnits = null;
        allEnemyUnits = FindObjectsOfType<EnemyUnit>();

        // Get all player units
        allPlayerUnits = null;
        allPlayerUnits = FindObjectsOfType<PlayerUnit>();
    }

    protected bool CheckUnitPresent(Vector3 location)
    {
        if (allPlayerUnits.Length >= 1)
        {
            for (int i = 0; i < allPlayerUnits.Length; i++)
            {
                if (allPlayerUnits[i].transform.position.x == location.x &&
                    allPlayerUnits[i].transform.position.z == location.z)
                {
                    // Unit is already present at this location
                    return true;
                }
            }
        }
        if (allEnemyUnits.Length >= 1)
        {
            for (int i = 0; i < allEnemyUnits.Length; i++)
            {
                if (allEnemyUnits[i].transform.position.x == location.x &&
                    allEnemyUnits[i].transform.position.z == location.z)
                {
                    // Unit is already present at this location
                    return true;
                }
            }
        }

        return false;
    }

    protected void GetTerrainData()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                tdc = hit.transform.gameObject.GetComponent<TerrainDataCreator>();
            }
            else
            {
                Debug.LogWarning("Ground not found.");
            }
        }
    }
}