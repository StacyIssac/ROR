using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/New Item")]

public class ItemController : ScriptableObject
{
    public int type;
    public string itemName;
    public Sprite itemImage;
    public GameObject itemModel;
    public int itemHeld = 1;
    [TextArea]
    public string itemInformation;
}
