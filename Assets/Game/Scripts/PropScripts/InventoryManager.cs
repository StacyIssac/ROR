using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    static InventoryManager instance;

    public BagController bag;
    public GameObject bagGrid;
    public Slot slot;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
        }

        instance = this;
    }

    public static void CreateItem(ItemController item)
    {
        Slot newItem = Instantiate(instance.slot, instance.bagGrid.transform.position, Quaternion.identity);
        newItem.gameObject.transform.SetParent(instance.bagGrid.transform);
        newItem.item = item;
        newItem.image.sprite = item.itemImage;
        item.itemHeld = 1;
        newItem.text.text = item.itemHeld.ToString();
    }

    public static void RefreshItem(ItemController item)
    {
        for(int i = 0; i < instance.bag.itemList.Count; i++)
        {
            if(instance.bag.itemList[i].name.Equals(item.name))
            {
                instance.bagGrid.transform.GetChild(i).GetComponent<Slot>().text.text = item.itemHeld.ToString();
            }
        }
    }
}
