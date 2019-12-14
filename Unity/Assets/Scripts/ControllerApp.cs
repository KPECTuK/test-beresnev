using System;
using App;
using Model.Data;
using Model.Rules;
using Service.Input;
using Service.Network;
using UnityEngine;

public sealed class ControllerApp : MonoBehaviour, IContext
{
	private ControllerScreenGame _screenGame;
	private ControllerScreenMenu _screenMenu;

	private int _indexTransition;
	private ITransition[] _fsm;

	private Repository _repository;
	private ServiceNetwork _service;

	private readonly IReflectorDriverLocal _local = new ReflectorDriverInputGyro(new InputProviderLocalForLocalGyro());
	private readonly IReflectorDriverRemote _remote = new ReflectorDriverInputGyro(new InputProviderLocalForRemote());

	private void Awake()
	{
		_screenGame = transform.Find("screen-game")?.gameObject.AddComponent<ControllerScreenGame>() ?? throw new Exception("can't find screen GAME");
		_screenMenu = transform.Find("screen-menu")?.gameObject.AddComponent<ControllerScreenMenu>() ?? throw new Exception("can't find screen MENU");

		_repository = new Repository();
		//! keep order
		_repository.GenerateMeta();
		_repository.GenerateSystem();
		_service = new ServiceNetwork(this);
		_service.Start();

		_fsm = new ITransition[]
		{
			new TransitionScreen(_screenGame, _screenMenu),
			new TransitionMenuInProgress(_screenMenu, _service),
			new TransitionScreen(_screenMenu, _screenGame),
			new TransitionGameInProgress(_screenGame, new StrategyDefault(this)),
		};
	}

	private void Update()
	{
		_service.Update(_repository);
		_indexTransition += _fsm[_indexTransition].Execute(_repository) ? 1 : 0;
		_indexTransition %= _fsm.Length;
	}

	private void OnDestroy()
	{
		_service.Stop();
		_repository.Save();

		for(var index = 0; index < _fsm.Length; index++)
		{
			_fsm[index].Release();
		}
	}

	// IContext
	public T Resolve<T>() where T : class
	{
		return
			_repository as T ??
			_service as T ??
			(typeof(T) == typeof(IReflectorDriverRemote) ? _remote as T : null) ??
			(typeof(T) == typeof(IReflectorDriverLocal) ? _local as T : null);
	}
}
