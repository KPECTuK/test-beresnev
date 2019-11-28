using Model;

namespace App
{
	public class TransitionScreen : ITransition
	{
		private ControllerScreen _from;
		private ControllerScreen _to;

		public TransitionScreen(ControllerScreen from, ControllerScreen to)
		{
			_from = from;
			_to = to;
		}

		public bool Execute(Repository repository)
		{
			return _from.FadeOut() && _to.FadeIn();
		}

		public void Release()
		{
			_from = null;
			_to = null;
		}
	}
}
