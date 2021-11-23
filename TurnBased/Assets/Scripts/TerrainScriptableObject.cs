using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TerrainData", menuName = "TerrainScriptableObject", order = 51)]
public class TerrainScriptableObject : ScriptableObject
{
    [SerializeField]
    private string _terrainType;
    [SerializeField]
    private string _terrainDescription;

    [SerializeField]
    private int _magicRange;

    [SerializeField]
    private string _magicType;
    [SerializeField]
    private string _magicDescription;

    [SerializeField]
    private int _magicDamage;

    [SerializeField]
    private GameObject _particlePrefab;

    public string TerrainType { get { return _terrainType; } }
    public string TerrainDescription { get { return _terrainDescription; } }
    public int MagicRange { get { return _magicRange; } }
    public string MagicType { get { return _magicType; } }
    public string MagicDescription { get { return _magicDescription; } }
    public int MagicDamage { get { return _magicDamage; } }

    public GameObject ParticlePrefab { get { return _particlePrefab; } }
}
