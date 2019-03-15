using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EffectList : MonoBehaviour
{
    public GameObject effectCellPrefab;
    public List<Sprite> effectsOrigin;


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
        // Disble no status effect label when adding effects on list
        if (transform.childCount == 1)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        // Instantiate effectCell prefab
        GameObject newEffectCell = Instantiate(effectCellPrefab, transform);

        // Effect Icon
        newEffectCell.transform.GetChild(1).GetChild(1).
            GetComponent<Image>().sprite = effect.effectIcon;

        // Duration
        newEffectCell.transform.GetChild(3).GetChild(0).gameObject.
           GetComponent<TextMeshProUGUI>().text = effect.effectDuration.ToString();

        // Name
        newEffectCell.transform.GetChild(2).gameObject.
            GetComponent<TextMeshProUGUI>().text = effect.effectName;

        // Description
        newEffectCell.transform.GetChild(4).gameObject.
            GetComponent<TextMeshProUGUI>().text = effect.effectDescription;

        // Origin
        if (originOfEffect != null)
        {
            newEffectCell.transform.GetChild(5).GetChild(1).gameObject.
                GetComponent<Image>().sprite = originOfEffect;
            newEffectCell.transform.GetChild(5).GetChild(1).gameObject.SetActive(true);
        }
        
    }

    // <summary>
    // Remove all effects
    // </summary>
    public void RemoveAllEffects()
    {
        if (transform.childCount > 1)
        {
            foreach (Transform cell in transform)
            {
                if (transform.childCount != 1)
                {
                    Destroy(cell);
                }
            }
        }
        
        transform.GetChild(0).gameObject.SetActive(true);
    }

    // <summary>
    // Remove one effect through effect name
    // </summary>
    public void RemoveEffect(Effect effect)
    {
        foreach (Transform cell in transform)
        {
            string effectName = cell.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text;
            if (effectName.Equals(effect.effectName))
            {
                Destroy(cell);
                break;
            }
        }

        if (transform.childCount == 1)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    // <summary>
    // Remove one effect through effect name
    // </summary>
    public void RemoveEffect(int effectPosition)
    {
        int position = 0;
        foreach (Transform cell in transform)
        {
            if (position == effectPosition)
            {
                Destroy(cell);
                break;
            }
        }

        if (transform.childCount == 1)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void UpdateEffectDuration(int duration, int effectPosition)
    {
        int position = 0;
        foreach (Transform cell in transform)
        {
            if (position == effectPosition)
            {
                cell.GetChild(3).GetChild(0).gameObject.
                    GetComponent<TextMeshProUGUI>().text = duration.ToString();
                break;
            }
        }
    }
}
