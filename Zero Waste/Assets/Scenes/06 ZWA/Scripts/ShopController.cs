using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopController : MonoBehaviour
{
    #region Editor Variables

    [Header("UI Components")]

    public List<GameObject> partHighlights;
    public List<GameObject> partNames;

    [Header("Shelf Components")]
    public GameObject shelf;
    public GameObject scrollview;
    public GameObject itemPrefab;

    [Header("Item Component")]
    public GameObject itemInfo;

    [Header("Shop Items")]
    public ShopItems[] shopItems;

    #endregion

    #region Private Variables

    private DataController dataController;

    int currentQuantity;

    ShopItems currentItem;

    #endregion

    IEnumerator DisplayPartNames()
    {
        foreach (GameObject tooltip in partNames)
            tooltip.SetActive(!tooltip.activeInHierarchy);

        yield return new WaitForSeconds(1.0f);

        foreach (GameObject tooltip in partNames)
            tooltip.GetComponent<Animator>().SetBool("Hide", true);

        yield return new WaitForSeconds(1.0f);

        foreach (GameObject tooltip in partNames)
        {
            tooltip.SetActive(!tooltip.activeInHierarchy);
            tooltip.GetComponent<Animator>().SetBool("Hide", false);
        }
    }

    public void ShowShop()
    {
        shelf.SetActive(true);

        ShowStock();
    }

    public void HideShop()
    {
        shelf.SetActive(false);
    }

    public void ShowItemDescription(ShopItems item)
    {
        currentItem = item;

        SetItemDescription();
    }

    public void HideItemDescription()
    {
        itemInfo.SetActive(false);

        currentQuantity = 1;
        currentItem = null;
    }

    public void ChangeQuantity()
    {
        string name = EventSystem.current.currentSelectedGameObject.name;

        Debug.Log(name);

        switch(name)
        {
            case "Add":
                currentQuantity++;
                break;

            case "Minus":
                if (currentQuantity > 1)
                    currentQuantity--;
                break;
        }

        itemInfo.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = currentQuantity.ToString();
        itemInfo.transform.GetChild(10).GetChild(1).GetComponent<TextMeshProUGUI>().text = (currentQuantity * currentItem.price).ToString();
    }

    public void BuyItem()
    {
        int totalPrice = currentQuantity * currentItem.price;

        if(dataController.currentSaveData.scraps >= totalPrice)
        {
            dataController.UseScrap(totalPrice);

            dataController.currentSaveData.AddBooster(currentItem.booster.boosterId, currentQuantity);

            dataController.SaveSaveData();
            dataController.SaveGameData();

            currentQuantity = 1;
            SetItemDescription();
        }

        else
        {
            itemInfo.transform.GetChild(11).gameObject.SetActive(true);
            itemInfo.transform.GetChild(11).GetChild(0).GetComponent<TextMeshProUGUI>().text = "NOT ENOUGH SCRAPS!";
        }
    }

    private void SetItemDescription()
    {
        itemInfo.SetActive(true);
        itemInfo.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = currentItem.booster.boosterName;
        itemInfo.transform.GetChild(1).GetComponent<Image>().sprite = currentItem.booster.icon;
        itemInfo.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = currentItem.price.ToString();
        itemInfo.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = currentItem.booster.description;
        itemInfo.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "1";
        itemInfo.transform.GetChild(10).GetChild(1).GetComponent<TextMeshProUGUI>().text = (currentQuantity * currentItem.price).ToString();
        itemInfo.transform.GetChild(11).gameObject.SetActive(false);
    }

    private void ShowStock()
    {
        int currentNodeId = dataController.currentSaveData.currentNodeId;

        ClearStock();

        for (int CTR = 0; CTR < shopItems.Length; CTR++)
        {
            if(shopItems[CTR].battleIDMod <= currentNodeId)
            {
                GameObject instancePrefab = Instantiate(itemPrefab, scrollview.transform);
                instancePrefab.GetComponent<ItemInstanceHandler>().SetItem(shopItems[CTR]);
            }
        }
    }

    private void ClearStock()
    {
        if (scrollview.transform.childCount > 0)
        {
            foreach (Transform child in scrollview.transform)
                Destroy(child.gameObject);
        }
    }

    private void Awake()
    {
        dataController = FindObjectOfType<DataController>();

        currentQuantity = 1;
        currentItem = null;
    }

    private void Start()
    {
        StartCoroutine(DisplayPartNames());
    }
}
