using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDataCreator : MonoBehaviour
{
    [SerializeField]
    private TerrainScriptableObject terrainData;

    public string terrainType;
    public string terrainDescription;
    public int magicRange;
    public string magicType;
    public string magicDescription;
    public int magicDamage;
    public GameObject particlePrefab;

    // Start is called before the first frame update
    void Start()
    {
        terrainType = terrainData.TerrainType;
        terrainDescription = terrainData.TerrainDescription;
        magicRange = terrainData.MagicRange;
        magicType = terrainData.MagicType;
        magicDescription = terrainData.MagicDescription;
        magicDamage = terrainData.MagicDamage;
        particlePrefab = terrainData.ParticlePrefab;
    }
}
