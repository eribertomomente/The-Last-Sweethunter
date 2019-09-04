using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterValues", menuName = "ScriptableObjects/ValuesScriptableObject", order = 1)]

public class CharacterValues : ScriptableObject
{
    public int Health;
    public int Armor;
    public int Attack;
    public int Special;
    public int Experience;

    public void AddExperience(int experience)
    {
        Experience += experience;
    }
}
