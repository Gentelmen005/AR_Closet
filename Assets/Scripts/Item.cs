using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public enum ItemType
{
	Box,
	Test
}

public class Item : MonoBehaviour
{
    public ItemType type;
    public bool isBlocked;
    bool flag = false;
    public Item[] intersectingItems;
    public Item[] DrawerClose;

    public void Interaction()
    {
        if (type == ItemType.Box && !isBlocked)
        {
            flag = !flag;
            GetComponentInParent<Animator>().SetBool("Open", flag);
            GetComponentInParent<Animator>().SetBool("Close", !flag);

            for (int i = 0; i < intersectingItems.Length; i++)
            {
                intersectingItems[i].isBlocked = flag;
            }

            for (int i = 0; i < DrawerClose.Length; i++)
            {
                DrawerClose[i].isBlocked = !flag;
            }
        }
    }
    public bool IsOpen()
    {
        return flag;
    }

}
