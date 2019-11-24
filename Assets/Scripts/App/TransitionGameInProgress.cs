using Model;

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
			_screen.Set(repository);

			return _strategy.CheckComplete(repository);
		}

		public void Release()
		{
			_screen = null;
			_strategy = null;
		}
	}
}
