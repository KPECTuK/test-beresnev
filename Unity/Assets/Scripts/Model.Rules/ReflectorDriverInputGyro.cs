using Model.Data;
using Service.Input;
using UnityEngine;

namespace Model.Rules
{
	public class ReflectorDriverInputGyro : IReflectorDriverLocal, IReflectorDriverRemote
	{
		private readonly IInputProvider _inputProvider;

		public ReflectorDriverInputGyro(IInputProvider inputProvider)
		{
			_inputProvider = inputProvider;
		}

		public void UpdateReflector(Repository repository, ModelReflector reflector)
		{
			if(_inputProvider.IsInputLeft())
			{
				reflector.Position += Vector2.left * repository.DataTime.SimStep;
			}

			if(_inputProvider.IsInputRight())
			{
				reflector.Position += Vector2.right * repository.DataTime.SimStep;
			}

			// if the network driver - strategy can't limit reflector
			//? if the local driver - strategy could limit, but that's not generic
			repository.Limit(reflector);
		}
	}
}
