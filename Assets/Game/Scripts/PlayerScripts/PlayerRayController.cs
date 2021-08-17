using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRayController : MonoBehaviour
{
    RaycastHit hit;
    Ray shootRay;
    StarshipController starshipContr;

    // Start is called before the first frame update
    void Start()
    {
        starshipContr = GameObject.FindGameObjectWithTag("GameController").GetComponent<StarshipController>();
    }

    // Update is called once per frame
    void Update()
    {
        RayShoot();
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
                starshipContr.openScreem = true;
            }
            else
            {
                starshipContr.openScreem = false;
            }
        }
    }
}
