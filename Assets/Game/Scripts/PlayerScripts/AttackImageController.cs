using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackImageController : MonoBehaviour
{
    Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(image.color.a > 0)
        {
            image.color -= new Color(0, 0, 0, 0.005f);
        }
        else
        {
            //image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }
    }
}
