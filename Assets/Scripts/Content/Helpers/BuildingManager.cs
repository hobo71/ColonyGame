﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour {

	private Dictionary<string, structureData> availBuildings = new Dictionary<string, structureData>();

	public Sprite DomeBasicImage;
	public GameObject DomeBasicPrefab;
	public GameObject DomeBasicPlacement;
	public Sprite SolarPanelImage;
	public GameObject SolarPanelPrefab;
	public GameObject SolarPanelPlacement;
	public Sprite WorkerRecyclerImage;
	public GameObject WorkerRecyclerPrefab;
	public GameObject WorkerRecyclerPlacement;
	public Sprite DrillBasicImage;
	public GameObject DrillBasicPrefab;
	public GameObject DrillBasicPlacement;
	public Sprite ScrapRecyclerImage;
	public GameObject ScrapRecyclerPrefab;
	public GameObject ScrapRecyclerPlacement;
	public GameObject TreeFarmPreFab;
	public GameObject TreeFarmPlacement;
	public Sprite TreeFarmImage;
	public GameObject WoodReprocessorPrefab;
	public GameObject WoodReprocessorPlacement;
	public Sprite WoodReprocessorImage;
	public GameObject OreProcessingPrefab;
	public GameObject OreProcessingPlacement;
	public Sprite OreProcessingImage;
	
	public GameObject ReactorControllerPrefab;
	public GameObject ReactorControllerPlacement;
	public Sprite ReactorControllerImage;
	
	public GameObject WaterPumpPrefab;
	public GameObject WaterPumpPlacement;
	public Sprite WaterPumpImage;

	// Use this for initialization
	void Start () {
		initBuildings();
	}

	private void initBuildings() {
		availBuildings.Add("BasicDome", new structureData(DomeBasicPrefab, DomeBasicPlacement, DomeBasicImage, "A Basic Dome that can be used to produce new settlers", "Basic Dome", new List<ressourceStack> {new ressourceStack(50, ressources.Stone), new ressourceStack(50, ressources.Wood)}));
		availBuildings.Add("SolarPanel", new structureData(SolarPanelPrefab, SolarPanelPlacement, SolarPanelImage, "A simple solar Panel that can generate small amounts of energy", "Solar Panel", new List<ressourceStack> {new ressourceStack(200, ressources.Stone), new ressourceStack(20, ressources.Wood)}));
		availBuildings.Add("WorkerRecycler", new structureData(WorkerRecyclerPrefab, WorkerRecyclerPlacement,WorkerRecyclerImage, "Small Structure that may be used to get rid of workers that are no longer needed, required 500 energy per worker", "Worker Recycler", new List<ressourceStack> {new ressourceStack(50, ressources.Stone), new ressourceStack(50, ressources.Wood)}));
		availBuildings.Add("DrillBasic", new structureData(DrillBasicPrefab, DrillBasicPlacement, DrillBasicImage, "A Drill Platform can automatically harvest nearby stones (80m range), but required a high amount of energy", "Basic Drill Platform", new List<ressourceStack> {new ressourceStack(250, ressources.Stone), new ressourceStack(50, ressources.Wood)}));
		availBuildings.Add("ScrapRecycler", new structureData(ScrapRecyclerPrefab, ScrapRecyclerPlacement, ScrapRecyclerImage, "Use this building to get a use of all that unnecessary scrap! It burns the scrap and generates medium amounts of energy", "Scrap Recycler", new List<ressourceStack> {new ressourceStack(200, ressources.Stone), new ressourceStack(100, ressources.Wood), new ressourceStack(100, ressources.Scrap)}));
		availBuildings.Add("TreeFarm", new structureData(TreeFarmPreFab, TreeFarmPlacement, TreeFarmImage, "A Dome that is used to grow trees, which can be reprocessed to wood using a wood reprocessor", "Tree Farm", TreeFarm.getPrice().ToList()));
		availBuildings.Add("WoodReprocessor", new structureData(WoodReprocessorPrefab, WoodReprocessorPlacement, WoodReprocessorImage, "Used to make Wood out of Trees.", "Wood Reprocessor", WoodReprocessor.getPrice().ToList()));
		availBuildings.Add("OreProcessingPlant", new structureData(OreProcessingPrefab, OreProcessingPlacement, OreProcessingImage, "The Mineral Processing Plant can be used to make gold and iron ingots out of the corresponding ores", "Ore Processing Plant", MineralProcessingPlant.getPrice().ToList()));
		availBuildings.Add("ReactorController", new structureData(ReactorControllerPrefab, ReactorControllerPlacement, ReactorControllerImage, "The Central control piece of the Nuclear Reactor. Uses uranium to turn water into steam", "ReactorController", ReactorController.getPrice().ToList()));
		availBuildings.Add("WaterPump", new structureData(WaterPumpPrefab, WaterPumpPlacement, WaterPumpImage, "The Water Pump can be used to pump water out of a nearby lake", "Water Pump", waterpump.getPrice().ToList(), waterpump.isNearWater));
		
	}

	public Dictionary<string, structureData> getBuildings() {
		return availBuildings;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static string getNiceString(List<ressourceStack> cost) {
		string text = "";

		foreach (var elem in cost) {
			text += elem.getRessource().ToString() + ": " + elem.getAmount() + " ";
		}

		return text;
	}

	public class structureData {
		//type declaration
		public delegate bool requirementcheck (GameObject holoPlacement, bool terrainCheck);
		public GameObject prefab;
		public GameObject placement;
		public Sprite icon;
		public string description;
		public string name;
		public List<ressourceStack> cost = new List<ressourceStack>();
		public requirementcheck buildCheck;

		public structureData(GameObject prefab, GameObject placement, Sprite icon, string description, string name, List<ressourceStack> costs, requirementcheck checkFunc = null) {
			this.prefab = prefab;
			this.placement = placement;
			this.icon = icon;
			this.description = description;
			this.name = name;
			this.cost = costs;
			this.buildCheck = checkFunc;
		}


	}
}
