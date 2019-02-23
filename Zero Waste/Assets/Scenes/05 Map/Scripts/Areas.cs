using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Area", menuName = "Map/Area")]
public class Areas : ScriptableObject {

    [Header("Details")]
    public Sprite sprite;

    [Space]
    public int areaId;
    public string areaName;
    public string subtitle;

    [Multiline]
    public string info;

    [Space]
    public Vector3 coordinates;
    public float zoomSize;

    [Range(1f, 100f)]
    public float zoomSpeed;
    [Range(1f, 100f)]
    public float moveSpeed;

    [Space]
    [Header("Nodes")]
    public int maxNodes;

    [Space]
    public List<Node> nodes;
    public List<Vector3> positions;
    
    [Header("Progess")]
    public string nextLevel;
    
}
