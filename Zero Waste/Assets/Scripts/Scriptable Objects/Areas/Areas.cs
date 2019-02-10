using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Area", menuName = "Area")]
public class Areas : ScriptableObject {

    [Header("Details")]
    public Sprite sprite;

    [Space]
    public string areaName;
    public string subtitle;

    [Multiline]
    public string info;

    [Space]
    public Vector3 coordinates;

    [Range(1f, 5f)]
    public float zoomSpeed;
    [Range(1f, 5f)]
    public float moveSpeed;

    [Space]
    [Header("Nodes")]
    public Battle[] battles;

    [Header("Progess")]
    public string nextLevel;
    
}
