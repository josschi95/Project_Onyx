using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Item Database", menuName = "Databases/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public Item[] items;
    //public Dictionary<Item, int> GetId = new Dictionary<Item, int>();
    public Dictionary<int, Item> GetItem = new Dictionary<int, Item>();

    public void AssignIDs()
    {
        //GetId = new Dictionary<Item, int>();
        GetItem = new Dictionary<int, Item>();
        for (int i = 0; i < items.Length; i++)
        {
            //GetId.Add(items[i], i);
            GetItem.Add(i, items[i]);
            items[i].itemID = i;
        }
    }

    //Removes Duplicates
    public void CleanUpList()
    {
        var tempList = new List<Item>();

        //Remove Duplicates
        for (int i = 0; i < items.Length; i++)
        {
            if (!tempList.Contains(items[i]))
            {
                tempList.Add(items[i]);
            }
        }

        //Order alphabetically
        tempList.OrderBy(x => x.name).ToList();

        //Place back into array
        items = new Item[tempList.Count];
        for (int i = 0; i < tempList.Count; i++)
        {
            items[i] = tempList[i];
        }
    }
}
