using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Cutscene/Dialogue")]
public class Dialogue : ScriptableObject {

    [Header("Unique Identifier")]
    public int dialogueId;

    [Header("Details")]
    public string characterName;
    public Sprite characterThumb;
    public Sprite characterHalf;
    public Sprite characterSprite;

    [Space]
    public bool isLive2D;

    [Space]
    [TextArea(1, 10)]
    public string content;

    [Space]
    public bool isNarration;
    public bool canSkip;

    [Space]
    public List<string> SFX;

    [Space]
    public Sprite background;
    public Color backgroundColor;

    [Space]
    public Color dialogueBoxColor;

    [Space]
    public string transition;

    [Header("Choices")]
    public bool withChoices;
    public List<Choice> choices;
}
