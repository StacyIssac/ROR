using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRayController : MonoBehaviour
{
    public GameObject screem;
    public GameObject EText;

    [HideInInspector]
    public bool canOpen = false;

    RaycastHit hit;
    Ray shootRay;
    bool openScreem = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RayShoot();

        if(openScreem && canOpen)
        {
            OpenScreem();
        }
    }

    void RayShoot()
    {
        Vector2 point = new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        shootRay = Camera.main.ScreenPointToRay(point);
        Physics.Raycast(shootRay, out hit);

        if (hit.transform != null)
        {
            if (hit.transform.CompareTag("Screem"))
            {
                openScreem = true;
            }
        }
    }

    void OpenScreem()
    {
        EText.SetActive(true);

        if(Input.GetKeyDown(KeyCode.E))
        {
            screem.SetActive(true);
        }
    }
}
