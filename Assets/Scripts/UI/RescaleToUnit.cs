using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RescaleToUnit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        // trovo gli attuali valori del rect transform
        RectTransform rt = gameObject.GetComponent<RectTransform>();
        float width = rt.sizeDelta.x;
        float height = rt.sizeDelta.y;
        float scaleFactor = rt.localScale.x; // x e y sono uguali

        // calcolo la width e height se la scale fosse a uno
        float correctWidth = width * scaleFactor;
        float correctHeight = height * scaleFactor;

        // setto i valori corretti sulla componente CanvasScaler
        gameObject.GetComponent<CanvasScaler>().referenceResolution = new Vector2(correctWidth, correctHeight);
        

    }
    
}
