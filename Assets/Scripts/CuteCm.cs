using Cinemachine;
using UnityEngine;

public class CuteCm : MonoBehaviour
{
	public static CuteCm only;
	
	private CinemachineStateDrivenCamera _me;
	private Animator _anim;
	private static readonly int StartIntroConv = Animator.StringToHash("startIntroConv");
	private static readonly int EndIntroConv = Animator.StringToHash("endIntroConv");
	private static readonly int InFightCam = Animator.StringToHash("inFightCam");
	private static readonly int End = Animator.StringToHash("endDialogue");

	private void Awake()
	{
		if (only) Destroy(gameObject);
		else only = this;
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
		_me = GetComponent<CinemachineStateDrivenCamera>();
	}

	public void SetStartIntroConv() => _anim.SetTrigger(StartIntroConv);
	public void SetEndIntroConv() => _anim.SetTrigger(EndIntroConv);
	public void UpdateInFightCam(bool b) => _anim.SetBool(InFightCam, b);
	public void EndDialogue() => _anim.SetTrigger(End);
}