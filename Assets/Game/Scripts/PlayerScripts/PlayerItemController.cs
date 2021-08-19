using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    [Header("±³°ü")]
    public BagController bagController ;
    ItemController item;
    PlayerSkills playerSkills;
    PlayerController playerController;

    [Header("ÖÓ±í")]
    public float shootVal;

    // Start is called before the first frame update
    void Start()
    {
        playerSkills = GetComponent<PlayerSkills>();
        playerController = GetComponent<PlayerController>();

        bagController.itemList.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddNewItem(ItemController item)
    {
        if(!bagController.itemList.Contains(item))
        {
            bagController.itemList.Add(item);
            InventoryManager.CreateItem(item);
        }
        else
        {
            item.itemHeld += 1;
            InventoryManager.RefreshItem(item);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Item"))
        {
            //itemType = other.gameObject.GetComponent<ItemController>().type;
            AddNewItem(other.gameObject.GetComponent<Item>().item);
            CheckItem(other.gameObject.GetComponent<Item>().item.type);
            Destroy(other.gameObject);
        }
    }

    void CheckItem(int index)
    {
        switch(index)
        {
            case 0:
                Clock();
                break;
            case 1:
                Knife();
                break;
        }
    }

    void Clock()
    {
        playerSkills.shootCDTime -= shootVal;
    }

    void Knife()
    {

    }
}
