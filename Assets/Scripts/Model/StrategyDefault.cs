using System;
using UnityEngine;

namespace Model
{
	// provider - the same
	public interface IContext
	{
		T Resolve<T>() where T : class;
	}

	public interface IReflectorDriver
	{
		void UpdateReflector(Repository repository, ModelReflector reflector);
	}

	public interface IStrategyGame
	{
		GameStatus Status { get; }
		void UpdateModel(Repository repository);
	}

	public interface IInputProvider
	{
		bool IsInputLeft();
		bool IsInputRight();
	}

	public class InputProviderLocalForLocal : IInputProvider
	{
		public bool IsInputLeft()
		{
			return ExtensionInput.IsLocalLeft();
		}

		public bool IsInputRight()
		{
			return ExtensionInput.IsLocalRight();
		}
	}

	public class InputProviderLocalForRemote : IInputProvider
	{
		public bool IsInputLeft()
		{
			return ExtensionInput.IsRemoteLeft();
		}

		public bool IsInputRight()
		{
			return ExtensionInput.IsRemoteRight();
		}
	}

	public class ReflectorDriverInput : IReflectorDriver
	{
		private readonly IInputProvider _inputProvider;

		public ReflectorDriverInput(IInputProvider inputProvider)
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

	public enum GameStatus
	{
		InProgress,
		WinRemote,
		WinLocal,
	}

	public class StrategyDefault : IStrategyGame
	{
		// TODO: logger might be overriden also

		private readonly IReflectorDriver _driverRemote = new ReflectorDriverInput(new InputProviderLocalForRemote());
		private readonly IReflectorDriver _driverLocal = new ReflectorDriverInput(new InputProviderLocalForLocal());

		public GameStatus Status { get; private set; }

		// IStrategyGame
		public void UpdateModel(Repository repository)
		{
			Status = GameStatus.InProgress;

			//? all the test should perform by stationary frame
			//? should update bought prior testing

			// reflectors
			_driverRemote.UpdateReflector(repository, repository.DataReflectorRemote);
			_driverLocal.UpdateReflector(repository, repository.DataReflectorLocal);

			// ball
			unsafe
			{
				fixed(ModelSegment* segmentsPtr = repository.DataCourt.LeftSide)
				{
					//? LimitReflect
					var size = repository.DataCourt.LeftSide.Length;
					if(repository.TestReflect(segmentsPtr, size))
					{
						// reflect border
						Debug.Log("bounce border LEFT");
						return;
					}
				}

				fixed(ModelSegment* segmentsPtr = repository.DataCourt.RightSide)
				{
					//? LimitReflect
					var size = repository.DataCourt.RightSide.Length;
					if(repository.TestReflect(segmentsPtr, size))
					{
						// reflect border
						Debug.Log("bounce border RIGHT");
						return;
					}
				}

				fixed(ModelSegment* segmentsPtr = repository.DataReflectorLocal.Shape)
				{
					var size = repository.DataReflectorLocal.Shape.Length;
					var bufferPtr = stackalloc ModelSegment[size];
					Buffer.MemoryCopy(segmentsPtr, bufferPtr, sizeof(ModelSegment) * size, sizeof(ModelSegment) * size);
					for(var index = 0; index < size; index++)
					{
						bufferPtr[index].V0.x *= repository.DataReflectorLocal.HalfSize;
						bufferPtr[index].V0 += repository.DataReflectorLocal.Position;
						bufferPtr[index].V1.x *= repository.DataReflectorLocal.HalfSize;
						bufferPtr[index].V1 += repository.DataReflectorLocal.Position;
					}
					if(repository.TestReflect(bufferPtr, size))
					{
						// reflect local
						Debug.Log("bounce reflector LOCAL");
						return;
					}
				}

				fixed(ModelSegment* segmentsPtr = repository.DataReflectorRemote.Shape)
				{
					var size = repository.DataReflectorRemote.Shape.Length;
					var bufferPtr = stackalloc ModelSegment[size];
					Buffer.MemoryCopy(segmentsPtr, bufferPtr, sizeof(ModelSegment) * size, sizeof(ModelSegment) * size);
					for(var index = 0; index < size; index++)
					{
						bufferPtr[index].V0.x *= repository.DataReflectorRemote.HalfSize;
						bufferPtr[index].V0 += repository.DataReflectorRemote.Position;
						bufferPtr[index].V1.x *= repository.DataReflectorRemote.HalfSize;
						bufferPtr[index].V1 += repository.DataReflectorRemote.Position;
					}

					if(repository.TestReflect(bufferPtr, size))
					{
						// reflect remote
						Debug.Log("bounce reflector REMOTE");
						return;
					}
				}

				fixed(ModelSegment* segmentsPtr = repository.DataCourt.RemoteGoal)
				{
					var size = repository.DataCourt.RemoteGoal.Length;
					if(repository.TestReflect(segmentsPtr, size))
					{
						//! wrong in out of court - use if below reflector
						Debug.Log("bounce GOAL REMOTE");

						Status = GameStatus.WinRemote;
						return;
					}
				}

				fixed(ModelSegment* segmentsPtr = repository.DataCourt.LocalGoal)
				{
					var size = repository.DataCourt.LocalGoal.Length;
					if(repository.TestReflect(segmentsPtr, size))
					{
						//! wrong in out of court - use if below reflector
						Debug.Log("bounce GOAL LOCAL");

						Status = GameStatus.WinLocal;
						return;
					}
				}

				repository.DataBall.Position += repository.DataBall.Speed * repository.DataTime.SimStep;
			}
		}
	}
}
