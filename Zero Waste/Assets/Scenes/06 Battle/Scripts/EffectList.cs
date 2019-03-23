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

            position++;
        }

        if (transform.childCount == 1)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void UpdateEffectDuration(Effect effect)
    {
        int position = 0;
        bool foundEffect = false;

        int i = 0;
        foreach (Effect effectOnList in effectsOnList)
        {
            if (effectOnList.GetInstanceID().Equals(effect.GetInstanceID()))
            {
                position = i;
                foundEffect = true;
                break;
            }

            i++;
        }

        Debug.Log(foundEffect);

        if (foundEffect)
        {
            i = 0;
            foreach (Transform cell in transform)
            {
                if (i == position)
                {
                    Debug.Log(cell.gameObject.name);
                        //GetComponent<TextMeshProUGUI>().text = effect.duration.ToString();
                    break;
                }

                i++;
            }
        }
    }
}
