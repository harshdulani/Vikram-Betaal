using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Betaal
{
	[System.Serializable]
	public class BetaalHandleAttack
	{
		[SerializeField] private bool drawAttackDebugSphere;
		[HideInInspector] public Vector3 attackPos;
		public float maxDistanceForAttack, shakeTime, shakeStrength, perAttackInterval, postAttackInterval;

		[Header("VFX"), SerializeField] private GameObject lightingPrefab;
		[SerializeField] private Transform leftHand, rightHand;

		private static int _currentAttackerIndex = 0;
		private static readonly List<BoltBeginPair> BoltsAndBegins = new List<BoltBeginPair>();
		
		private readonly BetaalController _betaal;

		public BetaalHandleAttack(BetaalController betaal)
		{
			_betaal = betaal;
			_currentAttackerIndex = 0;
			BoltsAndBegins.Clear();
		}
		
		public void DrawGizmos()
		{
			if(!drawAttackDebugSphere) return;
			Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
			Gizmos.DrawSphere(_betaal.transform.position, maxDistanceForAttack);
		}

		public int IncreaseHandleCount(Transform handle)
		{
			var position = handle.position;
			var leftHandDistance = (position - leftHand.position).sqrMagnitude;
			var rightHandDistance = (position - rightHand.position).sqrMagnitude;

			var beginParent = leftHandDistance < rightHandDistance ? leftHand : rightHand;
			var pair = new BoltBeginPair(null, beginParent);
			GetLightningInstance(ref pair);

			var lightningBeginT = pair.Bolt.transform.GetChild(0);
			lightningBeginT.parent = pair.BeginHand;
			lightningBeginT.localPosition = Vector3.zero;
			
			//lightning endT is child 0 because child 0 is unparented
			var lightningEndT = pair.Bolt.transform.GetChild(0);
			lightningEndT.parent = handle;
			lightningEndT.localPosition = Vector3.down * 0.3f;
			
			return ++_currentAttackerIndex;
		}

		private void GetLightningInstance(ref BoltBeginPair pair)
		{
			/*
			var availablePair = 
				BoltsAndBegins.Find(instance => instance.Bolt.activeSelf == false);
			
			//problem here because both begin and end become children of different untracked transforms
			if (availablePair.Bolt)
			{
				pair = availablePair;
				pair.BeginHand.parent = pair.Bolt.transform;
				pair.BeginHand.localPosition = Vector3.zero;
			}
			else
			{*/
				pair.Bolt = Object.Instantiate(lightingPrefab, _betaal.transform);
				BoltsAndBegins.Add(pair);
			//}
		}
		
		public void EndHandleAttackControl(Transform handle)
		{
			_currentAttackerIndex--;
			
			handle.GetChild(handle.childCount - 1).gameObject.SetActive(false);
			BoltsAndBegins[0].Bolt.SetActive(false);
			BoltsAndBegins.RemoveAt(0);

			/*
			_currentAttackerIndex--;

			var currentBolt = BoltsAndBegins[0];
			
			var boltBegin = currentBolt.BeginHand.GetChild(currentBolt.BeginHand.childCount - 1);
			var boltEnd = handle.GetChild(handle.childCount - 1);

			//get back last child, that would be assigned on run time as the lightning end T and parent it to the lightning instance
			boltEnd.parent = currentBolt.Bolt.transform;
			boltEnd.localPosition = Vector3.zero;
			
			//the lightning end T is parented to a hand, so now parent it to the bolt itself
			boltBegin.parent = currentBolt.Bolt.transform;
			boltBegin.localPosition = Vector3.zero;
			currentBolt.BeginHand = null;

			//turn off the instance to have it return to the pool //this does not make sense as of now
			currentBolt.Bolt.SetActive(false);*/
		}
	}

	public struct BoltBeginPair
	{
		public GameObject Bolt { get; set; }
		public Transform BeginHand { get; set; }

		public BoltBeginPair(GameObject bolt, Transform begin)
		{
			Bolt = bolt;
			BeginHand = begin;
		}
	}
}