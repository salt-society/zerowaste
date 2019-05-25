using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengerGrid : MonoBehaviour
{
    public DataController dataController;

    [Space]
    public GameObject scavDataPrefab;
    public GameObject unknownPrefab;

    [Space]
    public GameObject mutantInfoPanel;
    public GameObject instruction;
    public GameObject haventEncountered;
    public GameObject parentExit;

    [Space]
    public List<Sprite> areaSprites;
    public List<string> areaNames;
    public List<Sprite> scavSelected;
}
