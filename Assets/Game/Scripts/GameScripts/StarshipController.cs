using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarshipController : MonoBehaviour
{
    public float checkDis;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        ToHideCursor();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            if(Vector3.Distance(transform.position, player.transform.position) < checkDis)
            {
                player.GetComponent<PlayerRayController>().canOpen = true;
            }
            else
            {
                player.GetComponent<PlayerRayController>().canOpen = false;
            }
        }
    }

    void ToHideCursor()
    {
        //�������
        //Cursor.visible = false;
        //�������
        Cursor.lockState = CursorLockMode.Locked;
    }
}
