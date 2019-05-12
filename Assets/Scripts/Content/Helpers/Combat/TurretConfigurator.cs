using UnityEngine;

namespace Content.Helpers.Combat {
    public abstract class TurretConfigurator : DefaultStructure, TurretConfigurator.ConfigurableTurret {
        public bool active = true;
        public interface ConfigurableTurret {
            void setActive(bool val);
            bool getActive();
        }

        public override int getMaxEnergy() {
            return 5000;
        }

        public override int getMaxOutput() {
            return 0;
        }

        public override int getMaxInput() {
            return 20;
        }

        public override int getPriority() {
            return 10;
        }

        public override void handleClick() {
        
            Debug.Log("clicked structure");

            displayState(getActive());
        }

        public override void handleOption(string option) {
            Debug.Log("handling option: " + option);

            switch (option) {
                case "Activate":
                    this.GetComponent<ConfigurableTurret>().setActive(true);
                    displayState(true);
                    break;
                case "Deactivate":
                    this.GetComponent<ConfigurableTurret>().setActive(false);
                    displayState(false);
                    break;
            }
        }
        private void displayState(bool state) {
            if (state) {
                Notification.createNotification(this.gameObject, Notification.sprites.Working, "Working....", Color.cyan,
                    true);
            }
            else {
                Notification.createNotification(this.gameObject, Notification.sprites.Stopping, "Idle", Color.yellow,
                    false);
            }
        }

        public void setActive(bool val) {
            this.active = val;
        }

        public bool getActive() {
            return active;
        }

        public void displayInfo() {
            //not reachable, need to rework interface
        }

        public override PopUpCanvas.popUpOption[] getOptions() {
            PopUpCanvas.popUpOption activate = new PopUpCanvas.popUpOption("Activate", UIPrefabCache.ActivateBut);
            PopUpCanvas.popUpOption deactivate = new PopUpCanvas.popUpOption("Deactivate", UIPrefabCache.DeactivateBut);

            activate.setEnabled(!getActive());
            deactivate.setEnabled(getActive());

            var options = new[] {activate, deactivate};
            return options;
        }
    }
}