using UnityEditor;

namespace Editor {
    [CustomEditor(typeof(PointDefenseTurret))]
    public class PDD_RangeConfigurator : UnityEditor.Editor {
        void OnSceneGUI() {
            var turret = target as PointDefenseTurret;
            var transform = turret.transform;
            turret.range = Handles.RadiusHandle(
                transform.rotation,
                transform.position,
                turret.range);
        }
    }
}