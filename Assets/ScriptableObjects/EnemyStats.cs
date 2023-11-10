using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public int health;
    public int attack;
    public float velocity;
}
