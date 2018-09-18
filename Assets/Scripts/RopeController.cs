using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class RopeController : MonoBehaviour {
	[SerializeField] public Transform StartPoint;
	[SerializeField] public Transform EndPoint;
	[SerializeField] GameObject JointPrefab;
	[SerializeField] public int SegmentCount = 10;
	[SerializeField] public int RenderQuality = 2;
	[SerializeField] float CatmullResidue = 1f;
	[SerializeField] float CatmullTension = 0.2f;
	public GameObject[] PointList;
	public Vector3[] line;
	float[] lengthAt;
	float[] temperature;
	[SerializeField]float breakThreshold = 200;
	[SerializeField] float CoolDownSpeed = 40;
	[SerializeField] float HeatUpSpeed = 200;
	[SerializeField] int HeatUpInfluence = 9;
	[SerializeField] GameObject RopePrefab;
	float sumLength = 0;
	LineRenderer lineRenderer;
	float lineWidth = 0.2f;
	[SerializeField] ParticleSystem laserCuttingEffect;
	int lastShot = -1;
	public void ShotAt(int idx, Vector3 worldPos) {
		Debug.Log("Shot at " + idx);
		lastShot = Time.renderedFrameCount;
		// Make sure this is only called once a frame
		if (laserCuttingEffect) {
			laserCuttingEffect.gameObject.transform.position = worldPos;
		}
		float baseIncrement = Time.deltaTime * HeatUpSpeed;
		for (int i = 1; i <= HeatUpInfluence; i++) {
			int nowIdx = idx - i;
			if (nowIdx < 0)
				break;
			temperature[nowIdx] += baseIncrement * Mathf.Sqrt((HeatUpInfluence - i - 1) / HeatUpInfluence);
		}
		temperature[idx] += baseIncrement;
		for (int i = 1; i <= HeatUpInfluence; i++) {
			int nowIdx = idx + i;
			if (nowIdx > line.Length - 1)
				break;
			temperature[nowIdx] += baseIncrement * Mathf.Sqrt((HeatUpInfluence - i - 1) / HeatUpInfluence);
		}
	}
	public bool isBroken = false;
	void CoolDown() {
		for (int i = 0; i < line.Length; i++) {
			temperature[i] = (temperature[i] - CoolDownSpeed * Time.deltaTime) > 0 ? (temperature[i] - CoolDownSpeed * Time.deltaTime) : 0;
		}
	}
	void BreakAt(int idx) {
		// new gameobject
		var newRope = Instantiate(RopePrefab, transform.position, Quaternion.identity);
		newRope.transform.parent = transform.parent;

		// delete old spring
		if (idx % 20 == 0) {
			idx ++;
		}
		if (idx > RenderQuality * SegmentCount) {
			idx -= 2;
		}

		int cutAtSegment = idx / 20 + 1; // first one is a rigidbody holder
		var cutPos = line[idx];
		var newJoint3 = Instantiate(JointPrefab, PointList[cutAtSegment].transform.position, Quaternion.identity, newRope.transform);
		
		Destroy(PointList[cutAtSegment]);

		// add two new spring
		var newJoint1 = Instantiate(JointPrefab, cutPos, Quaternion.identity, transform);
		newJoint1.GetComponent<SpringJoint>().connectedBody = PointList[cutAtSegment - 1].GetComponent<Rigidbody>();
		newJoint1.transform.parent = transform;
		var newJoint2 = new GameObject();
		newJoint2.transform.position = cutPos;
		newJoint2.transform.parent = newRope.transform;
		Rigidbody newStartRigidbody = newJoint2.AddComponent<Rigidbody>();
		newStartRigidbody.useGravity = false;
		newStartRigidbody.isKinematic = false;
		newJoint3.GetComponent<SpringJoint>().connectedBody = newStartRigidbody;
		// PointList[0] = StartAnchor;

		// set values
		var newRopeController = newRope.GetComponent<RopeController>();
		newRopeController.isBroken = true;
		newRopeController.RenderQuality = RenderQuality;
		newRopeController.SegmentCount = SegmentCount - cutAtSegment + 1;
		newRopeController.PointList = new GameObject[newRopeController.SegmentCount + 1];
		newRopeController.PointList[0] = newJoint2;
		newRopeController.StartPoint = newJoint2.transform;
		newRopeController.EndPoint = EndPoint;

		
		newRopeController.PointList[1] = newJoint3;
		for (int i = cutAtSegment + 1; i < PointList.Length; i++) {
			PointList[i].transform.parent = newRope.transform;
			newRopeController.PointList[i - cutAtSegment + 1] = PointList[i];
		}
		newRopeController.PointList[1].GetComponent<SpringJoint>().connectedBody = newJoint2.GetComponent<Rigidbody>();
		if (PointList.Length > 2) {
			newRopeController.PointList[2].GetComponent<SpringJoint>().connectedBody = newJoint3.GetComponent<Rigidbody>();
		}
		Array.Resize(ref PointList, cutAtSegment + 1);
		
		PointList[cutAtSegment] = newJoint1;
		SegmentCount = cutAtSegment;
		EndPoint = newJoint1.transform;
		lineRenderer.positionCount = SegmentCount * RenderQuality + 1;
		Array.Resize(ref line, SegmentCount * RenderQuality + 1);
		lineRenderer.SetPositions(line);

		isBroken = true;
	}

	// Use this for initialization
	void Start () {
		Debug.Log(isBroken);
		line = new Vector3[SegmentCount * RenderQuality + 1];
		lengthAt = new float[SegmentCount * RenderQuality + 1];
		temperature = new float[SegmentCount * RenderQuality + 1];
		for (int i = 0; i < SegmentCount * RenderQuality + 1; i++) {
			line[i] = new Vector3();
		}
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.positionCount = SegmentCount * RenderQuality + 1;
		lineRenderer.startWidth = lineWidth;
		lineRenderer.endWidth = lineWidth;

		if (!isBroken) {
			GameObject StartAnchor = new GameObject("Start Anchor");
			StartAnchor.transform.position = StartPoint.position;
			StartAnchor.transform.parent = transform;
			Rigidbody StartRigidbody = StartAnchor.AddComponent<Rigidbody>();
			StartRigidbody.useGravity = false;
			StartRigidbody.isKinematic = true;
			PointList = new GameObject[SegmentCount + 1];
			PointList[0] = StartAnchor;
			for (int i = 0; i < SegmentCount; i++) {
				Vector3 pos = (EndPoint.position - StartPoint.position) / SegmentCount * (i + 1) + StartPoint.position;
				var nowSegment = Instantiate(JointPrefab, pos, Quaternion.identity, transform);
				var nowSpringJoint = nowSegment.GetComponent<SpringJoint>();
				nowSpringJoint.connectedBody = PointList[i].GetComponent<Rigidbody>();
				PointList[i + 1] = nowSegment;
			}

			var lastRigidbody = PointList[SegmentCount].GetComponent<Rigidbody>();
			lastRigidbody.isKinematic = true;
		}
	}

	static Vector3 CatmullRom(float t, float k, Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
		Vector4 A = new Vector4(1, t, t * t, t * t * t);
		Vector4 l1 = new Vector4(0, -k, 2 * k, -k);
		Vector4 l2 = new Vector4(1, 0, k - 3, 2 - k);
		Vector4 l3 = new Vector4(0, k, 3 - 2 * k, k - 2);
		Vector4 l4 = new Vector4(0, 0, -k, k);
		float n1 = Vector4.Dot(A, l1);
		float n2 = Vector4.Dot(A, l2);
		float n3 = Vector4.Dot(A, l3);
		float n4 = Vector4.Dot(A, l4);
		Vector3 ans = n1 * a + n2 * b + n3 * c + n4 * d;
		return ans;
	}

	void UpdateColor(int hottestPoint, float temp) {
		Gradient colorGrad = new Gradient();
		float t = lengthAt[hottestPoint] / sumLength;
		int leftAnchor = hottestPoint - HeatUpInfluence;
		int rightAnchor = hottestPoint + HeatUpInfluence;
		if (leftAnchor < 0)
			leftAnchor = 0;
		if (rightAnchor > line.Length - 1)
			rightAnchor = line.Length - 1;
		
		float lt = lengthAt[leftAnchor] / sumLength;
		float rt = lengthAt[rightAnchor] / sumLength;

		float vibrant = Mathf.Sqrt(temperature[hottestPoint] / (breakThreshold * 2 / 3));
		// Debug.Log("T = " + t + " Point" + hottestPoint);
		colorGrad.SetKeys(
            new GradientColorKey[] { 
				new GradientColorKey(Color.black, 0.0f), 
				new GradientColorKey(Color.black, lt), 
				new GradientColorKey(Color.red * vibrant, t), 
				new GradientColorKey(Color.black, rt),
				new GradientColorKey(Color.black, 1.0f) 
				},
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
		lineRenderer.colorGradient = colorGrad;
	}
	
	// Update is called once per frame
	[SerializeField] bool colorTest = false;
	[SerializeField] bool breakTest = false;
	int colorTestPoint = 0;
	void Update () {
		if (Time.renderedFrameCount - lastShot > 20) {
			laserCuttingEffect.Stop();
		}
		else {
			laserCuttingEffect.Play();
		}

		PointList[0].transform.position = StartPoint.position;
		PointList[PointList.Length - 1].transform.position = EndPoint.position;
		line[0] = PointList[0].transform.position;
		sumLength = 0;
		for (int i = 0; i < SegmentCount; i++) {
			// Debug.Log(i + 1);
			// Debug.Log(PointList[i + 1]);
			// line[i + 1] = PointList[i + 1].transform.position;
			for (int j = 0; j < RenderQuality - 1; j++) {
				float t = ((float)j + 1) / RenderQuality;
				Vector3 pos = CatmullRom(
					t, CatmullTension,
					i >= 1 ? PointList[i - 1].transform.position : PointList[0].transform.position - PointList[0].transform.forward * CatmullResidue,
					PointList[i].transform.position,
					PointList[i + 1].transform.position,
					i + 2 <= SegmentCount ? PointList[i + 2].transform.position : PointList[SegmentCount].transform.position + PointList[SegmentCount].transform.forward * CatmullResidue
				);
				line[i * RenderQuality + j + 1] = pos;
				lengthAt[i * RenderQuality + j + 1] = lengthAt[i * RenderQuality + j] + Vector3.Distance(line[i * RenderQuality + j], line[i * RenderQuality + j + 1]);
			}
			line[(i + 1) * RenderQuality] = PointList[i + 1].transform.position;
			lengthAt[(i + 1) * RenderQuality] = lengthAt[(i + 1) * RenderQuality - 1] + Vector3.Distance(line[(i + 1) * RenderQuality - 1], line[(i + 1) * RenderQuality]);
		}
		sumLength = lengthAt[SegmentCount * RenderQuality];
		lineRenderer.SetPositions(line);

		if (isBroken) {
			UpdateColor(colorTestPoint, 0);
			return;
		}
		else {
			if (breakTest && Input.GetKeyDown(KeyCode.B)) {
				BreakAt(210);
			}
		}

		if (colorTest) {
			if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				if (colorTestPoint > 0)
					colorTestPoint -= RenderQuality;
			}
			if (Input.GetKeyDown(KeyCode.RightArrow)) {
				if (colorTestPoint < RenderQuality * SegmentCount)
					colorTestPoint += RenderQuality;
			}
			UpdateColor(colorTestPoint, 1000);
		}
		else {

			CoolDown();

			float maxTemp = 0;
			int idx = 0;
			for (int i = 0; i < line.Length; i++) {
				if (temperature[i] > maxTemp) {
					maxTemp = temperature[i];
					idx = i;
				}
			}
			if (maxTemp > breakThreshold) {
				BreakAt(idx);
			}
			else
				UpdateColor(idx, maxTemp);
		}
	}
}
