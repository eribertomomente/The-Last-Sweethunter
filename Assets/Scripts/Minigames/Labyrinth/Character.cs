// Transition between two nodes
using UnityEngine;

public class Character
{

    public int x;
    public int y;
    public GameObject obj;

    // Default weight could also be 0, but 1 will give a better animation effect
    public Character(int startX, int startY, GameObject o)
    {
        this.x = startX;
        this.y = startY;
        this.obj = o;
    }

}
