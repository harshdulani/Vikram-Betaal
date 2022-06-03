using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private Rigidbody[] rigidbodies;
		[SerializeField] private float ragdollThrowBackForce;
		
		[SerializeField] private int lPunchDamage = 20, lUppercutDamage = 40;

		private Animator _anim;

		private void Awake()
		{
			DOTween.KillAll();
		}

		private void Start()
		{
			_anim = GetComponent<Animator>();
		}

		private void Update()
		{
			if (Input.GetKeyUp(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			if (Input.GetKeyUp(KeyCode.E)) GoRagdoll();
		}

		public int GetAttackDamage(PlayerAttackType type)
		{
			return type switch
				   {
					   PlayerAttackType.None => 0,
					   PlayerAttackType.LPunch => lPunchDamage,
					   PlayerAttackType.LUppercut => lUppercutDamage,
					   _ => throw new ArgumentOutOfRangeException(nameof (type), type, null)
				   };
		}
		
		private void GoRagdoll()
		{
			_anim.enabled = false;
			foreach (var rb in rigidbodies)
			{
				rb.isKinematic = false;
				rb.AddForce(-transform.forward * ragdollThrowBackForce, ForceMode.Impulse);
			}
		}
	}
}