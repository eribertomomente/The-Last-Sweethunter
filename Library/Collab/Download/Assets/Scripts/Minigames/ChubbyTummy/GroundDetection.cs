using UnityEngine;
using System.Collections;

public class GroundDetection : MonoBehaviour
{


    void OnTriggerStay()
    {
        Debug.Log("##########################àà");
        GetComponent<Action>().setIsOnGround(true);
    }

}
