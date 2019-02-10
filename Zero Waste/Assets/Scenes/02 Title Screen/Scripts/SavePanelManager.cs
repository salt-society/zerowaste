using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavePanelManager : MonoBehaviour
{
    [Header("Save Components")]
    public GameObject savePanel;
    public GameObject saveResultPanel;

    [Space]
    public Button[] saveButtons;

    [Space]
    public Color currentSaveColor;
    public Color enabledColor;
    public Color disabledColor;

    [Header("Data Controller")]
    public DataController dataController;

    private List<SaveData> saves;
    private bool savePanelOpen;

    void Start()
    {
        // Get reference to Data Controller
        dataController = GameObject.FindObjectOfType<DataController>();
        if (dataController != null)
        {
            // Get all saves
            saves = dataController.currentGameData.saves;
        }

        // Enable buttons for each corresponding save
        SetUpSavePanel();
    }

    void SetUpSavePanel()
    {
        // Set state of save panel, if its open or not
        savePanelOpen = false;

        // Disable all buttons first
        foreach (Button button in saveButtons)
        {
            button.enabled = false;
            button.GetComponent<Image>().color = disabledColor;
        }

        // Enable if there's a save file
        int i = 0;
        foreach (SaveData save in saves)
        {
            saveButtons[i].enabled = true;
            saveButtons[i].GetComponent<Image>().color = enabledColor;
            i++;
        }

        SetCurrentSave();
    }

    void SetCurrentSave()
    {
        if (dataController.currentGameData.currentSave == null)
        {
            return;
        }

        int i = 0;
        foreach (SaveData save in saves)
        {
            if (save.fileName.Equals(dataController.currentGameData.currentSave.fileName))
            {
                saveButtons[i].GetComponent<Image>().color = currentSaveColor;
            }
            else
            {
                saveButtons[i].GetComponent<Image>().color = enabledColor;
            }
        }
    }

    public void ShowSaves()
    {
        savePanel.SetActive(true);
        savePanelOpen = true;
    }

    IEnumerator HideSaves()
    {
        savePanel.GetComponent<Animator>().SetBool("Fade Out", true);
        yield return new WaitForSeconds(1f);
        savePanel.GetComponent<Animator>().SetBool("Fade Out", false);
        savePanel.SetActive(false);
    }

    public void LoadSave(int saveNo)
    {
        dataController.LoadSaveData(saveNo);
        StartCoroutine(ShowLoadResult());
    }

    IEnumerator ShowLoadResult()
    {
        saveResultPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        saveResultPanel.SetActive(false);
        SetCurrentSave();
    }

    void Update()
    {
        if (savePanelOpen)
        {
            if (Input.touchCount > 0)
            {
                if (TouchPhase.Ended == Input.GetTouch(0).phase)
                {
                    StartCoroutine(HideSaves());
                    savePanelOpen = false;
                }
            }
        }
        
    }
}

