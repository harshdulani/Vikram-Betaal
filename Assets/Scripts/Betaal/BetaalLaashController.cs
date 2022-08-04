using System.Collections.Generic;
using DG.Tweening;
using Player;
using TMPro;
using UnityEngine;

namespace Betaal
{
	public class BetaalLaashController : MonoBehaviour
	{
		[SerializeField] private GameObject betaal;
		[SerializeField] private List<Rigidbody> rigidbodies;
		[SerializeField] private SkinnedMeshRenderer rend;

		[SerializeField] private float colorTweenDuration;
		[SerializeField] private TextMeshPro text;
		[SerializeField] private float blinkDuration;

		private PlayerController _player;
		private Animator _anim;
		private BetaalBackArms _arms;

		private Color _originalSkinColor, _originalEmission;
		private Tween _textColorTween;
		private Vector3 _initPos;
		private bool _hasBeenInteractedWith;
		
		private static readonly int EmissiveColor = Shader.PropertyToID("_EmissiveColor");

		private void OnEnable()
		{
			GameEvents.InteractWithBetaal += OnInteractWithBetaal;
		}
		
		private void OnDisable()
		{
			GameEvents.InteractWithBetaal -= OnInteractWithBetaal;
		}

		private void Start()
		{
			_player = GameObject.FindGameObjectWithTag("Player").transform.root.GetComponent<PlayerController>();
			_arms = GetComponent<BetaalBackArms>();
			_anim = GetComponent<Animator>();
			_initPos = transform.position;

			DOVirtual.DelayedCall(5f, GoRagdoll);
			
			_arms.SendToCeiling();

			_originalSkinColor = rend.materials[0].color;
			_originalEmission = rend.materials[0].GetColor(EmissiveColor);
			
			rend.materials[0].SetColor(EmissiveColor, Color.black);
			rend.materials[0].color = Color.white;
			
			float GetCharSpacing() => text.characterSpacing;
			void SetCharSpacing(float value) => text.characterSpacing = value;
			
			DOTween.To(GetCharSpacing, SetCharSpacing, -36, blinkDuration)
								 .SetLoops(-1, LoopType.Yoyo)
								 .SetRecyclable(true)
								 .SetAutoKill(false);
			
			text.color = Color.clear;
			
		}

		private void GoRagdoll()
		{
			transform.position = _initPos;

			DOVirtual.DelayedCall(0.12f, () =>
										 {
											 _anim.enabled = false;
											 foreach (var rb in rigidbodies) rb.isKinematic = false;
										 });
		}

		private void OnInteractWithBetaal()
		{
			if(_hasBeenInteractedWith) return;
			
			_hasBeenInteractedWith = true;
			rend.materials[0].DOColor(_originalSkinColor, colorTweenDuration).SetEase(Ease.InOutElastic);

			DOTween.To(() => rend.materials[0].GetColor(EmissiveColor),
					   value => rend.materials[0].SetColor(EmissiveColor, value),
					   _originalEmission, colorTweenDuration)
				   .SetEase(Ease.InOutElastic)
				   .OnComplete(() =>
							   {
								   Lightning.only.CustomLightning(3500000);
								   DOVirtual.DelayedCall(0.15f, () =>
																{
																	gameObject.SetActive(false);
																	betaal.SetActive(true);
																});
								   GameEvents.InvokeConversationStart();
							   });
		}

		private void TurnInteractionUiStatus(bool status)
		{
			if(_textColorTween.IsActive()) _textColorTween.Kill();
			_textColorTween = text.DOColor(status ? Color.white : Color.clear, 0.5f);
		}
		
		private void OnTriggerEnter(Collider other)
		{
			if(!other.CompareTag("Player")) return;
			
			TurnInteractionUiStatus(_player.TryInteractWithBetaalStatusChange(true));
		}

		private void OnTriggerExit(Collider other)
		{
			if(!other.CompareTag("Player")) return;

			TurnInteractionUiStatus(false);
		}
	}
}