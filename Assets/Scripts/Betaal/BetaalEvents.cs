using System;

namespace Betaal
{
	public static class BetaalEvents
	{
		public static event Action<BetaalController> StartHandleAttack;
		public static event Action StartBetaalArmsAttack, EndBetaalArmsAttack, EndBetaalHandleAttack;

		public static void InvokeStartHandleAttack(BetaalController betaal) => StartHandleAttack?.Invoke(betaal);
		public static void InvokeEndBetaalArmsAttack() => EndBetaalArmsAttack?.Invoke();
		public static void InvokeEndBetaalHandleAttack() => EndBetaalHandleAttack?.Invoke();
		public static void InvokeStartBetaalArmsAttack() => StartBetaalArmsAttack?.Invoke();
	}
}