using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    [Header("????")]
    public BagController bagController ;
    ItemController item;
    PlayerSkills playerSkills;
    PlayerController playerController;

    [Header("?ӱ?")]
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
            case 2:
                Chocolate();
                break;
            case 3:
                AirWall();
                break;
        }
    }

    void Clock()
    {
        playerSkills.SetShootCDTime(0);
    }

    void Knife()
    {
        playerSkills.SetCritChance(0);
    }

    void Chocolate()
    {
        playerSkills.SetSpeed(0);
        playerController.SetSpeed(0);
    }

    void AirWall()
    {
        playerController.SetJumpCount();
    }

    void Scopes()
    {
        playerSkills.SetDissolve(0);
    }

    void Branch()
    {
        playerSkills.canCheckEnemyState = true;
    }

    void Cat()
    {
        playerSkills.SetEnergyAdd(0);
    }

    void Breakf()
    {
        playerSkills.SetHP(0);
    }
}
