using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryGrid : MonoBehaviour
{
    public DataController dataController;
    public GameObject storyPrefab;
    public List<Sprite> mapIcons;

    public void PopulateGrid()
    {
        if (dataController == null)
            return;

        GameObject storyCell;
    }
}
