using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO implementare network behaviour
public class HealthBar : MonoBehaviour
{ 
    
    private float lifeValueNormalized = 1f;

    public Color perfetctHealth;
    public Color goodHealth;
    public Color mediumHealth;
    public Color lowHealth;
    public Color zeroHealth;

    public GameObject HealthBarContainer;
    private Transform bar;
    private Transform barImage;

    // Start is called before the first frame update
    private void Start()
    {
        bar = HealthBarContainer.transform.Find("Bar");
        barImage = bar.Find("BarImage");

        bar.localScale = new Vector3(lifeValueNormalized, 1f);
        barImage.GetComponent<Image>().color = perfetctHealth;
    }

    public void setLifeValue(float lifeVal)
    {
        if (lifeVal < 1f)
        {
            lifeValueNormalized = lifeVal;
            bar.localScale = new Vector3(lifeValueNormalized, 1f);

            Transform barSprite = bar.Find("BarSprite");

            if (lifeValueNormalized > .9f)
            {
                barImage.GetComponent<Image>().color = perfetctHealth;
            }
            else if (lifeValueNormalized > .65f)
            {
                barImage.GetComponent<Image>().color = goodHealth;
            }
            else if (lifeValueNormalized > .35f)
            {
                barImage.GetComponent<Image>().color = mediumHealth;
            }
            else if (lifeValueNormalized > .1f)
            {
                barImage.GetComponent<Image>().color = lowHealth;
            }
            else
            {
                barImage.GetComponent<Image>().color = zeroHealth;
            }
        }
    }
}
