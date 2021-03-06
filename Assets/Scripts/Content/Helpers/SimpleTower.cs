using System;
using System.Collections.Generic;
using Content.Helpers.Combat;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Content.Helpers {
    public abstract class SimpleTower : TurretConfigurator, IWeapon {

        public float Damage;

        private void Start() {
            energyContainer = this.GetComponent<EnergyContainer>();
        }

        public float attacksPerSecond;
        public float maxRange;
        public float minRange;

        internal GameObject enemy;
        internal Collider enemyCollider;
        internal float timeElapsed = 0f;
        private EnergyContainer energyContainer;

        public new virtual void FixedUpdate() {

            if (!active || energyContainer.getCurEnergy() < 1) return;
            
            energyContainer.addEnergy(-idleEnergyUsage * Time.deltaTime, energyContainer);
            
            if (enemy == null) {
                enemy = findEnemy(useRandom());
                if (enemy != null) {
                    enemyCollider = enemy.GetComponent<Collider>();
                }
            }

            if (enemy == null) {
                return;
            }

            var impactPos = getImpactPos();
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= 1 / attacksPerSecond) {
                timeElapsed -= 1 / attacksPerSecond;
                shoot(enemy, impactPos);
                energyContainer.addEnergy(-energyPerShot * Time.deltaTime, energyContainer);
            }
        }

        public virtual Vector3 getImpactPos() {
            return enemyCollider.ClosestPoint(this.transform.position + Vector3.up * 5);
        }

        public abstract void shoot(GameObject target, Vector3 impactPos);
        public abstract bool useRandom();

        internal GameObject findEnemy() {
            //gather potential targets
            HPHandler.Faction self = this.GetComponent<HPHandler>().faction;
            GameObject target = null;
            float closestDistance = maxRange;

            foreach (HPHandler.Faction item in Enum.GetValues(typeof(HPHandler.Faction))) {
                //skip non-enemy factions
                if (item == self || item == HPHandler.Faction.Neutral) {
                    continue;
                }

                //loop through faction members
                var factionMembers = HPHandler.factionMembers;
                foreach (var enemy in factionMembers[item]) {
                    var dist = Vector3.Distance(this.gameObject.transform.position,
                        enemy.gameObject.transform.position);
                    if (dist < closestDistance && dist > minRange && enemy.GetComponent<Collider>() != null) {
                        target = enemy.gameObject;
                        closestDistance = dist;
                    }
                }
            }

            return target;
        }

        internal GameObject findEnemy(bool random) {
            if (!random) {
                return findEnemy();
            }

            //gather potential targets
            HPHandler.Faction self = this.GetComponent<HPHandler>().faction;
            var potentialEnemies = new List<GameObject>();

            foreach (HPHandler.Faction item in Enum.GetValues(typeof(HPHandler.Faction))) {
                //skip non-enemy factions
                if (item == self || item == HPHandler.Faction.Neutral) {
                    continue;
                }

                //loop through faction members
                var factionMembers = HPHandler.factionMembers;
                foreach (var enemy in factionMembers[item]) {
                    //calc dist
                    var dist = Vector3.Distance(this.gameObject.transform.position,
                        enemy.gameObject.transform.position);

                    //check dist
                    if (dist < maxRange && dist > minRange && enemy.GetComponent<Collider>() != null) {
                        potentialEnemies.Add(enemy);
                    }
                }
            }

            //using ? operator for absolutely no reason at all, but it looks cool
            return potentialEnemies.Count == 0 ? null : potentialEnemies[Random.Range(0, potentialEnemies.Count)];
        }

        public float getDamage() {
            return Damage;
        }

        public float getRange() {
            return maxRange;
        }

        public void setRange(float range) {
            this.maxRange = range;
        }
        
        public abstract string getName();
    }
}