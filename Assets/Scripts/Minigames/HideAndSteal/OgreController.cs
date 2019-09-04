using System.Collections.Generic;
using UnityEngine;

public class OgreController : MonoBehaviour
{

    private Vector3[] patrolPoints;

    int counter_patrol = 0;
    private float step = 0.02f;
    Vector3 destination;
    private bool flip;
    //public Animator animator;

    private void Start()
    {
        patrolPoints = new Vector3[3];
        flip = false;
        //Se è il primo orco allora setto questi punti per il patrolling
        if(tag == "Enemy_Ogre1")
        {
            patrolPoints[0] = new Vector3(21.17f, 0.09f, 0);
            patrolPoints[1] = new Vector3(13.62f, -5.81f, 0);
            patrolPoints[2] = new Vector3(8.49f, 0.75f, 0);
        }

        else
        {

            patrolPoints[0] = new Vector3(-3.51f, 9.23f, 0);
            patrolPoints[1] = new Vector3(2.49f, 5.2f, 0);
            patrolPoints[2] = new Vector3(-4.32f, 2.58f, 0);
        }

        destination = patrolPoints[counter_patrol];
        //Debug.Log("START - Destinazione: " + destination+ " e sono: "+transform.position+ " e manca: "+ (destination - transform.position));
    }

    // Update is called once per frame
    void Update()
    {
       Patrol();
    }

    public void Patrol()
    {
        //AI PATROL
        if (patrolPoints.Length > 0 && patrolPoints != null)
        {

            Vector3 toDest = destination - transform.position;

            //Debug.Log("Destinazione: " + destination + " e sono: " + transform.position + " e manca: " + (destination - transform.position));

            //Vai nel punto indicato
            if (toDest.magnitude > 1) {
            
                float xStep;
                float yStep;

                //Warning: se non funziona metti il localposition
                if (destination.x > transform.position.x) xStep = step;
                
                else xStep = -step;

                if (destination.y > transform.position.y) yStep = step;

                else yStep = -step;

                xStep += transform.position.x;
                yStep += transform.position.y;

                transform.position = new Vector3(xStep, yStep, 0);
                GetComponent<SpriteRenderer>().flipX = flip;


            }
            else
            {
                flip = !flip;
               
                //Ho raggiunto il punto indicato
                if (counter_patrol >= patrolPoints.Length - 1)
                {
                    //Rinizia dal primo punto di patrol
                    counter_patrol = 0;
                }
                else
                {
                    //Vai al prossimo punto di patrol
                    counter_patrol++;
                }

                destination = patrolPoints[counter_patrol];

            }
          

        }
        else
        {
            Debug.Log("NO patrol points set!");
        }
    }
}
