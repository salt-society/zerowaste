﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Environment Components")]
    public GameObject background;

    public void SetBackground(Sprite backgroundSprite)
    {
        background.GetComponent<SpriteRenderer>().sprite = backgroundSprite;
    }
}
