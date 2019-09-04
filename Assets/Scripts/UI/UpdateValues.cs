using UnityEngine;
using UnityEngine.UI;

public class UpdateValues : MonoBehaviour
{
    public CharacterValues characterValues;

    public Text health;
    public Text armor;
    public Text attack;
    public Text special;

    void Start()
    {
        health.text = characterValues.Health.ToString();
        armor.text = characterValues.Armor.ToString();
        attack.text = characterValues.Attack.ToString();
        special.text = characterValues.Special.ToString();
    }

}
