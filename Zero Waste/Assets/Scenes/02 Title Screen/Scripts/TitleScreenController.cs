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

    private int nextSceneId;

    void Start()
    {
        // Get reference to Data Controller
        dataController = GameObject.FindObjectOfType<DataController>();
        if (dataController != null)
        {
           
        }

        // Play Sound
        GameObject.FindObjectOfType<AudioManager>().PlaySound("Hazy Darkness");
    }

    void Update()
    {
        /*if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (TouchPhase.Moved == touch.phase)
            {
                MoveMask(touch.position);
            }
        }*/
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
            int currentCutscene = dataController.currentSaveData.currentCutscene;
            string nextLevel = dataController.currentSaveData.nextLevel;
            
            nextSceneId = dataController.currentGameData.NextSceneId(nextLevel);

            if (nextSceneId == 3 && currentCutscene == 0)
            {
                genderPanel.SetActive(true);
            }
            else
            {
                genderPanel.SetActive(true);
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
                yield return new WaitForSeconds(0.5f);
            }

            if (chosenGender == 1)
            {
                genderButtons[0].GetComponent<Animator>().SetBool("Chosen", false);
                genderButtons[0].GetComponent<Animator>().SetBool("Exit", true);
                genderImages[0].GetComponent<Image>().color = Color.black;
                yield return new WaitForSeconds(0.5f);
            }
        }

        string gender = (chosenGender > 0) ? "Female" : "Male";
        dataController.currentSaveData.gender = gender;
        AddDefaultScavengers(gender);
        
        genderButtons[chosenGender].GetComponent<Animator>().SetBool("Chosen", true);
        genderImages[chosenGender].GetComponent<Image>().color = Color.white;
    }

    public void AddDefaultScavengers(string gender)
    {
        Player scavenger01 = ScriptableObject.CreateInstance<Player>();
        Player scavenger02 = ScriptableObject.CreateInstance<Player>();
        dataController.scavengerRoster = new List<Player>();

        if (gender.Equals("Male"))
        {
            scavenger01 = Instantiate(dataController.allScavengersList[0]);
            scavenger01.name = "Ryleigh";

            scavenger02 = Instantiate(dataController.allScavengersList[3]);
            scavenger02.name = "Paige";
        }

        if (gender.Equals("Female"))
        {
            scavenger01 = Instantiate(dataController.allScavengersList[1]);
            scavenger01.name = "Ryleigh";

            scavenger02 = Instantiate(dataController.allScavengersList[2]);
            scavenger02.name = "Paige";
        }

        dataController.AddScavenger(scavenger01);
        dataController.AddScavenger(scavenger02);
    }

    public void ContinueGame()
    {
        GameObject.FindObjectOfType<AudioManager>().PlaySound("Button Click 2");

        dataController.SaveScavengers();
        dataController.SaveSaveData();
        dataController.SaveGameData();
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(2f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", true);
        yield return new WaitForSeconds(2f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", false);
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(nextSceneId);
    }
}
