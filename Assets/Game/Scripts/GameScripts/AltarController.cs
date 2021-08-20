using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarController : MonoBehaviour
{
    public float openDis;
    public GameObject openImage;
    bool canOpen = true;
    GameObject player;
    GameObject gameController;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameController = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        if(canOpen)
        {
            CheckPlayerDis();
        }
        else
        {
            openImage.SetActive(false);
        }
    }

    void CheckPlayerDis()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < openDis)
        {
            openImage.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                gameController.GetComponent<EnemyController>().CreateEnermy();
                canOpen = false;
            }

        }
        else
        {
            openImage.SetActive(false);
        }
    }
}
