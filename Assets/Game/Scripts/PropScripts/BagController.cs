using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bag", menuName = "Inventory/New Bag")]

public class BagController : ScriptableObject
{
    public List<ItemController> itemList = new List<ItemController>();
}
