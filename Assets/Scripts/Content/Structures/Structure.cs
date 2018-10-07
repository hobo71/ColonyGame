using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Structure {
    
    bool isWorking();
    GameObject getGameobject();
    HPHandler.ressourceStack[] getCost();
    HPHandler.ressourceStack[] getResources();
    HPHandler getHP();
    inventory getInv();
    void salvage();
    bool isSalvaging();
}
