using System;

public static partial class GameEvents
{
	public static event Action GameplayStart;
	public static event Action IntroConversationComplete;

}

public static partial class GameEvents
{
	public static void InvokeGameplayStart() => GameplayStart?.Invoke();
	public static void InvokeIntroConversationComplete() => IntroConversationComplete?.Invoke();
}