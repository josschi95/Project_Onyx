using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A class that will be cached by the DungeonManager script for when toggling different elements within the dungeon */

public interface IDungeonElement
{
    void OnActivation(int type, int value);


}
