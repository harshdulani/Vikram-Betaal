using System;

public static partial class GameEvents
{
	public static event Action GameplayStart;
	public static event Action ConversationStart;
	public static event Action IntroConversationComplete;
}

public static partial class GameEvents
{
	public static void InvokeGameplayStart() => GameplayStart?.Invoke();
	public static void InvokeConversationStart() => ConversationStart?.Invoke();

	public static void InvokeIntroConversationComplete() => IntroConversationComplete?.Invoke();
}