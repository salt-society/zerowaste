using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificationManager : MonoBehaviour {

    public TextMeshProUGUI messageDisplay;
    public Animator animator;

    private bool isNotifVisible = false;

    public void ShowNotif(string message)
    {
        animator.SetBool("HideNotification", false);
        animator.SetBool("ShowNotification", true);
        messageDisplay.SetText(message);

        isNotifVisible = true;
    }

    public void HideNotif()
    {
        animator.SetBool("HideNotification", true);
    }

    public void ShakeNotif()
    {
        if (isNotifVisible)
        {
            animator.SetTrigger("ShakeNotif");
        }
    }

    public void ShowAndHideNotif(string message)
    {
        animator.SetBool("ShowAndHide", true);
        messageDisplay.SetText(message);
        isNotifVisible = false;
    }
}
