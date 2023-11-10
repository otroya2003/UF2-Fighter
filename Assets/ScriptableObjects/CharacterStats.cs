using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/CharacterStats")]
public class CharacterStats : ScriptableObject
{
    public int health;
    public int attack;
    public int combo;
    public float velocity;
}

