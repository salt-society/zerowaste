using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : AreaController
{
    [Header("Shop Items")]
    public ShopItems[] ShopItems;


    // Display all available items
    public List<ShopItems> DisplayAvailableShopItems()
    {
        int currentID = dataController.currentSaveData.currentBattleId;

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
        dataController.currentSaveData.UseScrap(total);

        // Add to Inventory

        // Save data here
    }
}
