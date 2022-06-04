using System;

namespace Betaal
{
	public static class BetaalEvents
	{
		public static event Action<BetaalController> StartHandleAttack;

		public static void InvokeStartHandleAttack(BetaalController betaal) => StartHandleAttack?.Invoke(betaal);
	}
}