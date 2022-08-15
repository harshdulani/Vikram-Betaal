using System;

public static partial class GameEvents
{
	public static event Action GameplayStart, GameLose, GameWin;
	public static event Action ConversationStart, ConversationEnd;
	public static event Action IntroConversationComplete;
	public static event Action InteractWithBetaal;
	public static event Action BetaalFightStart, SadhuFightStart;
	public static event Action<bool> BetaalFightEnd;
}

public static partial class GameEvents
{
	public static void InvokeGameplayStart() => GameplayStart?.Invoke();
	public static void InvokeConversationStart() => ConversationStart?.Invoke();

	public static void InvokeIntroConversationComplete() => IntroConversationComplete?.Invoke();
	public static void InvokeInteractWithBetaal() => InteractWithBetaal?.Invoke();
	public static void InvokeBetaalFightStart() => BetaalFightStart?.Invoke();
	public static void InvokeConversationEnd() => ConversationEnd?.Invoke();
	public static void InvokeBetaalFightEnd(bool isTemporary = false) => BetaalFightEnd?.Invoke(isTemporary);
	
	public static void InvokeSadhuFightStart() => SadhuFightStart?.Invoke();
	public static void InvokeGameLose() => GameLose?.Invoke();
	public static void InvokeGameWin() => GameWin?.Invoke();
}