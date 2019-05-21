using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProductionStructure : TogglableStructure {
	private inventory inventory;

	private new void Start() {
		base.Start();
		inventory = this.GetComponent<inventory>();
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
		
		//consume goods
		var dataList = this.getProduceData();
		foreach (var data in dataList) {
			handleProduceData(data);
		}
	}

	private void handleProduceData(ProduceData data) {

		if (this.storedEnergy < data.energyCost * Time.deltaTime) return;
		
		foreach (var elem in data.consume) {
			var scaled = elem.clone();
			scaled.setAmount(scaled.getAmount() * Time.deltaTime);
			if (!inventory.canTake(scaled)) return;
		}
		
		//has enough energy and ressources!
		this.storedEnergy -= data.energyCost * Time.deltaTime;

		foreach (var elem in data.consume) {
			var scaled = elem;
			scaled.setAmount(scaled.getAmount() * Time.deltaTime);
			inventory.remove(scaled);
		}
		
		//create outomce/energy
		this.storedEnergy += data.energyProduce *= Time.deltaTime;
		
		foreach (var elem in data.outcome) {
			var scaled = elem;
			scaled.setAmount(scaled.getAmount() * Time.deltaTime);
			inventory.add(scaled);
		}
	}
}
