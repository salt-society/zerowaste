using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Cutscene/Dialogue")]
public class Dialogue : ScriptableObject {

    [Header("Unique Identifier")]
    public int dialogueId;

    [Header("Character")]
    public string characterName;
    public Sprite characterThumb;
    public Sprite characterHalf;
    public Sprite characterSprite;

    [Space]
    public bool isCharacterLive2D;

    [Header("Content")]
    [TextArea(1, 10)]
    public string content;

    [Header("Content Details")]
    public bool isNarration;
    public bool canSkip;

    [Header("Sounds")]
    public List<string> SFX;

    [Header("Graphics")]
    public Sprite background;
    
    [Space]
    public Color dialogueBoxColor;
    public Color backgroundColor;

    [Header("Choices")]
    public bool withChoices;
    public List<Choice> choices;

    [Header("Item")]
    public bool withItem;
    public Sprite itemSprite;

    [Header("Words")]
    public bool withUnfamiliarWord;
    public List<string> words;
    public List<Color32> wordColors;
}
