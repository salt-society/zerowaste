using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    [Header("Data Controller")]
    public DataController dataController;

    [Header("Gender Selection")]
    public GameObject genderPanel;
    public GameObject[] genderImages;
    public Button[] genderButtons;
    public Button continueButton;

    [Space]
    public Color clickedColor;

    [Header("Transition")]
    public GameObject fadeTransition;

    private int nextSceneId;
    private bool canTouchScreen;

    void Start()
    {
        // Get reference to Data Controller
        dataController = GameObject.FindObjectOfType<DataController>();
        if (dataController != null)
        {
            // Play Sound
            GameObject.FindObjectOfType<AudioManager>().PlaySound("RPG Theme Looping");
            GameObject.FindObjectOfType<AudioManager>().PlaySound("Burning");

            // Disable continue button
            continueButton.enabled = false;
            canTouchScreen = true;

        }
    }

    void Update()
    {
        if (canTouchScreen)
        {
            if (Input.touchCount == 1)
            {
                canTouchScreen = false;
                StartGame();
            }
        }
    }

    public void StartGame()
    {
        GameObject.FindObjectOfType<AudioManager>().PlaySound("Crumpling Paper");

        if (dataController != null)
        {
            // Check how many cutscenes are unlocked
            // No cutscenes = New Game
            int cutsceneId = dataController.currentSaveData.currentCutsceneId;
            if (cutsceneId == -1)
            {
                // Get next scene id
                nextSceneId = dataController.GetNextSceneId("Cutscene");

                // Unlock prologue, set cutscene details
                dataController.currentSaveData.UnlockCutscene(0, false);

                // Then save changes made in save file
                dataController.SaveSaveData();
                dataController.SaveGameData();

                // Show gender panel so player can choose which
                // main character he/she will play
                genderPanel.SetActive(true);
            }
            // If there is a cutscene unlocked, signifies the game have been started
            else
            {
                // Check again how many cutscenes are unlocked
                // If its not more than one (3 if game is complete), check if that cutscene is finished
                // because most likely the very first scene is Prologue
                if (dataController.currentSaveData.cutscenes.Count > 1)
                {
                    // NOTE: INSET CHECKING OF TUTORIALS HERE

                    // Since tutorials and ZWA are not yet done, temporary
                    // go straight to Map
                    nextSceneId = dataController.GetNextSceneId("Map");
                    StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("RPG Theme Looping", 2f));
                    StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("Burning", 2f));
                    StartCoroutine(LoadScene());
                }
                else
                {
                    // Check if Prologue is finished
                    if (dataController.currentSaveData.cutscenes[0])
                    {
                        // NOTE: INSET CHECKING OF TUTORIALS HERE

                        // Since tutorials and ZWA are not yet done, temporary
                        // go straight to Map
                        /*nextSceneId = dataController.GetNextSceneId("Map");
                        StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("RPG Theme Looping", 2f));
                        StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("Burning", 2f));
                        StartCoroutine(LoadScene());*/

                        // Turn of Title Screen BGM
                        StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("RPG Theme Looping", 2f));
                        StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("Burning", 2f));

                        nextSceneId = dataController.GetNextSceneId("Cutscene");
                        dataController.currentSaveData.UnlockCutscene(0, false);
                        StartCoroutine(LoadScene());
                    }
                    // If not, repeat Prologue Cutscene
                    else
                    {
                        // Turn of Title Screen BGM
                        StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("RPG Theme Looping", 2f));
                        StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("Burning", 2f));

                        nextSceneId = dataController.GetNextSceneId("Cutscene");
                        dataController.currentSaveData.UnlockCutscene(0, false);
                        StartCoroutine(LoadScene());
                    }
                }
            }
        }
    }

    public void ChooseGender(int chosenGender)
    {
        // Just to be safe, always check if there's a data controller before execution
        if (dataController != null)
        {
            // Play SFX
            GameObject.FindObjectOfType<AudioManager>().PlaySound("Click 01");

            // Gender button animation
            StartCoroutine(ChangeGender(chosenGender));

            // Enable continue button
            continueButton.enabled = true;
        } 
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
        genderImages[chosenGender].GetComponent<Image>().color = new Color(255, 255, 255, .10f);
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
        // Just to be safe, always check if there's a data controller before execution
        if(dataController != null) 
        {
            // Disable button to avoid multiple calls of function
            continueButton.enabled = false;
            continueButton.GetComponent<Image>().color = clickedColor;

            // Stop SFX
            StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("RPG Theme Looping", 2f));
            StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("Burning", 2f));

            // Save progress
            dataController.SaveScavengers();
            dataController.SaveSaveData();
            dataController.SaveGameData();

            // Load next scene
            StartCoroutine(LoadScene());
        }
    }

    public void ContinueSFX()
    {
        if (dataController != null)
        {
            GameObject.FindObjectOfType<AudioManager>().PlaySound("Crumpling Paper");
        }
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
