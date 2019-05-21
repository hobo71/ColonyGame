using System.Collections;
using System.Collections.Generic;
using Content.Helpers.Combat;
using UnityEngine;
using UnityEngine.AI;

public class MeleeSoldier : CombatUnit {
	
	public float Damage;
	public TrailRenderer trailRenderer;
	
	private MovementMode lastState = MovementMode.idle;
	private Animator UnitAnimator;
	private static readonly int Walking = Animator.StringToHash("walk");
	private static readonly int Attack = Animator.StringToHash("attack");
	private static readonly int Idle = Animator.StringToHash("idle");

	private void Awake() {
		UnitAnimator = this.GetComponent<Animator>();
	}

	private new void FixedUpdate() {
		base.FixedUpdate();

		if (base.activeMode != lastState) {
			lastState = base.activeMode;
			updateState();
		}

		if (activeEnemy == null) return;
	}
	
	private void updateState() {
		//set animator state
		switch (base.activeMode) {
			case MovementMode.approaching:
			case MovementMode.inRange_opening:
			case MovementMode.walking:
			case MovementMode.tooClose:
				UnitAnimator.SetTrigger(Walking);
				break;
			case MovementMode.inRange_closing:
			case MovementMode.inRange_staying:
				base.lockState = true;
				UnitAnimator.SetTrigger(Attack);
				break;
			case MovementMode.idle:
				UnitAnimator.SetTrigger(Idle);
				this.GetComponent<NavMeshAgent>().destination = this.gameObject.transform.position;
				break;
		}
	}
	
	public void HitStart() {
		trailRenderer.enabled = true;
	}
	
	public void Hit() {
		enemyHP.HP -= Damage;
	}

	public void HitEnd() {
		base.lockState = false;
		trailRenderer.enabled = false;
	}
	
}
