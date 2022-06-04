using UnityEngine;

namespace Betaal
{
	public class BetaalBodyCollider : MonoBehaviour
	{
		private BetaalController _controller;

		private void Start()
		{
			_controller = transform.root.GetComponent<BetaalController>();
		}

		private void OnCollisionEnter(Collision other)
		{
			if(!other.collider.CompareTag("Betaal")) return;
		}
	}
}
