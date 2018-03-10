using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	
	void Start () {
        EventMgr.Instance.SendEvent("Test",true);
        UIMgr.Instance.ShowUI("Panel",typeof(UITest));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
