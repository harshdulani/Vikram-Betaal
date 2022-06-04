using UnityEngine;

namespace Betaal
{
	[System.Serializable]
	public class BetaalHandleAttack
	{
		[SerializeField] private bool drawAttackDebugSphere;
		[HideInInspector] public Vector3 attackPos;
		public float maxDistanceForAttack, shakeTime, shakeStrength, postAttackInterval;

		private readonly BetaalController _betaal;
		public BetaalHandleAttack(BetaalController betaal) => _betaal = betaal;

		public void DrawGizmos()
		{
			if(!drawAttackDebugSphere) return;
			Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
			Gizmos.DrawSphere(_betaal.transform.position, maxDistanceForAttack);
		}
	}
}