using UnityEngine;

public class CuteCm : MonoBehaviour
{
	public static CuteCm only;
	
	private Animator _anim;
	private static readonly int StartIntroConv = Animator.StringToHash("startIntroConv");
	private static readonly int EndIntroConv = Animator.StringToHash("endIntroConv");
	private static readonly int InFightCam = Animator.StringToHash("inFightCam");
	private static readonly int End = Animator.StringToHash("endDialogue");

	private void OnEnable()
	{
		GameEvents.GameLose += OnFightEnd;
		GameEvents.BetaalFightEnd += OnFightEnd;
	}

	private void OnDestroy()
	{
		GameEvents.GameLose -= OnFightEnd;
		GameEvents.BetaalFightEnd -= OnFightEnd;
	}

	private void Awake()
	{
		if (only) Destroy(gameObject);
		else only = this;
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
	}

	public void SetStartIntroConv() => _anim.SetTrigger(StartIntroConv);
	public void SetEndIntroConv() => _anim.SetTrigger(EndIntroConv);
	public void UpdateInFightCam(bool b) => _anim.SetBool(InFightCam, b);
	public void EndDialogue() => _anim.SetTrigger(End);

	private void OnFightEnd() => _anim.SetBool(InFightCam, false);
	private void OnFightEnd(bool obj) => _anim.SetBool(InFightCam, false);
}