using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    [Header("Data Controller")]
    public DataController dataController;

    [Header("Mask Effect")]
    public GameObject spriteMask;
    public GameObject genderPanel;
    public GameObject[] genderImages;
    public Button[] genderButtons;

    [Header("Transition")]
    public GameObject fadeTransition;

    private int nextScene;

    void Start()
    {
        // Get reference to Data Controller
        dataController = GameObject.FindObjectOfType<DataController>();

        // Play Sound
        GameObject.FindObjectOfType<AudioManager>().PlaySound("Hazy Darkness");
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (TouchPhase.Moved == touch.phase)
            {
                MoveMask(touch.position);
            }
        }
    }

    void MoveMask(Vector2 touchPosition)
    {
        Vector2 spritePosition = Camera.main.ScreenToWorldPoint(touchPosition);
        spriteMask.transform.position = spritePosition;
        spriteMask.SetActive(true);
    }

    public void StartGame()
    {
        StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("Hazy Darkness", 2f));
        GameObject.FindObjectOfType<AudioManager>().PlaySound("Amulet Absorption");

        if (dataController != null)
        {
            // If save is new, ask for player's gender
            // If not, go to main menu
            if (!dataController.currentSaveData.prologueFinish)
            {
                genderPanel.SetActive(true);

                dataController.currentSaveData.currentCutscene = 0;
                nextScene = 3;
            }
            else
            {
                nextScene = 4;
            }
        }
    }

    public void ChooseGender(int chosenGender)
    {
        GameObject.FindObjectOfType<AudioManager>().PlaySound("Button Click 2");
        StartCoroutine(ChangeGender(chosenGender));
    }

    IEnumerator ChangeGender(int chosenGender)
    {
        if (!string.IsNullOrEmpty(dataController.currentSaveData.gender))
        {
            if (chosenGender == 0)
            {
                genderButtons[1].GetComponent<Animator>().SetBool("Chosen", false);
                genderButtons[1].GetComponent<Animator>().SetBool("Exit", true);
                genderImages[1].GetComponent<Image>().color = Color.black;
                yield return new WaitForSeconds(1f);
            }

            if (chosenGender == 1)
            {
                genderButtons[0].GetComponent<Animator>().SetBool("Chosen", false);
                genderButtons[0].GetComponent<Animator>().SetBool("Exit", true);
                genderImages[0].GetComponent<Image>().color = Color.black;
                yield return new WaitForSeconds(1f);
            }
        }

        string gender = (chosenGender > 0) ? "Female" : "Male";
        dataController.currentSaveData.gender = gender;
        genderButtons[chosenGender].GetComponent<Animator>().SetBool("Chosen", true);
        genderImages[chosenGender].GetComponent<Image>().color = Color.white;

        Debug.Log(dataController.currentSaveData.gender);
    }

    public void ContinueGame()
    {
        dataController.SaveSaveData();
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(2f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", true);
        yield return new WaitForSeconds(2f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", false);
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(nextScene);
    }
}
