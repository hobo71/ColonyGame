using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

	// Use this for initialization
	void Start () {
		initBuildings();
	}

	private void initBuildings() {
		availBuildings.Add("BasicDome", new structureData(DomeBasicPrefab, DomeBasicPlacement, DomeBasicImage, "A Basic Dome that can be used to produce new settlers", "Basic Dome", new List<HPHandler.ressourceStack> {new HPHandler.ressourceStack(50, HPHandler.ressources.Stone), new HPHandler.ressourceStack(50, HPHandler.ressources.Wood)}));
		availBuildings.Add("SolarPanel", new structureData(SolarPanelPrefab, SolarPanelPlacement, SolarPanelImage, "A simple solar Panel that can generate small amounts of energy", "Solar Panel", new List<HPHandler.ressourceStack> {new HPHandler.ressourceStack(200, HPHandler.ressources.Stone), new HPHandler.ressourceStack(20, HPHandler.ressources.Wood)}));
		availBuildings.Add("WorkerRecycler", new structureData(WorkerRecyclerPrefab, WorkerRecyclerPlacement,WorkerRecyclerImage, "Small Structure that may be used to get rid of workers that are no longer needed, required 500 energy per worker", "Worker Recycler", new List<HPHandler.ressourceStack> {new HPHandler.ressourceStack(50, HPHandler.ressources.Stone), new HPHandler.ressourceStack(50, HPHandler.ressources.Wood)}));
		availBuildings.Add("DrillBasic", new structureData(DrillBasicPrefab, DrillBasicPlacement, DrillBasicImage, "A Drill Platform can automatically harvest nearby stones (80m range), but required a high amount of energy", "Basic Drill Platform", new List<HPHandler.ressourceStack> {new HPHandler.ressourceStack(250, HPHandler.ressources.Stone), new HPHandler.ressourceStack(50, HPHandler.ressources.Wood)}));
		availBuildings.Add("ScrapRecycler", new structureData(ScrapRecyclerPrefab, ScrapRecyclerPlacement, ScrapRecyclerImage, "Use this building to get a use of all that unnecessary scrap! It burns the scrap and generates medium amounts of energy", "Scrap Recycler", new List<HPHandler.ressourceStack> {new HPHandler.ressourceStack(200, HPHandler.ressources.Stone), new HPHandler.ressourceStack(100, HPHandler.ressources.Wood), new HPHandler.ressourceStack(100, HPHandler.ressources.Scrap)}));
		availBuildings.Add("TreeFarm", new structureData(TreeFarmPreFab, TreeFarmPlacement, TreeFarmImage, "A Dome that is used to grow trees, which can be reprocessed to wood using a wood reprocessor", "Tree Farm", TreeFarm.getPrice().ToList()));
		availBuildings.Add("WoodReprocessor", new structureData(WoodReprocessorPrefab, WoodReprocessorPlacement, WoodReprocessorImage, "Used to make Wood out of Trees.", "Wood Reprocessor", WoodReprocessor.getPrice().ToList()));
		
	}

	public Dictionary<string, structureData> getBuildings() {
		return availBuildings;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static string getNiceString(List<HPHandler.ressourceStack> cost) {
		string text = "";

		foreach (var elem in cost) {
			text += elem.getRessource().ToString() + ": " + elem.getAmount() + " ";
		}

		return text;
	}

	public class structureData {
		public GameObject prefab;
		public GameObject placement;
		public Sprite icon;
		public string description;
		public string name;
		public List<HPHandler.ressourceStack> cost = new List<HPHandler.ressourceStack>();

		public structureData(GameObject prefab, GameObject placement, Sprite icon, string description, string name, List<HPHandler.ressourceStack> costs) {
			this.prefab = prefab;
			this.placement = placement;
			this.icon = icon;
			this.description = description;
			this.name = name;
			this.cost = costs;
		}


	}
}
