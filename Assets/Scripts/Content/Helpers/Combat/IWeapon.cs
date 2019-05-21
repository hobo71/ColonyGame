using System;
using UnityEngine;

namespace Content.Helpers.Combat {
    public interface IWeapon {
        float getDamage();
        float getRange();
        void setRange(float range);
        string getName();
        GameObject gameObject { get; }
    }
}