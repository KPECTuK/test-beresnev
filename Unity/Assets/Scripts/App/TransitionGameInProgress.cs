using Model.Data;
using Model.Rules;

namespace App
{
	public class TransitionGameInProgress : ITransition
	{
		private IStrategyGame _strategy;
		private ControllerScreenGame _screen;

		public TransitionGameInProgress(ControllerScreenGame screen, IStrategyGame strategy)
		{
			_screen = screen;
			_strategy = strategy;
		}

		public bool Execute(Repository repository)
		{
			_strategy.UpdateModel(repository);
			_screen.Render(repository);
			return _strategy.Status != StatusMatch.InProgress;
		}

		public void Release()
		{
			_screen = null;
			_strategy = null;
		}
	}
}
