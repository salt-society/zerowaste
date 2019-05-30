using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    [Space]
    public DataController dataController;

    [Space]
    public GameObject genderPanel;
    public GameObject[] genderImages;
    public Button[] genderButtons;
    public Button continueButton;
    public Color clickedColor;

    [Space]
    public GameObject privacyPolicyPanel;

    [Space]
    public GameObject fadeTransition;
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
                if (TouchPhase.Began == Input.GetTouch(0).phase)
                {
                    Vector2 origin = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero);

                    if (hit.collider == null)
                    {
                        canTouchScreen = false;
                        StartGame();
                    }
                }
            }
        }
    }

    public void ShowPrivacyPolicy()
    {
        privacyPolicyPanel.SetActive(!privacyPolicyPanel.activeInHierarchy);
        canTouchScreen = !canTouchScreen;
    }

    public void StartGame()
    {
        if (dataController == null)
            return;

        FindObjectOfType<AudioManager>().PlaySound("Crumpling Paper");
        if (dataController.testing)
        {
            // Unlock areas/nodes/battles
            dataController.currentSaveData.EncounteredMutant(0);
            dataController.currentSaveData.EncounteredMutant(1);
            dataController.currentSaveData.EncounteredMutant(2);

            dataController.currentSaveData.currentNodeId = 14;

            // Add scavengers to roster, default male
            dataController.currentSaveData.gender = "Male";
            AddDefaultScavengers("Male");
            Player newCharacter = ScriptableObject.CreateInstance<Player>();
            newCharacter = Instantiate(dataController.allScavengersList[8]);
            dataController.AddScavenger(newCharacter);
            dataController.AddScrap(1000);

            dataController.AddExp(dataController.scavengerRoster[0], 30);
            dataController.AddExp(dataController.scavengerRoster[1], 30);
            dataController.currentSaveData.currentCutsceneId = 1;

            // Scene testing
            dataController.nextScene = dataController.GetNextSceneId("ZWA");
            StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("RPG Theme Looping", 2f));
            StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("Burning", 2f));
            StartCoroutine(LoadScene());
        }
        else
        {
            // Check how many cutscenes are unlocked
            // No cutscenes = New Game
            if (dataController.currentSaveData.currentNodeId < 0)
            {
                // Get next scene id
                dataController.nextScene = dataController.GetNextSceneId("Cutscene");

                // Unlock prologue, set cutscene details
                dataController.currentSaveData.UnlockCutscene(0, false);

                // Show gender panel so player can choose which
                // main character he/she will play
                genderPanel.SetActive(true);
            }
            // If there is a cutscene unlocked, signifies the game has been started
            else
            {
                if (dataController.currentSaveData.currentNodeId >= 0)
                {
                    if (!dataController.currentSaveData.battleTutorial)
                    {
                        dataController.currentNode = dataController.allNodes[0];

                        dataController.nextScene = dataController.GetNextSceneId("Battle");
                        StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("RPG Theme Looping", 2f));
                        StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("Burning", 2f));
                        StartCoroutine(LoadScene());
                    }

                    else
                    {
                        dataController.nextScene = dataController.GetNextSceneId("ZWA");
                        StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("RPG Theme Looping", 2f));
                        StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("Burning", 2f));
                        StartCoroutine(LoadScene());
                    }
                }

                else
                {
                    StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("RPG Theme Looping", 2f));
                    StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("Burning", 2f));

                    dataController.nextScene = dataController.GetNextSceneId("Cutscene");
                    dataController.currentSaveData.UnlockCutscene(0, false);
                    StartCoroutine(LoadScene());
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
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", true);
        yield return new WaitForSeconds(1f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", false);
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(dataController.GetNextSceneId("Loading"));
    }
}
