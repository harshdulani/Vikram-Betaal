using DG.Tweening;
using Player;
using TMPro;
using UnityEngine;

namespace Betaal
{
	public class PickupBetaal : MonoBehaviour
	{
		[SerializeField] private TextMeshPro text;

		private PlayerController _player;

		private Tween _textColorTween;
		private Transform _camera;


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
			_camera = Camera.main.transform;
			_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
			_player.allowedInteractionWithBetaalChange = true;
		}

		private void Update()
		{
			text.transform.rotation = Quaternion.LookRotation(transform.position - _camera.position);
		}

		private void OnInteractWithBetaal()
		{
			Lightning.only.CustomLightning(3500000);
			DOVirtual.DelayedCall(0.15f, () =>
										 {
											 transform.parent.gameObject.SetActive(false);
											 _player.TurnCarriedBetaalOn(gameObject);
										 });
			GameEvents.InvokeConversationStart();
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