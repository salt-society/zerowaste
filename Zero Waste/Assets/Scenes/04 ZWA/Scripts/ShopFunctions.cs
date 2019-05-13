using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopFunctions : MonoBehaviour
{
    [Header("Shop Items")]
    public ShopItems[] ShopItems;

    private DataController myController;

    // Start is called before the first frame update
    void Start()
    {
        myController = FindObjectOfType<DataController>();
    }

    // Display all available items
    public List<ShopItems> DisplayAvailableShopItems()
    {
        int currentID = myController.currentSaveData.currentBattleId;

        List<ShopItems> availableList = new List<ShopItems>();

        foreach (ShopItems item in ShopItems)
        {
            if (item.battleIDMod <= currentID)
                availableList.Add(item);
        }

        return availableList;
    }

    // Function after clicking the buy button
    public void BuyBoosters(int price, int quantity, Booster booster)
    {
        // Calculate the total price
        int total = price * quantity;

        // Reduce the scrap
        myController.currentSaveData.UseScrap(total);

        // Add to Inventory

        // Save data here
    }
}
