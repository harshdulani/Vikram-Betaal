using System;

public static partial class GameEvents
{
	public static event Action GameplayStart;
	public static event Action ConversationStart, ConversationEnd;
	public static event Action IntroConversationComplete;
	public static event Action InteractWithBetaal;
	public static event Action BetaalFightStart;
}

public static partial class GameEvents
{
	public static void InvokeGameplayStart() => GameplayStart?.Invoke();
	public static void InvokeConversationStart() => ConversationStart?.Invoke();

	public static void InvokeIntroConversationComplete() => IntroConversationComplete?.Invoke();
	public static void InvokeInteractWithBetaal() => InteractWithBetaal?.Invoke();
	public static void InvokeBetaalFightStart() => BetaalFightStart?.Invoke();
	public static void InvokeConversationEnd() => ConversationEnd?.Invoke();
}