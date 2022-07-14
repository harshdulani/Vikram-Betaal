using Player;
using UnityEngine;

namespace Betaal
{
	public class BetaalArmEndCollider : MonoBehaviour
	{
		private static PlayerController _player;
		private BetaalBackArms _me;

		private void Start()
		{
			_me = transform.root.GetComponent<BetaalBackArms>();
		}

		private void OnCollisionEnter(Collision other)
		{
			if(!_me.isAttacking) return;
			if(!other.collider.CompareTag("Player")) return;

			if (!_player) _player = other.transform.root.GetComponent<PlayerController>();
			
			_player.GetHit(other.relativeVelocity);
		}
	}
}