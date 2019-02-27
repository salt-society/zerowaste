using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cutscene", menuName = "Cutscene/Cutscene")]
public class Cutscene : ScriptableObject {

    [Header("Unique Identifier")]
	public int cutsceneId;

    [Space]
    [Header("Cutscene Details")]
    public string chapter;
    public string title;
    public Sprite firstBackground;

    [Space]
    [Header("Dialogue")]
    public List<Dialogue> dialogues;

    [Space]
    public List<string> nextLevels;
    public List<int> levelIds;

    [Space]
    public string nextScene;
}
