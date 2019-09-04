using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    [SerializeField]
    private string candyName;
    [SerializeField]
    private int candyPoint;
    [SerializeField]
    private string candyColour;

    public Candy(string candyName, int candyPoint, string candyColour)
    {
        this.candyName = candyName;
        this.candyPoint = candyPoint;
        this.candyColour = candyColour;
    }

    public string GetCandyName()
    {
        return this.candyName;
    }
    public int GetCandyPoint()
    {
        return this.candyPoint;
    }
    public string GetCandyColour()
    {
        return this.candyColour;
    }

    public override bool Equals(object other)
    {
        //Check for null and compare run-time types.
        if ((other == null) || !this.GetType().Equals(other.GetType()))
        {
            return false;
        }
        else
        {
            Candy c = (Candy) other;
            return (this.candyName == c.GetCandyName()) && (this.candyColour == c.GetCandyColour());
        }
    }

    public override int GetHashCode()
    {
        return 624022166 + base.GetHashCode();
    }
}
