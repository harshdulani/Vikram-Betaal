using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class OldieAttack
{
	public GameObject attackPrefab, explosionPrefab;
	public Transform attackSpawnTransform;

	public float attackSpeed, delayBeforeLaunchAttack;

	public virtual void StartAttack(Transform player) { }
}

[System.Serializable]
public class ProjectileAttack : OldieAttack
{
	public override void StartAttack(Transform player)
	{
		var projectile = Object.Instantiate(attackPrefab, attackSpawnTransform.position, Quaternion.identity);

		var pTransform = projectile.transform;

		var attackPos = player.position + Vector3.up * 2f;
		
		pTransform.rotation = Quaternion.LookRotation(attackPos - pTransform.position);
		pTransform.DOMoveY(pTransform.position.y + 1.5f, 0.5f)
				  .SetDelay(delayBeforeLaunchAttack)
				  .OnComplete(() =>
								  pTransform.DOMove(attackPos, attackSpeed)
											.SetEase(Ease.Linear)
											.OnComplete(() =>
														{
															Object.Instantiate(explosionPrefab, pTransform.position, Quaternion.identity);
															projectile.gameObject.SetActive(false);
														}));
	}
}

[System.Serializable]
public class ProximityAttack : OldieAttack
{
	public override void StartAttack(Transform player)
	{
		var projectile = Object.Instantiate(attackPrefab, attackSpawnTransform.position, Quaternion.identity);

		var pTransform = projectile.transform;

		var attackPos = player.position + Vector3.up * 1f;
		
		pTransform.DOMove(attackPos, attackSpeed)
				  .SetEase(Ease.Linear)
				  .SetDelay(delayBeforeLaunchAttack)
				  .OnComplete(() =>
							  {
								  Object.Instantiate(explosionPrefab, pTransform.position, Quaternion.identity);
								  projectile.gameObject.SetActive(false);
							  });
	}
}