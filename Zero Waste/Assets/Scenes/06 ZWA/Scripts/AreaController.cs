﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaController : MonoBehaviour
{
    public DataController dataController;

    [Space]
    public GameObject zwaSign;
    public GameObject mapPoints;

    [Space]
    public GameObject areaBackground;
    public GameObject areaUI;

    [Space]
    public GameObject closeConfirmation;

    [Space]
    public List<GameObject> partNames;
    public List<GameObject> parts;

    [Space]
    public PartIdentifier currentPart;
    public bool isInteractable;

    [Space]
    public int nextSceneId;

    [Space]
    public GameObject fadeTransition;

    public virtual void Start()
    {
        dataController = FindObjectOfType<DataController>();
    }

    public IEnumerator ShowInteractableParts()
    {
        foreach (GameObject name in partNames)
            name.SetActive(!name.activeInHierarchy);

        yield return new WaitForSeconds(2f);

        foreach (GameObject name in partNames)
            name.SetActive(!name.activeInHierarchy);

        yield return null;
        isInteractable = true;
    }

    public void CloseConfirmation()
    {
        closeConfirmation.SetActive(!closeConfirmation.activeInHierarchy);
    }

    public virtual void CloseArea()
    {
        partNames[4].SetActive(false);
        currentPart.partSelected.SetActive(false);

        closeConfirmation.SetActive(!closeConfirmation.activeInHierarchy);
        areaBackground.SetActive(!areaBackground.activeInHierarchy);
        areaUI.SetActive(!areaUI.activeInHierarchy);

        zwaSign.SetActive(!zwaSign.activeInHierarchy);
        mapPoints.SetActive(!mapPoints.activeInHierarchy);

        isInteractable = false;
    }

    public virtual IEnumerator ShowPartIE(PartIdentifier partIdentifier)
    {
        currentPart = partIdentifier;

        partIdentifier.Highlight();
        yield return new WaitForSeconds(1.0f);
        parts[partIdentifier.partId].SetActive(true);

        isInteractable = false;
    }

    public virtual IEnumerator ClosePartIE() 
    {
        parts[currentPart.partId].SetActive(false);
        yield return new WaitForSeconds(0.5f);
        currentPart.Highlight();

        currentPart = null;
        isInteractable = true;
    }

    public IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(2f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", true);
        yield return new WaitForSeconds(2f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", false);
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(2);
    }

    public virtual void Update()
    {
        if (isInteractable)
        {
            if (Input.touchCount == 1)
            {
                if (TouchPhase.Began == Input.GetTouch(0).phase)
                {
                    Vector2 origin = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero);

                    if (hit.collider != null)
                    {
                        GameObject part = hit.transform.gameObject;
                        if (part.GetComponent<PartIdentifier>() != null)
                        {
                            PartIdentifier partIdentifier = part.GetComponent<PartIdentifier>();
                            StartCoroutine(ShowPartIE(partIdentifier));
                        }
                        else
                        {

                        }
                    }
                    else
                    {

                    }
                }
            }
        }
    }
}
