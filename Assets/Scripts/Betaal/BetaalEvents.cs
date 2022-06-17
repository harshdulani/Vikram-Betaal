using System;

namespace Betaal
{
	public static class BetaalEvents
	{
		public static event Action<BetaalController> StartHandleAttack;
		public static event Action StartBetaalAttack, EndBetaalAttack;

		public static void InvokeStartHandleAttack(BetaalController betaal) => StartHandleAttack?.Invoke(betaal);
		public static void InvokeEndBetaalAttack() => EndBetaalAttack?.Invoke();
		public static void InvokeStartBetaalAttack() => StartBetaalAttack?.Invoke();
	}
}