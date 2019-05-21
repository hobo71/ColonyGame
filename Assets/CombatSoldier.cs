using System;
using System.Collections;
using System.Collections.Generic;
using Content.Helpers.Combat;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class CombatSoldier : CombatUnit, IWeapon {
    public float maxSpeed = 15f;
    public float forwardSpeed = 4f;
    public float backwardSpeed = 2f;
    public float damage = 2f;

    public LineRenderer lineRenderer;

    private Animator UnitAnimator;
    private Animator ShotAnimator;
    private MovementMode lastState = MovementMode.idle;
    
    private static readonly int Walking = Animator.StringToHash("Walking");
    private static readonly int AttackForward = Animator.StringToHash("Attack_Forward");
    private static readonly int AttackNormal = Animator.StringToHash("Attack_Normal");
    private static readonly int AttackBackwards = Animator.StringToHash("Attack_Backwards");
    private static readonly int Shoot = Animator.StringToHash("shoot");
    private static readonly int Idle = Animator.StringToHash("Idle");

    private void Awake() {
        UnitAnimator = this.GetComponent<Animator>();
        ShotAnimator = lineRenderer.GetComponent<Animator>();
    }

    private new void FixedUpdate() {
        base.FixedUpdate();
        
        if (base.activeMode != lastState) {
            if (activeMode == MovementMode.walking) {
                agent.speed = maxSpeed;
            }
            lastState = base.activeMode;
            updateState();
        }

        if (activeEnemy == null) return;

        var curSpeed = maxSpeed;

        switch (base.activeMode) {
            case MovementMode.tooClose:
            //fallthrough
            case MovementMode.inRange_opening:
                curSpeed = backwardSpeed;
                break;
            case MovementMode.inRange_closing:
                curSpeed = forwardSpeed;
                break;
        }

        agent.speed = curSpeed;
    }

    public void Fire() {
        if (activeEnemy == null ||
            Vector3.Distance(activeEnemy.transform.position, this.transform.position) > maxRange) return;
        
        //display shot renderer
        lineRenderer.SetPosition(0, lineRenderer.gameObject.transform.position);
        lineRenderer.SetPosition(1, getImpactPos(enemyCollider));
        ShotAnimator.SetTrigger(Shoot);
        enemyHP.HP -= damage;
    }

    private Vector3 getImpactPos(Collider col) {
        return RandomPointInBounds(col.bounds);
        //return activeEnemy.GetComponent<Collider>().bounds.center;
    }

    private static Vector3 RandomPointInBounds(Bounds bounds) {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    private void updateState() {
        //set animator state
        switch (base.activeMode) {
            case MovementMode.approaching:
            case MovementMode.walking:
                UnitAnimator.SetTrigger(Walking);
                break;
            case MovementMode.inRange_closing:
                UnitAnimator.SetTrigger(AttackForward);
                break;
            case MovementMode.inRange_staying:
                UnitAnimator.SetTrigger(AttackNormal);
                break;
            case MovementMode.inRange_opening:
                UnitAnimator.SetTrigger(AttackBackwards);
                break;
            case MovementMode.tooClose:
                UnitAnimator.SetTrigger(AttackBackwards);
                break;
            case MovementMode.idle:
                UnitAnimator.SetTrigger(Idle);
                this.GetComponent<NavMeshAgent>().destination = this.gameObject.transform.position;
                break;
        }
    }

    public float getDamage() {
        return damage;
    }

    public float getRange() {
        return maxRange;
    }

    public void setRange(float range) {
        maxRange = range;
        scanRange = maxRange * 1.5f + 10;
    }

    public string getName() {
        return "Standard marine";
    }
}