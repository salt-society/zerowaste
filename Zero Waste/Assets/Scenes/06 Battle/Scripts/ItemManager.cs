using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemManager : MonoBehaviour
{
    private DataController dataController;
    private CharacterManager characterManager;
    private ParticleManager particleManager;
    private StatusManager statusManager;

    private GameObject[] scavObjs;
    private int currentScav;

    [Space]
    public GameObject boosterPanel;
    public GameObject noBoosterMessage;

    [Space]
    public GameObject[] boosters;

    void Start()
    {
        dataController = FindObjectOfType<DataController>();
        characterManager = FindObjectOfType<CharacterManager>();
        particleManager = FindObjectOfType<ParticleManager>();
        statusManager = FindObjectOfType<StatusManager>();

        GetScavengerObjects();
        SetBoosters();
    }

    public void GetScavengerObjects()
    {
        scavObjs = characterManager.GetAllCharacterPrefabs(1);
    }

    public void SetBoosters()
    {
        if (dataController != null)
        {
            if (dataController.boosters.Length == 0)
            {
                noBoosterMessage.SetActive(true);
            }
            else
            {
                noBoosterMessage.SetActive(false);

                int i = 0;
                foreach (Booster booster in dataController.boosters)
                {
                    boosters[i].GetComponent<Image>().sprite = booster.icon;
                    boosters[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = booster.boosterName;
                    boosters[i].SetActive(true);
                    i++;
                }
            }
            
        }
    }

    public void SetCurrentScavenger(int position)
    {
        currentScav = position;
    }

    public void ShowBoosterPanel()
    {
        boosterPanel.SetActive(!boosterPanel.activeInHierarchy);
    }

    public void UseBooster(int boosterNo)
    {
        boosters[boosterNo].SetActive(false);
        ShowBoosterPanel();

        Debug.Log("Booster: " + dataController.boosters[boosterNo].boosterName);
        StartCoroutine(ApplyBooster(Instantiate(dataController.boosters[boosterNo])));
    }

    IEnumerator ApplyBooster(Booster booster)
    {
        particleManager.PlayParticles(booster.particleIndex, scavObjs[currentScav].transform.position);
        yield return new WaitForSeconds(1f);

        foreach (Effect effect in booster.effects)
        {
            if (effect.type.Equals("Direct"))
            {
                int valueChanged = scavObjs[currentScav].GetComponent<CharacterMonitor>().ScavengerHealed(effect.target, effect.strength);

                // HP - Skill, Ult | ANT - Charge, Skill, Ult
                if (effect.target.Equals("HP"))
                {
                    CharacterMonitor scavMonitor = scavObjs[currentScav].GetComponent<CharacterMonitor>();
                    StartCoroutine(statusManager.IncrementHealthBar(scavMonitor.CurrentHealth, scavMonitor.MaxHealth, scavMonitor.Position));
                    StartCoroutine(statusManager.ShowValues(valueChanged.ToString(), booster.type, scavObjs[currentScav], booster.particleIndex));
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    CharacterMonitor scavMonitor = scavObjs[currentScav].GetComponent<CharacterMonitor>();
                    StartCoroutine(statusManager.IncrementAntidoteBar(scavMonitor.CurrentAnt, scavMonitor.MaxAnt, scavMonitor.Position));
                    StartCoroutine(particleManager.PlayParticles(effect.particleIndex, scavObjs[currentScav].transform.position));

                    StartCoroutine(statusManager.ShowValues(valueChanged.ToString(), booster.type, scavObjs[currentScav], booster.particleIndex));

                    yield return new WaitForSeconds(1f);
                }
            }
            else
            {
                // Buff Scavengers
                scavObjs[currentScav].GetComponent<CharacterMonitor>().ScavengerBuffed(Instantiate(effect));

                if (effect.application.Equals("CharStats"))
                {
                    scavObjs[currentScav].GetComponent<CharacterMonitor>().EffectsAnimation(4, effect);
                    StartCoroutine(statusManager.ShowBuff(scavObjs[currentScav], effect.state));
                    yield return new WaitForSeconds(1f);
                }

                if (effect.application.Equals("Condition"))
                {
                    scavObjs[currentScav].GetComponent<CharacterMonitor>().EffectsAnimation(4, effect);
                    StartCoroutine(particleManager.PlayParticles(effect.particleIndex, scavObjs[currentScav].transform.position));
                    yield return new WaitForSeconds(1f);
                }

                statusManager.AddEffectToStatusPanel("Scavenger", scavObjs[currentScav].GetComponent<CharacterMonitor>().Position, effect);
                statusManager.AddEffectsToStatusList("Scavenger", scavObjs[currentScav].GetComponent<CharacterMonitor>().Position, effect, booster.icon);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
