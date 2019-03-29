using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EffectList : MonoBehaviour
{
    public GameObject effectCellPrefab;
    public List<Sprite> effectsOrigin;
    private List<Effect> effectsOnList;

    // <summary>
    // Add array of effects on list
    // </summary>
    public void AddEffects(Effect[] effects, Sprite originOfEffect)
    {
        foreach (Effect effect in effects)
        {
            AddEffect(effect, originOfEffect);
        }
    }

    // <summary>
    // Add one effect
    // </summary>
    public void AddEffect(Effect effect, Sprite originOfEffect)
    {
        if (effectsOnList == null)
        {
            effectsOnList = new List<Effect>();
            effectsOnList.Add(effect);
        }
        else
        {
            effectsOnList.Add(effect);
        }

        // Disble no status effect label when adding effects on list
        if (transform.childCount == 1)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        // Instantiate effectCell prefab
        GameObject newEffectCell = Instantiate(effectCellPrefab, transform);

        // Effect Icon
        newEffectCell.transform.GetChild(1).GetChild(1).
            GetComponent<Image>().sprite = effect.icon;

        // Duration
        newEffectCell.transform.GetChild(3).GetChild(0).gameObject.
           GetComponent<TextMeshProUGUI>().text = effect.duration.ToString();

        // Name
        newEffectCell.transform.GetChild(2).gameObject.
            GetComponent<TextMeshProUGUI>().text = effect.effectName;

        // Description
        newEffectCell.transform.GetChild(4).gameObject.
            GetComponent<TextMeshProUGUI>().text = effect.description;

        // Origin
        if (originOfEffect != null)
        {
            newEffectCell.transform.GetChild(5).GetChild(1).gameObject.
                GetComponent<Image>().sprite = originOfEffect;
            newEffectCell.transform.GetChild(5).GetChild(1).gameObject.SetActive(true);
        }
        
    }

    // <summary>
    // Remove one effect through effect position
    // </summary>
    public void RemoveEffect(int effectNo)
    {
        int i = 0;
        foreach (Transform effectCell in transform)
        {
            if (i == (effectNo + 1))
            {
                Destroy(effectCell.gameObject);
                break;
            }
            i++;
        }

        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void UpdateEffect(int effectNo, string duration)
    {
        int i = 0;
        foreach (Transform effectCell in transform)
        {
            if (i == (effectNo + 1))
            {
                effectCell.gameObject.transform.GetChild(3).GetChild(0).
                    GetComponent<TextMeshProUGUI>().text = duration;

                break;
            }
            i++;
        }
    }
}
