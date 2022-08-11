using UnityEngine;

namespace Oldie
{
	public class OldieRefBank : MonoBehaviour
	{
		public bool IsDead { get; set; }
		public OldieCombatController Combat { get; private set; }
		public OldieMovement Movement { get; private set; }
		public Animator Animator { get; private set; }

		private void Start()
		{
			Animator = GetComponent<Animator>();
		
			Combat = GetComponent<OldieCombatController>();
			Movement = GetComponent<OldieMovement>();
		}
	}
}