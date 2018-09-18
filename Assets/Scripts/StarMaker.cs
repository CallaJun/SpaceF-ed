using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarMaker : MonoBehaviour {
	[SerializeField] int StarCount = 200;
	[SerializeField] GameObject StarPrefab;
	[SerializeField] float MinDistance = 4, MaxDistance = 10;
	MaterialPropertyBlock tmpPropertyBlock;

	// Use this for initialization
	void Start () {
		tmpPropertyBlock = new MaterialPropertyBlock();
		for (int i = 0; i < StarCount; i++) {
			GameObject now = Instantiate(StarPrefab, Vector3.zero, Quaternion.identity, transform);
			now.name = "Star "  + i.ToString();
			Vector3 pos = Random.onUnitSphere;
			pos *= Random.Range(MinDistance, MaxDistance);
			now.transform.position = pos;
			Renderer renderer = now.GetComponent<Renderer>();
			renderer.GetPropertyBlock(tmpPropertyBlock);
			// tmpPropertyBlock.SetColor("_Color", )
			tmpPropertyBlock.SetFloat("_ScaleX", 0.1f);
			tmpPropertyBlock.SetFloat("_ScaleY", 0.1f);
			renderer.SetPropertyBlock(tmpPropertyBlock);
		}
	}
}
