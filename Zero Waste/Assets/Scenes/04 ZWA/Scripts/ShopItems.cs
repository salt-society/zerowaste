using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop Item", menuName = "Item/Shop Item")]
public class ShopItems : ScriptableObject
{
    [Header("Booster")]
    public Booster booster;

    [Header("Shop Specifics")]
    public int price;
    public int battleIDMod;
}
