using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Choice", menuName = "Choice")]
public class Choice : ScriptableObject
{
    [Header("Choice Details")]
    public string characterName;
    public string choice;

    [Space]
    public Dialogue response;
    public Color color;

    [Header("Result")]
    public Effect[] effects;
}
