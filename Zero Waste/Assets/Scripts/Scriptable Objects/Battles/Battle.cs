using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Battle", menuName = "Battle")]
public class Battle : ScriptableObject {

    [Header("Battle Details")]
    public string battleNo;
    public string nodeName;

}
