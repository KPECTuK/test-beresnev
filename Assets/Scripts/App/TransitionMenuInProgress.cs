using Model;
using UnityEngine;

namespace App
{
	public class TransitionMenuInProgress : ITransition
	{
		private ControllerScreenMenu _screen;

		public TransitionMenuInProgress(ControllerScreenMenu screen)
		{
			_screen = screen;
		}

		public bool Execute(Repository repository)
		{
			repository.GenerateMeta();
			repository.GenerateMatch();

			Debug.Log("menu done");

			return true;
		}

		public void Release()
		{
			_screen = null;
		}
	}
}
