using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface clickable {

    void handleClick();
    void handleLongClick();
    void handleOption(string option);
    void displayInfo();
    PopUpCanvas.popUpOption[] getOptions();
    
}
