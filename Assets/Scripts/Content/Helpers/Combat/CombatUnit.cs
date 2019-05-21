using System;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Helpers.Combat {
    public abstract class CombatUnit : MonoBehaviour {
        //enemy has hphandler, is enemy faction, and has a collider, and was the closest enemy at assignment time
        private int counter = 0;

        public GameObject activeEnemy = null;
        public MovementMode activeMode = MovementMode.idle;

        public float maxRange;
        public float minRange;
        public float scanRange;
        public float optimalRange = 0.7f;
        public bool targetProjectiles = false;
        
        public Vector3 positionTarget = Vector3.zero;
        internal NavMeshAgent agent;
        internal HPHandler enemyHP;
        internal Collider enemyCollider;
        private int blockMoves = 0;
        private Vector3 movePosCache = Vector3.zero;
        private bool hasMoveTarget = false;
        public bool lockState = false;

        [System.Serializable]
        public enum MovementMode {
            approaching,
            inRange_closing,
            inRange_staying,
            inRange_opening,
            tooClose,
            idle,
            walking
        }

        private void Start() {
            agent = this.GetComponent<NavMeshAgent>();
        }

        internal void FixedUpdate() {

            if (activeMode == MovementMode.walking && agent.remainingDistance < 10f && agent.remainingDistance > 8f) {
                blockMoves = 120;
            }

            if (activeMode == MovementMode.walking && agent.remainingDistance < 5f && blockMoves-- <= 0) {
                agent.destination = this.transform.position;
                this.activeMode = MovementMode.idle;
                clearCache();
                moveRand();
            }
            
            counter++;
            //search for new enemy
            if (counter % 10 == 0 && activeEnemy == null) {
                activeEnemy = findClosestEnemy();
                if (activeEnemy != null) {
                    enemyHP = activeEnemy.GetComponent<HPHandler>();
                    enemyCollider = activeEnemy.GetComponent<Collider>();
                }
            }

            if (activeEnemy == null || lockState) {
                if (activeEnemy == null && activeMode == MovementMode.idle) {
                    useTargetCache();
                }
                return;
            }
            
            cacheMoveTarget();

            //update state
            var enemyPoint = enemyCollider.ClosestPoint(this.transform.position);
            var dist = Vector3.Distance(this.transform.position, enemyPoint);
            
            if (dist > scanRange) {
                //too far away
                activeEnemy = null;
                if (activeMode != MovementMode.walking) {
                    activeMode = MovementMode.idle;
                }
                return;
            } else if (dist > maxRange) {
                activeMode = MovementMode.approaching;
            } else if (dist > minRange) {
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
            } else if (dist <= minRange) {
                activeMode = MovementMode.tooClose;
            }

            updatePos(activeEnemy, activeMode, enemyPoint);

            //now let the subclass do with the state what it wants
        }

        private void cacheMoveTarget() {
            if (movePosCache.Equals(Vector3.zero) && hasMoveTarget) {
                hasMoveTarget = false;
                movePosCache = agent.destination;
            }
        }

        private void useTargetCache() {
            if (movePosCache.Equals(Vector3.zero)) {
                return;
            }

            moveTo(movePosCache);
            movePosCache = Vector3.zero;
        }

        private void clearCache() {
            movePosCache = Vector3.zero;
            hasMoveTarget = false;
        }

        private void updatePos(GameObject enemy, MovementMode mode, Vector3 enemyPos) {

            if (mode == MovementMode.inRange_staying || mode == MovementMode.walking) {
                return;
            }
            var dir = this.transform.position - enemyPos;
            var dirSimple = this.transform.position - enemy.transform.position;
            var optimalDist = maxRange * optimalRange;
            positionTarget = enemyPos + dir.normalized * optimalDist;

            agent.destination = positionTarget;
            agent.updateRotation = false;
            this.transform.rotation = Quaternion.LookRotation(-dirSimple, Vector3.up);
        }

        //checks if target is actually attackable
        private bool targetValid(GameObject target) {
            //check for projectile targets
            if (!targetProjectiles && target.GetComponent<Projectile>() != null) return false;
            
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

        public void moveTo(Vector3 target) {
            agent.destination = target;
            activeEnemy = null;
            activeMode = MovementMode.walking;
            blockMoves = 180;
            agent.updateRotation = true;
            hasMoveTarget = true;
        }
        
        public void moveRand() {
            Vector3 goTo = this.transform.forward * UnityEngine.Random.Range(1, 8) + this.transform.right * UnityEngine.Random.Range(-5f, 5f);
            agent.destination = goTo + this.transform.position;
        }
    }
}