using System;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Helpers.Combat {
    public abstract class CombatUnit : MonoBehaviour {
        //enemy has hphandler, is enemy faction, and has a collider, and was the closest enemy at assignment time
        private int counter = 0;

        internal void Awake() {
            animator = lineRenderer.GetComponent<Animator>();
        }

        public GameObject activeEnemy = null;
        public MovementMode activeMode = MovementMode.idle;

        public float maxRange;
        public float minRange;
        public float scanRange;
        public float optimalRange = 0.7f;

        public LineRenderer lineRenderer;
        
        public Vector3 positionTarget = Vector3.zero;
        internal NavMeshAgent agent;
        internal HPHandler enemyHP;
        private Animator animator;

        [System.Serializable]
        public enum MovementMode {
            closeApproaching,
            inRange_closing,
            inRange_staying,
            inRange_opening,
            tooClose,
            idle
        }

        private void Start() {
            agent = this.GetComponent<NavMeshAgent>();
        }

        internal void FixedUpdate() {
            counter++;
            //search for new enemy
            if (counter % 10 == 0 && activeEnemy == null) {
                activeEnemy = findClosestEnemy();
                if (activeEnemy != null) {
                    enemyHP = activeEnemy.GetComponent<HPHandler>();
                }
            }

            if (activeEnemy == null) {
                return;
            }

            //update state
            var dist = Vector3.Distance(this.transform.position, activeEnemy.transform.position);
            if (dist > scanRange) {
                //too far away
                activeMode = MovementMode.idle;
                activeEnemy = null;
                return;
            }
            else if (dist > maxRange) {
                activeMode = MovementMode.closeApproaching;
            }
            else if (dist > minRange) {
                //in range, betweent min and max range
                var optimalDist = maxRange * optimalRange;
                if (dist > optimalDist + 1) {
                    activeMode = MovementMode.inRange_closing;
                }
                else if (dist > optimalDist - 1) {
                    activeMode = MovementMode.inRange_staying;
                }
                else {
                    activeMode = MovementMode.inRange_opening;
                }
            }
            else {
                activeMode = MovementMode.tooClose;
            }

            updatePos(activeEnemy, activeMode);

            //now let the subclass do with the state what it wants
        }

        private void updatePos(GameObject enemy, MovementMode mode) {

            if (mode == MovementMode.inRange_staying) {
                return;
            }
            
            var enemyPos = enemy.transform.position;
            var dir = this.transform.position - enemyPos;
            var optimalDist = maxRange * optimalRange;
            positionTarget = enemyPos + dir.normalized * optimalDist;

            agent.destination = positionTarget;
            agent.updateRotation = mode == MovementMode.closeApproaching;
            this.transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);
        }

        //checks if target is actually attackable
        private bool targetValid(GameObject target) {
            return target.GetComponent<HPHandler>() != null && target.GetComponent<Collider>() != null;
        }

        internal GameObject findClosestEnemy() {
            //gather potential targets
            HPHandler.Faction self = this.GetComponent<HPHandler>().faction;

            float closest = float.MaxValue;
            GameObject result = null;

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
                    if (dist < closest && targetValid(enemy)) {
                        closest = dist;
                        result = enemy;
                    }
                }
            }

            return result;
        }
    }
}