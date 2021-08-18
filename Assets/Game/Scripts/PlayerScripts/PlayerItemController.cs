using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    public int itemCount;

    PlayerSkills playerSkills;
    PlayerController playerController;
    int itemType;
    int[] itemCountArray;

    // Start is called before the first frame update
    void Start()
    {
        playerSkills = GetComponent<PlayerSkills>();
        playerController = GetComponent<PlayerController>();
        itemCountArray = new int[itemCount];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetItem(int type)
    {
        itemCountArray[type] += 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Item"))
        {
            itemType = other.gameObject.GetComponent<ItemController>().type;
            SetItem(itemType);
            Destroy(other.gameObject);
        }
    }
}
