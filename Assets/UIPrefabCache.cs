using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPrefabCache : MonoBehaviour {

	public GameObject canvas;
	public GameObject template;
	public Sprite activateBut;
	public Sprite deactivateBut;
	public Sprite infoBut;
	public static GameObject Canvas;
	public static GameObject Template;
	public static Sprite ActivateBut;
	public static Sprite DeactivateBut;
	public static Sprite InfoBut;


	// Use this for initialization
	void Awake () {
		ActivateBut = activateBut;
		DeactivateBut = deactivateBut;
		InfoBut = infoBut;
		Canvas = canvas;
		Template = template;
	}
}
