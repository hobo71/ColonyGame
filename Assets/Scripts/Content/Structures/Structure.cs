using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Structure {
    
    bool isWorking();
    GameObject getGameobject();
    ressourceStack[] getCost();
    ressourceStack[] getResources();
    HPHandler getHP();
    inventory getInv();
    void salvage();
    bool isSalvaging();
    
}

public abstract class simpleImplement : MonoBehaviour, Structure {
    
    private readonly List<ressourceStack> ownResource = new List<ressourceStack>();
    private HPHandler hp;
    private inventory inv;
    private bool salvaging;

    public void Start() {
        ownResource.Add(new ressourceStack(getHP().getInitialHP(), getHP().type));
        hp = this.GetComponent<HPHandler>();
        inv = this.GetComponent<inventory>();
    }

    public void FixedUpdate() {
        if (salvaging) {
            this.getHP().HP -= 2.5f;
        }
    }

    public bool isWorking() {
        return true;
    }

    public GameObject getGameobject() {
        return this.gameObject;
    }

    public abstract ressourceStack[] getCost();
    public ressourceStack[] getResources() {
        return ownResource.ToArray();
    }

    public HPHandler getHP() {
        return hp;
    }

    public inventory getInv() {
        return inv;
    }

    public void salvage() {
        salvaging = true;
        Salvaging.displayIndicator(this.gameObject);
    }

    public bool isSalvaging() {
        return salvaging;
    }
}
