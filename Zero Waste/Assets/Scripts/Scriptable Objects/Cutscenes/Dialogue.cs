using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject {

    [Header("Unique Identifier")]
    public int dialogueNo;

    [Header("Details")]
    public string characterName;
    public Sprite characterSprite;

    [Space]
    [TextArea(1, 10)]
    public string content;
    public bool isNarration;

    [Space]
    public Sprite background;
    public Color dialogueBoxColor;

    [Space]
    public string transition;
}
