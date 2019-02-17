using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HistoryGrid : MonoBehaviour
{
    public DataController dataController;

    public GameObject historyCell01;
    public GameObject historyCell02;
    public GameObject historyCell03;
    public GameObject historyCell04;

    public string playerGender;
    public Player[] playerCharacter;

    public void AddCell(Dialogue dialogue, Choice choice, bool isChoice)
    {
        GameObject historyCell;
        dataController = GameObject.FindObjectOfType<DataController>();

        // Variant of History Cell
        if (!isChoice)
        {
            if (dialogue.isNarration)
            {
                historyCell = Instantiate(historyCell03, transform);

                // Content
                historyCell.transform.GetChild(0).gameObject.
                    GetComponent<TextMeshProUGUI>().text = dialogue.content;
            }
            else
            {
                if (dialogue.characterName.Equals("Ryleigh"))
                {
                    historyCell = Instantiate(historyCell02, transform);

                    if (dataController != null)
                    {
                        playerGender = dataController.currentSaveData.gender;
                        Debug.Log(playerGender);

                        if (playerGender.Equals("Male"))
                        {
                            historyCell.transform.GetChild(2).GetChild(0).
                                GetComponent<Image>().sprite = playerCharacter[0].characterThumb;
                            historyCell.transform.GetChild(2).GetChild(0).
                                GetComponent<Image>().color = Color.white;
                            historyCell.transform.GetChild(2).GetChild(1).
                                gameObject.SetActive(false);
                        }

                        if (playerGender.Equals("Female"))
                        {
                            historyCell.transform.GetChild(2).GetChild(0).
                                GetComponent<Image>().sprite = playerCharacter[1].characterThumb;
                            historyCell.transform.GetChild(2).GetChild(0).
                               GetComponent<Image>().color = Color.white;
                            historyCell.transform.GetChild(2).GetChild(1).
                                gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        if (playerGender.Equals("Male"))
                        {
                            historyCell.transform.GetChild(2).GetChild(0).
                                GetComponent<Image>().sprite = playerCharacter[0].characterThumb;
                            historyCell.transform.GetChild(2).GetChild(0).
                              GetComponent<Image>().color = Color.white;
                            historyCell.transform.GetChild(2).GetChild(1).
                                gameObject.SetActive(false);
                        }

                        if (playerGender.Equals("Male"))
                        {
                            historyCell.transform.GetChild(2).GetChild(0).
                                GetComponent<Image>().sprite = playerCharacter[1].characterThumb;
                            historyCell.transform.GetChild(2).GetChild(0).
                              GetComponent<Image>().color = Color.white;
                            historyCell.transform.GetChild(2).GetChild(1).
                                gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    historyCell = Instantiate(historyCell01, transform);

                    // Character Image
                    if (dialogue.characterThumb == null)
                    {
                        historyCell.transform.GetChild(2).GetChild(0).
                            GetComponent<Image>().color = Color.black;
                        historyCell.transform.GetChild(2).GetChild(1).
                            gameObject.SetActive(true);
                    }
                    else
                    {
                        historyCell.transform.GetChild(2).GetChild(0).
                            GetComponent<Image>().sprite = dialogue.characterThumb;
                        historyCell.transform.GetChild(2).GetChild(0).
                           GetComponent<Image>().color = Color.white;
                        historyCell.transform.GetChild(2).GetChild(1).
                            gameObject.SetActive(false);
                    }
                }

                // Character Name
                historyCell.transform.GetChild(0).GetChild(0).
                    GetComponent<TextMeshProUGUI>().text = dialogue.characterName;

                // Content
                historyCell.transform.GetChild(1).GetChild(0).
                   GetComponent<TextMeshProUGUI>().text = dialogue.content;
            }
        }
        else
        {
            historyCell = Instantiate(historyCell04, transform);

            historyCell.transform.GetChild(1).GetComponent
               <Image>().color = choice.color;
            historyCell.transform.GetChild(1).GetChild(0).
                   GetComponent<TextMeshProUGUI>().text = "0" + (choice.choiceId + 1).ToString();
            historyCell.transform.GetChild(2).GetComponent
                <TextMeshProUGUI>().text = choice.choice;
        }
    }
}
