using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHighlighting : MonoBehaviour
{
    private PlayerUnit[] allPlayerUnits;

    private Material[] groundMaterials;
    private Transform[] groundTransforms;
    private Color[] standardGroundColors;

    public Color highlightColor;

    private void Start()
    {
        allPlayerUnits = FindObjectsOfType<PlayerUnit>();

        groundTransforms = new Transform[transform.childCount];
        for (int i = 0; i < groundTransforms.Length; i++)
        {
            groundTransforms[i] = transform.GetChild(i);
        }

        //Set correct lenght of arrays
        groundMaterials = new Material[groundTransforms.Length];
        standardGroundColors = new Color[groundTransforms.Length];

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            groundMaterials[i] = renderers[i].material;
            standardGroundColors[i] = groundMaterials[i].color;
        }
        
        Subscribe();

    }

    private void Subscribe()
    {
        // Subscribe to on unit events
        foreach (PlayerUnit unit in allPlayerUnits)
        {
            unit.OnPlayerUnitSelectedforMovement += Unit_OnUnitSelectedforMovement;
            unit.OnPlayerUnitDeselectedforMovement += Unit_OnUnitDeselectedforMovement;
            unit.OnPlayerUnitSelectedforMagic += Unit_OnUnitSelectedforMagic;
            unit.OnPlayerUnitDeselectedforMagic += Unit_OnUnitDeselectedforMagic;
        }
    }

    private void Unit_OnUnitDeselectedforMagic(object sender, PlayerUnit e)
    {
        //Un-Highlight the ground where magic can be cast
        StopCoroutine(HighlightGround(e.tdc.magicRange, e.transform.position));
        ResetColors();
    }

    private void Unit_OnUnitSelectedforMagic(object sender, PlayerUnit e)
    {
        //Highlight the ground where magic can be cast
        StartCoroutine(HighlightGround(e.tdc.magicRange, e.transform.position));
    }

    private void Unit_OnUnitDeselectedforMovement(object sender, PlayerUnit e)
    {
        //Un-highlight the ground where the unit can go
        StopCoroutine(HighlightGround(e.movementRange, e.transform.position));
        ResetColors();
    }

    private void Unit_OnUnitSelectedforMovement(object sender, PlayerUnit e)
    {
        //Highlight the ground where the unit can go
        StartCoroutine(HighlightGround(e.movementRange, e.transform.position));
    }

    IEnumerator HighlightGround(int range, Vector3 centerPosition)
    {
        for (int i = 0; i < groundTransforms.Length; i++)
        {
            // Determine round tiles within range
            float zDistance = Mathf.Abs(groundTransforms[i].position.z - centerPosition.z);
            float xDistance = Mathf.Abs(groundTransforms[i].position.x - centerPosition.x);
            float totalDistance = xDistance + zDistance;
            if (totalDistance <= range)
            {
                // Highlight the ground tiles in the range
                groundMaterials[i].color = highlightColor;
            }
        }
        yield return null;
    }

    private void ResetColors()
    {
        for (int i = 0; i < standardGroundColors.Length; i++)
        {
            groundMaterials[i].color = standardGroundColors[i];
        }
    }

}
