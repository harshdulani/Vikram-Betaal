using System.Collections.Generic;
using UnityEngine;

public class RandomiseTrees : MonoBehaviour
{
	[SerializeField] private Transform treeParent;

	[SerializeField] private Vector3[] randomScales;
	[SerializeField] private List<float> randomRotations;

	private readonly List<Transform> _trees = new List<Transform>();
	
	private int _randomIndex;

	private void Start()
	{
		for (var i = 0; i < treeParent.childCount; i++) _trees.Add(treeParent.GetChild(i));
	}

	public void Randomise()
	{
		foreach (var tree in _trees)
		{
			tree.localScale = randomScales[_randomIndex % randomScales.Length];
			
			tree.localRotation = Quaternion.LookRotation(Vector3.up * randomRotations[_randomIndex++ % randomRotations.Count]);
		}
	}
}
