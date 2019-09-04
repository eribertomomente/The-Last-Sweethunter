using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//TODO implementare network behaviour
public class HealthBar : MonoBehaviour
{ 
    
    private float lifeValueNormalized = 1f;

    public Sprite greenHealth;
    public Sprite yellowHealth;
    public Sprite redHealth;
    public Sprite noHealth;
    
    public Transform bar;
    //private Transform barImage;

    public LobbyPlayer myPlayerInfo;
    public Action action;

    // Start is called before the first frame update
    private void Start()
    {
        if (myPlayerInfo.characterIndex > 2)
        {
            bar.gameObject.SetActive(false);
        }
        else
        {
            myPlayerInfo.UpdateLifeValue(lifeValueNormalized);
        }
    }
    

    public void Update()
    {
        float lifeVal = myPlayerInfo.normalizedLifeValue;
        
        if (lifeVal > .7f)
        {
            bar.GetComponent<Image>().sprite = greenHealth;
        }
        else if (lifeVal > .4f)
        {
            bar.GetComponent<Image>().sprite = yellowHealth;
        }
        else if (lifeVal > .1f)
        {
            bar.GetComponent<Image>().sprite = redHealth;
        }
        else
        {
            bar.GetComponent<Image>().sprite = noHealth;
        }
    }
}
