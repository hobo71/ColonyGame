using System;
using System.Collections;
using System.Collections.Generic;
using Content.Helpers.Combat;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class CombatSoldier : CombatUnit {
    
    public float maxSpeed = 15f;
    public float forwardSpeed = 4f;
    public float backwardSpeed = 2f;
    public float damage = 2f;
        
    private Animator animator;
    private MovementMode lastState = MovementMode.idle;
    private static readonly int Walking = Animator.StringToHash("Walking");
    private static readonly int AttackForward = Animator.StringToHash("Attack_Forward");
    private static readonly int AttackNormal = Animator.StringToHash("Attack_Normal");
    private static readonly int AttackBackwards = Animator.StringToHash("Attack_Backwards");
    private static readonly int Shoot = Animator.StringToHash("shoot");

    private new void Awake() {
        base.Awake();
        animator = this.GetComponent<Animator>();
    }

    private new void FixedUpdate() {
        base.FixedUpdate();

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
        if (base.activeMode != lastState) {
            lastState = base.activeMode;
            updateState();
        }
    }

    public void Fire() {
        lineRenderer.SetPosition(0, lineRenderer.gameObject.transform.position);
        lineRenderer.SetPosition(1, activeEnemy.GetComponent<Collider>().bounds.center);
        lineRenderer.GetComponent<Animator>().SetTrigger(Shoot);
        enemyHP.HP -= damage;
    }

    private void updateState() {
        
        //set animator state
        switch (base.activeMode) {
            case MovementMode.closeApproaching:
                animator.SetTrigger(Walking);
                break;
            case MovementMode.inRange_closing:
                animator.SetTrigger(AttackForward);
                break;
            case MovementMode.inRange_staying:
                animator.SetTrigger(AttackNormal);
                break;
            case MovementMode.inRange_opening:
                animator.SetTrigger(AttackBackwards);
                break;
            case MovementMode.tooClose:
                animator.SetTrigger(AttackBackwards);
                break;
            case MovementMode.idle:
                animator.SetTrigger(Walking);
                break;
        }
    }
}