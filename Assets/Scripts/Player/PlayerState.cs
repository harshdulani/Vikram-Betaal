using Cinemachine;
using Player.Combat;
using Player.Movement;
using UnityEngine;
using UnityEngine.AI;

public enum PlayerAttackType { None, LPunch, LUppercut }

namespace Player
{
	public class PlayerState : MonoBehaviour
	{
		public PlayerInput Combat { get; private set; }
		public PlayerController Controller { get; private set; }
		public PlayerMovement Movement { get; private set; }
		public NavMeshAgent Agent { get; private set; }
		public CinemachineImpulseSource Impulse { private set; get; }
		public PlayerAttackType CurrentAttackType { get; set; }

		public Transform chestCollider, headCollider;

		public float maxHealth, currentHealth;
		
		public bool inCombat, disableMovementByAnimation, isBlocking;

		public void AllowMovement() => disableMovementByAnimation = false;

		public void DisallowMovement()  => disableMovementByAnimation = true;

		private void Awake()
		{
			Movement = GetComponent<PlayerMovement>();
			Combat = GetComponent<PlayerInput>();
			Controller = GetComponent<PlayerController>();
			Agent = GetComponent<NavMeshAgent>();
			Impulse = GetComponent<CinemachineImpulseSource>();

			currentHealth = maxHealth;
		}

		public bool CanGetAttacked() => !isBlocking;
	}
}
