﻿using System;
using System.Collections;
using System.Collections.Generic;
using Content.Helpers;
using DigitalRuby.LightningBolt;
using UnityEngine;

public class TeslaTower : SimpleTower {
    public GameObject whipPrefab;
    public GameObject startPos;
    public GameObject impactPrefab;

    private bool ready = false;
    private bool morphing = false;
    private static readonly int Activate = Animator.StringToHash("activate");
    private static readonly int Deactivate = Animator.StringToHash("deactivate");

    public override void FixedUpdate() {

        if (morphing || (!ready && !base.active)) {
            return;
        }

        if (ready && !base.active) {
            morphing = true;
            print("deactivating tesla tower");
            Invoke("deactivate", 5f);
            this.GetComponent<Animator>().SetTrigger(Deactivate);
            setParticleState(false);
            ready = false;
            return;
        }
        
        if (!ready && enemyIsClose()) {
            morphing = true;
            Invoke("activate", 5f);
            print("activating tesla tower...");
            this.GetComponent<Animator>().SetTrigger(Activate);
        } else if (ready && !enemyIsClose()) {
            morphing = true;
            print("deactivating tesla tower");
            Invoke("deactivate", 5f);
            this.GetComponent<Animator>().SetTrigger(Deactivate);
            setParticleState(false);
            ready = false;
        }

        if (!ready) {
            return;
        }


        enemy = findEnemy(useRandom());
        if (enemy != null) {
            enemyCollider = enemy.GetComponent<Collider>();
        }
        else {
            return;
        }

        var impactPos = getImpactPos();
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= 1 / attacksPerSecond) {
            timeElapsed -= 1 / attacksPerSecond;
            shoot(enemy, impactPos);
        }
    }

    public override ressourceStack[] getCost() {
        
        ressourceStack[] cost = new ressourceStack[2];

        cost[0] = new ressourceStack(100, ressources.Wood);
        cost[1] = new ressourceStack(100, ressources.Stone);
        return cost;
    }

    private void activate() {
        this.ready = true;
        setParticleState(true);
        morphing = false;
    }
    private void deactivate() {
        morphing = false;
        startPos.SetActive(false);
    }

    private void setParticleState(bool state) {
        startPos.GetComponent<Renderer>().enabled = state;
        startPos.transform.GetChild(0).gameObject.SetActive(state);
    }
    
    private bool enemyIsClose() {
        return this.findEnemy(true) != null;
    }

    public override void shoot(GameObject target, Vector3 impactPos) {
        var position = startPos.transform.position;
        var bullet = GameObject.Instantiate(whipPrefab, position,
            Quaternion.LookRotation(impactPos - position));
        var controller = bullet.GetComponent<LightningBoltScript>();
        controller.StartPosition = startPos.transform.position;
        controller.EndPosition = impactPos;

        if (target != null) {
            target.GetComponent<HPHandler>().HP -= Damage;
        }


        GameObject.Instantiate(impactPrefab, impactPos, Quaternion.AngleAxis(-90f, new Vector3(1, 0, 0)));
    }

    public override bool useRandom() {
        return true;
    }

    public override string getName() {
        return "Electric overcharge";
    }
}