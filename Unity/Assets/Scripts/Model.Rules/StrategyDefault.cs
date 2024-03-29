﻿using System;
using App;
using Model.Data;
using UnityEngine;

namespace Model.Rules
{
	public class StrategyDefault : IStrategyGame
	{
		// TODO: logger might be overriden also

		private readonly IContext _context;

		public StatusMatch Status { get; private set; }

		public StrategyDefault(IContext context)
		{
			_context = context;
		}

		// IStrategyGame
		public void UpdateModel(Repository repository)
		{
			Status = StatusMatch.InProgress;

			//? all the test should perform by stationary frame
			//? should update bought prior testing

			// reflectors
			_context.Resolve<IReflectorDriverRemote>().UpdateReflector(repository, repository.DataReflectorRemote);
			_context.Resolve<IReflectorDriverLocal>().UpdateReflector(repository, repository.DataReflectorLocal);

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

						Status = StatusMatch.WinLocal;

						repository.ScoreInc();

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

						Status = StatusMatch.WinRemote;
						return;
					}
				}

				repository.DataBall.Position += repository.DataBall.Speed * repository.DataTime.SimStep;
			}
		}
	}
}
