using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxController : MonoBehaviour
{
    public GameObject priceTag;
    public GameObject box;
    public GameObject item;
    public int price;
    public int boxType;
    public float priceApearDis;
    public float buyingDis;
    public List<ItemController> itemControllers = new List<ItemController>();

    GameObject player;
    GameObject buying;
    bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
        priceTag.GetComponent<Text>().text = price + "";
        player = GameObject.FindGameObjectWithTag("Player");
        buying = GameObject.FindGameObjectWithTag("GameController").GetComponent<PropController>().buying;
        //Ëæ»ú·½Ïò
        var rotation = Quaternion.Euler(Random.Range(0f, 30f), Random.Range(0, 360f), Random.Range(0f, 30f));
        box.transform.rotation = rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isOpen)
        {
            CheckPlayerDis();
        }
        else
        {
            OpenBox();
            priceTag.SetActive(false);
            buying.SetActive(false);
            Destroy(this);
        }
    }

    void CheckPlayerDis()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < priceApearDis)
        {
            priceTag.SetActive(true);
            if (Vector3.Distance(player.transform.position, transform.position) < buyingDis)
            {
                buying.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (player.GetComponent<PlayerSkills>().energy > price)
                    {
                        player.GetComponent<PlayerSkills>().energy -= price;
                        isOpen = true;
                    }
                }
            }
            else
            {
                buying.SetActive(false);
            }

        }
        else
        {
            priceTag.SetActive(false);
        }
    }

    void OpenBox()
    {
        var pos = Vector3.Normalize(player.transform.position - transform.position);
        var tempItem = Instantiate(item, pos * 2 + transform.position, Quaternion.identity);
        var temp = Random.Range(0, itemControllers.Count);
        tempItem.GetComponent<Item>().item = itemControllers[temp];
    }
}
