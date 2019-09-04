using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{

    public static int scoreValue = 0;

    public static int numBasic = 0;
    public static int numRedCandies = 0;
    public static int numYellowCandies = 0;
    public static int numBlueCandies = 0;
    public static int numGreenCandies = 0;
    public static int numOrangeCandies = 0;
    public static int numVioletCandies = 0;
    public static int numLollipop = 0;
    public static int numPraline = 0;
    public static int numLicorice = 0;
    public static int numGummyBear = 0;
    public static int numSugarCane = 0;
    public static int numMarshmallow = 0;
    public static int numMacaron = 0;
    public static int numDonut = 0;
    public static int numCupcake = 0;

    Text score;
    // Start is called before the first frame update
    void Start()
    {

        // WARNING
        score = GameObject.FindGameObjectsWithTag("playerstat")[0].GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        score.text = ""+ scoreValue;
    }
}
