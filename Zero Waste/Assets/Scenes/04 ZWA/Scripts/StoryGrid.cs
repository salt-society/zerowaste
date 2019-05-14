using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryGrid : MonoBehaviour
{
    public DataController dataController;
    public GameObject storyPrefab;

    [Space]
    public List<Sprite> mapIcons;

    [Space]
    public List<Battle> zwaStory;

    public void PopulateGrid()
    {
        if (dataController == null)
            return;

        GameObject storyCell;

        
    }
}
