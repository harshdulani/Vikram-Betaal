using System;
using DG.Tweening;

public static partial class GameEvents
{
	public static event Action GameplayStart;
	public static event Action ConversationStart;
	public static event Action IntroConversationComplete;
	public static event Action InteractWithBetaal, BetaalConversationStart;
}

public static partial class GameEvents
{
	public static void InvokeGameplayStart() => GameplayStart?.Invoke();
	public static void InvokeConversationStart() => ConversationStart?.Invoke();

	public static void InvokeIntroConversationComplete() => IntroConversationComplete?.Invoke();
	public static void InvokeInteractWithBetaal() => InteractWithBetaal?.Invoke();
	public static void InvokeBetaalConversationStart() => BetaalConversationStart?.Invoke();
}