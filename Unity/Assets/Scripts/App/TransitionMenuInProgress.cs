using Model.Data;
using Model.Rules;
using Service.Network;

namespace App
{
	public class TransitionMenuInProgress : ITransition
	{
		private ControllerScreenMenu _screen;
		private ServiceNetwork _service;

		public TransitionMenuInProgress(ControllerScreenMenu screen, ServiceNetwork service)
		{
			_screen = screen;
			_service = service;
		}

		public bool Execute(Repository repository)
		{
			//repository.GenerateMatch();
			_screen.Render(repository);
			return _service.ConnectionReady;
		}

		public void Release()
		{
			_screen = null;
		}
	}
}
