using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarshipController : MonoBehaviour
{
    public float checkDis;
    public GameObject panel1;
    public GameObject panel2;
    public GameObject screem;
    public GameObject EText;
    public GameObject cam;

    GameObject player;
    [HideInInspector]
    public bool openScreem = false;
    bool canOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        ToHideCursor();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (openScreem && canOpen)
        {
            OpenScreem();
        }
        else
        {
            EText.SetActive(false);
        }

        if (player != null)
        {
            if(Vector3.Distance(transform.position, player.transform.position) < checkDis)
            {
                canOpen = true;
            }
            else
            {
                canOpen = false;
            }
        }

        if(screem.active)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                screem.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                cam.SetActive(true);
                player.GetComponent<PlayerController>().enabled = true;
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

    public void Button1()
    {
        panel1.SetActive(true);
        panel2.SetActive(false);
    }

    public void Button2()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
    }

    void OpenScreem()
    {
        EText.SetActive(true);

        if (Input.GetKeyDown(KeyCode.E))
        {
            screem.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            cam.SetActive(false);
            player.GetComponent<PlayerController>().enabled = false;
        }
    }
}
