using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public PartIdentifier currentPart;
    public bool isInteractable;

    void Start()
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

    public void CloseArea()
    {
        closeConfirmation.SetActive(!closeConfirmation.activeInHierarchy);
        areaBackground.SetActive(!areaBackground.activeInHierarchy);
        areaUI.SetActive(!areaUI.activeInHierarchy);

        zwaSign.SetActive(!zwaSign.activeInHierarchy);
        mapPoints.SetActive(!mapPoints.activeInHierarchy);

        isInteractable = false;
    }

    public virtual IEnumerator ShowPart(PartIdentifier partIdentifier)
    {
        currentPart = partIdentifier;

        partIdentifier.Highlight();
        yield return new WaitForSeconds(1.0f);
        parts[partIdentifier.partId].SetActive(!parts[partIdentifier.partId].activeInHierarchy);

        isInteractable = false;
    }

    public virtual IEnumerator ClosePart() 
    {
        parts[currentPart.partId].SetActive(!parts[currentPart.partId].activeInHierarchy);
        yield return new WaitForSeconds(0.5f);
        currentPart.Highlight();

        currentPart = null;
        isInteractable = true;
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
                            StartCoroutine(ShowPart(partIdentifier));
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
