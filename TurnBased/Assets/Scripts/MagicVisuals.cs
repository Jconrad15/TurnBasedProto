using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicVisuals : MonoBehaviour
{
    private PlayerUnit[] allPlayerUnits;
    private EnemyUnit[] allEnemyUnits;

    private int destroyDelay = 10;

    private void Start()
    {
        allPlayerUnits = FindObjectsOfType<PlayerUnit>();
        allEnemyUnits = FindObjectsOfType<EnemyUnit>();

        // Subscribe to on player magic event
        Subscribe();
    }

    // Event Subscriptions
    private void Subscribe()
    {
        // Subscribe to on player events
        foreach (PlayerUnit unit in allPlayerUnits)
        {
            unit.OnPlayerMagicCast += Unit_OnPlayerMagicCast;
        }

        // Subscribe to on enemy events
        foreach (EnemyUnit unit in allEnemyUnits)
        {
            unit.OnEnemyMagicCast += Unit_OnEnemyMagicCast;
        }
    }

    private void UnSubscribe()
    {
        // Subscribe to on player events
        foreach (PlayerUnit unit in allPlayerUnits)
        {
            unit.OnPlayerMagicCast -= Unit_OnPlayerMagicCast;
        }

        // Subscribe to on enemy events
        foreach (EnemyUnit unit in allEnemyUnits)
        {
            unit.OnEnemyMagicCast -= Unit_OnEnemyMagicCast;
        }
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }

    // On Events
    private void Unit_OnEnemyMagicCast(object sender, Unit.MagicCastEventArgs e)
    {
        CreateParticles(e);
    }

    private void Unit_OnPlayerMagicCast(object sender, Unit.MagicCastEventArgs e)
    {
        CreateParticles(e);
    }

    // Private Internal Functions
    private void CreateParticles(Unit.MagicCastEventArgs e)
    {
        GameObject particleClone = Instantiate(e.TDCreator.particlePrefab, e.Location, Quaternion.identity);
        Destroy(particleClone, destroyDelay);
    }
}
