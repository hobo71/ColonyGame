using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuDisplay : MonoBehaviour {

	public GameObject itemPrefab;
	public GameObject arrowUp;
	public GameObject arrowDown;
	public int elemsPerSide = 12;
	private int curSide = 0;
	private int sides = 0;

	private GameObject[,] buttons;

	// Use this for initialization
	void Start () {
		print("Initing building menu...");

		float height = Screen.height * 0.66f - 75;
		print("height: " + height);

		
		//building data
		var buildings = GameObject.Find("Terrain").GetComponent<BuildingManager>().getBuildings().Values.ToList();

		int count = buildings.Count;
		int sides = (int) (count / elemsPerSide) + 1;
		this.sides = sides - 1;
		print("sides=" + sides + " elemsPerSide=" + elemsPerSide);

		buttons = new GameObject[sides, elemsPerSide];
		int tCount = 0;


		//create sides
		for (int j = 0; j < sides; j++) {
			//individual side, creating left and right in each iteration
			for (int i = 0; i < elemsPerSide / 2; i++) {

				if (tCount >= count) continue;

				var obj = GameObject.Instantiate(itemPrefab, this.transform);
				obj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(5.6f, - (40f + 5f) * (i) - 30, 0);
				//obj.GetComponent<Image>().color = new Color(1f - (1f / count * tCount), 1f - (1f / count * tCount * 2), 1f);
				obj.GetComponent<Image>().sprite = buildings[tCount].icon;
				
				int elem = tCount;
				obj.GetComponent<Button>().onClick.AddListener(() => buildingClicked(buildings[elem]));
				buttons[j, i] = obj;

				if (j != 0) obj.SetActive(false);
				tCount++;

				//object #2 
				if (tCount >= count) continue;

				obj = GameObject.Instantiate(itemPrefab, this.transform);
				obj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(5.6f + 45f, - (40f + 5f) * (i) - 30, 0);
				//obj.GetComponent<Image>().color = new Color(1f, 1f - (1f / count * tCount), 1f - (1f / count * tCount * 2));
				obj.GetComponent<Image>().sprite = buildings[tCount].icon;
				
				int elemB = tCount;
				obj.GetComponent<Button>().onClick.AddListener(() => buildingClicked(buildings[elemB]));
				buttons[j, (int) (elemsPerSide / 2f) + i] = obj;
				
				if (j != 0) obj.SetActive(false);
				tCount++;
			}
		}

		handleLimit();
	}

	private void buildingClicked(BuildingManager.structureData data) {
        
        clickDetector.overlayClicked = true;

		print("clicked building icon ! " + data.name);

		InfoClicked preview = Scene_Controller.getInstance().buildPreview.GetComponent<InfoClicked>();
        preview.show();
        preview.setTitle(data.name);
        preview.setDesc(data.description);
		preview.setPrice(data.cost);
		preview.setData(data);
	}

	public void nextSide() {
        clickDetector.overlayClicked = true;
		curSide++;
		if (curSide > sides) curSide--;

		print("switched side to: " + curSide + " #ofSides=" + sides + " elems per side: " + buttons.GetLength(1));
		reloadSide(curSide);
	}

	public void previousSide() {
        clickDetector.overlayClicked = true;
		curSide--;
		if (curSide < 0) curSide++;

		print("switched side to: " + curSide + " #ofSides=" + sides + " elems per side: " + buttons.GetLength(1));
		reloadSide(curSide);
	}

	private void handleLimit() {
		if (curSide >= sides) {
			arrowDown.SetActive(false);
		} else {
			arrowDown.SetActive(true);
		}

		if (curSide <= 0) {
			arrowUp.SetActive(false);
		} else {
			arrowUp.SetActive(true);
		}
	}

	private void reloadSide(int curSide) {
		for (int i = 0; i <= sides; i++) {
			for (int j = 0; j < buttons.GetLength(1); j++) {
				try {
					print("iterating in inner loop, j=" + j + " i=" + i + " curside=" + curSide);
					if (i == curSide) {
						buttons[i, j].SetActive(true);
					} else {
						buttons[i, j].SetActive(false);
					}
				} catch (NullReferenceException ex) {}
			}
		}

		handleLimit();
	}

	// Update is called once per frame
	void Update () {
		
	}
}
