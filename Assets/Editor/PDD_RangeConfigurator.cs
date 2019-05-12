using Content.Helpers.Combat;
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
    
    [CustomEditor(typeof(CombatSoldier))]
    public class Combat_RangeConfigurator : UnityEditor.Editor {
        void OnSceneGUI() {
            var elem = target as CombatSoldier;
            var transform = elem.transform;
            elem.scanRange = Handles.RadiusHandle(
                transform.rotation,
                transform.position,
                elem.scanRange);
        
            Handles.DrawDottedLine(transform.position, elem.positionTarget, 4f);
        }
    }
}