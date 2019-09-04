using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovementBullet : NetworkBehaviour
{
    [SerializeField]
    private float bulletSpeed;
    private bool isFlipped = true;


    // Start is called before the first frame update
    void Start()
    {
        if (!isFlipped)
        {
            bulletSpeed = -bulletSpeed;
        }
        this.transform.Translate(bulletSpeed+.1f, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(bulletSpeed, 0, 0); 
        Destroy(this.gameObject, 6);
    }

    public void setIsFlipped(bool value)
    {
        isFlipped = value;
    }



}
