using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInstanceHandler : MonoBehaviour
{
    private ShopItems heldItem;

    private GameObject shop;

    public void SetItem(ShopItems item)
    {
        heldItem = item;

        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = heldItem.booster.icon;

        gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = heldItem.booster.name;
    }

    public void SendItem()
    {
        shop.GetComponent<ShopController>().ShowItemDescription(heldItem);
    }

    private void Awake()
    {
        shop = GameObject.Find("Shop Screen");
    }
}
