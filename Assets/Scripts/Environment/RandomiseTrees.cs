using System.Collections.Generic;
using UnityEngine;

public class RandomiseTrees : MonoBehaviour
{
	[SerializeField] private Transform treeParent;

	[SerializeField] private Vector3[] randomScales;
	[SerializeField] private List<float> randomRotations, randomYPositions;

	private readonly List<Transform> _trees = new List<Transform>();
	
	private int _randomIndex;

	private void Start()
	{
		for (var i = 0; i < treeParent.childCount; i++) _trees.Add(treeParent.GetChild(i));
		
		/*
		for (var i = 0; i <= 36; i++) randomRotations.Add(Mathf.Lerp(0, 360, i / 36f));

		for (var i = 0; i < 20; i++) randomYPositions.Add(Mathf.Lerp(0f, -2f, i / 20f));
		*/
	}

	public void Randomise()
	{
		foreach (var tree in _trees)
		{
			tree.localScale = randomScales[_randomIndex % randomScales.Length];
			
			/*
			var pos = tree.localPosition;
			pos.x = randomYPositions[_randomIndex % randomYPositions.Count];
			tree.localPosition = pos;*/
			
			tree.localRotation = Quaternion.LookRotation(Vector3.up * randomRotations[_randomIndex++ % randomRotations.Count]);
		}
	}
}
