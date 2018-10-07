using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EnergyContainer {

	int getMaxEnergy();
    int getCurEnergy();
    int getMaxOutput();
    int getMaxInput();
    int getPriority();          //higher = more imporant
    GameObject GetGameObject();
    List<EnergyContainer> getConnections();
    List<EnergyContainer> getNetwork();
    void setNetwork(List<EnergyContainer> network);
    void addConnection(EnergyContainer container);
    void reloadConnections();
    void handleConnections();   //called at FixedUpdate
    void addEnergy(float amount, EnergyContainer from);

}
