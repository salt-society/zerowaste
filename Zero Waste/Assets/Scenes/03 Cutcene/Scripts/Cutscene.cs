using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cutscene", menuName = "Cutscene")]
public class Cutscene : ScriptableObject {

    [Header("Unique Identifier")]
	public int cutsceneId;

    [Space]
    [Header("Cutscene Details")]
    public string chapter;
    public string title;
    public Sprite firstBackground;
    public string transition;

    [Space]
    public string nextLevel;

    [Space]
    [Header("Dialogue")]
    public List<Dialogue> dialogues;
}
