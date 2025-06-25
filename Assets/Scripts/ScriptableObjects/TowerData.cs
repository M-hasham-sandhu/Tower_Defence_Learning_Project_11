using UnityEngine;
using System.Collections.Generic;

public enum TowerType
{
    Type_1,
    Type_2,
    Type_3,
    Type_4
}

[System.Serializable]
public class TowerLevelStats
{
    [Header("Prefab & Visuals")]
    public GameObject towerPrefab;
    [Header("Stats")]
    public float range = 5f;
    public float attackRate = 1f;
    public float damage = 10f;
    [Header("Projectile (optional)")]
    public bool usesProjectile = false;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    [Header("Economy")]
    public int cost = 100;
    public int sellValue = 50;
}

[CreateAssetMenu(menuName = "TowerDefense/TowerData")]
public class TowerData : ScriptableObject
{
    public TowerType towerType;
    [Header("Levels")]
    public List<TowerLevelStats> levels = new List<TowerLevelStats>();
}