using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(inventory))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(HPHandler))]
[RequireComponent(typeof(NavMeshSourceTag))]
[RequireComponent(typeof(ClickOptions))]
public abstract class ProductionStructure : TogglableStructure {
	private inventory inventory;
	private Animator animator;
	//used animations: work
	public bool hasAnimation;
	public bool useMovers = false;
	private static readonly int Work = Animator.StringToHash("work");

	private new void Start() {
		base.Start();
		inventory = this.GetComponent<inventory>();
		if (hasAnimation) {
			animator = this.GetComponent<Animator>();
			if (animator == null) {
				hasAnimation = false;
			}
		}
	}

	public override void doStart() {
		base.doStart();
		if (useMovers) {
			foreach (var data in getProduceData()) {
				foreach (var kind in data.outcome) {
					DeliveryRoutes.addRoute(this.gameObject, DeliveryRoutes.getClosest("dropBase", this.gameObject).gameObject, kind.getRessource());
				}
			}
		}
	}

	public class ProduceData {
		//amount are in per second
		public List<ressourceStack> consume;
		public float energyCost = 0;
		public List<ressourceStack> outcome;
		public float energyProduce = 0;
		
	}

	public abstract List<ProduceData> getProduceData();
	
	
	private new void FixedUpdate() {
		base.FixedUpdate();

		if (!busy) return;
		
		//request resources
		if (useMovers) {
			foreach (var data in getProduceData()) {
				foreach (var kind in data.consume) {
					if (counter % 180 == 0 && this.inventory.getFillPercent() < 0.9f) {
						RessourceHelper.deliverTo(this.gameObject, false, kind.getRessource());
					}
				}
			}
		}
		
		//consume goods
		var dataList = this.getProduceData();
		var workDone = false;
		foreach (var data in dataList) {
			if (handleProduceData(data)) {
				workDone = true;
			}
		}

		if (hasAnimation) {
			animator.SetBool(Work, workDone);
		}
	}

	private bool handleProduceData(ProduceData data) {

		if (this.storedEnergy < data.energyCost * Time.deltaTime) return false;
		
		foreach (var elem in data.consume) {
			var scaled = elem.clone();
			scaled.setAmount(scaled.getAmount() * Time.deltaTime);
			if (!inventory.canTake(scaled)) return false;
		}
		
		//has enough energy and ressources!
		this.storedEnergy -= data.energyCost * Time.deltaTime;

		foreach (var elem in data.consume) {
			var scaled = elem;
			scaled.setAmount(scaled.getAmount() * Time.deltaTime);
			inventory.remove(scaled);
		}
		
		//create outcome/energy
		this.storedEnergy += data.energyProduce *= Time.deltaTime;
		
		foreach (var elem in data.outcome) {
			var scaled = elem;
			scaled.setAmount(scaled.getAmount() * Time.deltaTime);
			inventory.add(scaled);
		}

		return true;
	}
}
