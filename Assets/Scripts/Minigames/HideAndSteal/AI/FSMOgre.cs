using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMOgre : MonoBehaviour {

    [Range(0f, 20f)] public float sight_range;

    private float reactionTime = 0f;

    private FSM fsm;
    private GameObject target;

    private Vector3 startPosition = new Vector3(4f, -10.9f, 0);

    public Animator animator;
    private Animator playerAnimator;

    void Start(){
    
        //States
        FSMState patrol = new FSMState();
        patrol.enterActions.Add(SetGoal);
        patrol.stayActions.Add(Walk);
        patrol.exitActions.Add(StopPatrol);

        FSMState attack = new FSMState();
        attack.enterActions.Add(StartAttack);
        attack.stayActions.Add(Fight);
        attack.exitActions.Add(StopAttack);

        //Transitions
        FSMTransition t1 = new FSMTransition(ScanField);
        FSMTransition t2 = new FSMTransition(Attack);

        // Link states with transitions
        patrol.AddTransition(t1, attack);
        attack.AddTransition(t2, patrol);

        // Setup a FSA at initial state
        fsm = new FSM(patrol);

        // Start monitoring
        StartCoroutine(Patrol());
    }

    // GIMMICS
    private void OnValidate()
    {
        Transform t = transform.Find("Range");
        if (t != null)
        {
            t.localScale = new Vector3(sight_range / transform.localScale.x, 1f, sight_range / transform.localScale.z) / 5f;
        }
    }

    // Periodic update, run forever
    public IEnumerator Patrol()
    {
        while (true)
        {
            fsm.Update();
            yield return new WaitForSeconds(reactionTime);
        }
    }

    // CONDITIONS

    //TRANSITION from PATROL to ATTACK
    public bool ScanField() {

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            if ((go.transform.position - transform.position).magnitude <= sight_range)
            {
                Debug.Log("Trovato un player!");
                target = go;
                playerAnimator = go.GetComponent<Animator>();
                playerAnimator.SetBool("IsAttacked", true);
                return true;
            }
        }
       

        return false;       
    }

   
    //TRANSITION from ATTACK to PATROL
    public bool Attack(){

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (go == target)
            {
                //Animazione + spostamento player nel punto iniziale di spawn
                Debug.Log("Attacco il player");
                animator.SetBool("IsInRange", true);
                StartCoroutine("StartAnimation", go);
                //Destroy(go);
                
                return true;
            }

        }

        return true;
       
    }

    // ACTION

    //enter action for PATROL
    public void SetGoal() {}

    //stay for PATROL
    public void Walk() {}

    //exit action of patrol
    public void StopPatrol(){}

    //enter action for ATTACK
    public void StartAttack() {}

    //stay action for ATTACK
    public void Fight() {}

    //exit action for ATTACK
    public void StopAttack()
    {
        target = null;
       
    }

    IEnumerator StartAnimation(GameObject go)
    {

        float timePassed = 0;
        while (timePassed < 1)
        {
            print("nel while");
           
          
            timePassed += Time.deltaTime;

            yield return null;
        }
        go.transform.position = startPosition;
        playerAnimator.SetBool("IsAttacked", false);
        animator.SetBool("IsInRange", false);

    }


}