using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reactorPart : MonoBehaviour, clickable, reactorPart.IHeatableElement, SaveLoad.SerializableInfo, HPHandler.IDestroyAction {

    public interface IHeatableElement {
        float getTemp();
        void setData(ReactorLogic.HeatableStructure data);
    }

    public bool scaleAnim = false;
    public float emissionRate = 0;
    public bool isReactor = false;
    public GameObject explosionPrefab = null;

    private ReactorLogic.HeatableStructure data = null;
    private ParticleSystem particles = null;
    private ParticleSystem particleFire = null;

    void Start() {
        particles = this.transform.GetChild(0).GetComponent<ParticleSystem>();
        if (isReactor) {
            particleFire = this.transform.GetChild(1).GetComponent<ParticleSystem>();
        }
    }

    public void displayInfo() {
    }

    public PopUpCanvas.popUpOption[] getOptions() {
        PopUpCanvas.popUpOption[] options = new PopUpCanvas.popUpOption[] { };
        return options;
    }

    public void handleClick() {

        if (Salvaging.isActive()) {
            Salvaging.createNotification(this.gameObject);
            return;
        }
    }

    public void handleLongClick() {
        this.GetComponent<ClickOptions>().Create();
        foreach (var item in data.connectedController.allStructures) {
            if (item.gameObject.Equals(this.gameObject)) {
                continue;
            }
            item.gameObject.GetComponent<ClickOptions>().Create();
        }
    }

    public void handleOption(string option) {
        return;
    }

    public float getTemp() {
        if (data == null) {
            return 0;
        }
        return (int)data.temperature;
    }

    public void setData(ReactorLogic.HeatableStructure data) {
        this.data = data;
    }

    void FixedUpdate() {
        if (data != null && (data.connectedController == null || !data.connectedController.isActive())) {
            var delta = data.temperature / 35 * Time.deltaTime;
            data.temperature -= delta;
            if (particles != null) {
                particles.emissionRate = 0;
            }
            if (particleFire != null) {
                particleFire.Stop();
            }
        } else if (data != null && data.connectedController != null && data.connectedController.isActive() && data.temperature > 10) {
            //display working anim
            if (particles != null) {
                if (scaleAnim) {
                    particles.startSize = 0.5f + data.temperature / 2000f;
                    particles.emissionRate = emissionRate + (data.temperature / 2000f) * 10f;
                    
                } else {
                    particles.emissionRate = emissionRate;
                    if (isReactor) {
                        if (this.data.temperature > 1800f && !particleFire.isPlaying) {
                            particleFire.Play();
                        } else if (this.data.temperature > 1800f) {
                            particleFire.Stop();
                        }
                    }
                }
            }
        }
    }

    public SaveLoad.SerializationInfo getSerialize() {
        return new serializationData(getTemp());
    }

    public void handleDeserialization(SaveLoad.SerializationInfo info) {
        print("got deserialization for: " + info.scriptTarget);

        serializationData data = (serializationData)info;
        print("deserilazing...");
        var rData = new ReactorLogic.HeatableStructure();
        rData.temperature = data.temp;
        this.setData(rData);

    }

    public void beforeDestroy() {
        if (this.data != null && this.data.connectedController != null) {
            this.data.connectedController.deactivate();
        }

        //if reactor, trigger explosion
        if (isReactor) {
            print("Triggering nuclear meltdown!");
            GameObject.Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        }
    }

    [System.Serializable]
    class serializationData : SaveLoad.SerializationInfo {

        public float temp;

        public serializationData(float temp) {
            this.temp = temp;
        }

        public override string scriptTarget {
            get {
                return "reactorPart";
            }
        }
    }
}
