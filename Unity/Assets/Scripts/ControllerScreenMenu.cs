using System;
using App;
using Model.Data;
using UnityEngine;
using UnityEngine.UI;

public class ControllerScreenMenu : ControllerScreen
{
	private Button[] _buttonColor;
	private RectTransform _groupConnections;
	private RectTransform _groupScores;

	private new void Awake()
	{
		base.Awake();

		_groupScores = transform.SearchComponent<RectTransform>(_ => _.name == "group-scores") ?? throw new Exception("can't find screen GROUP SCORE");
		_groupConnections = transform.SearchComponent<RectTransform>(_ => _.name == "group-connections") ?? throw new Exception("can't find screen GROUP CONNECTIONS");
		var groupColor = transform.SearchComponent<RectTransform>(_ => _.name == "group-color") ?? throw new Exception("can't find screen GROUP COLOR");
		
		_buttonColor = new Button[groupColor.childCount];
		for(var index = 0; index < groupColor.childCount; index++)
		{
			// might gaps
			_buttonColor[index] = groupColor.GetChild(index).GetComponent<Button>();
			if(_buttonColor[index])
			{
				var buttonIndex = index;
				_buttonColor[index].onClick.AddListener(() => OnButtonColor(buttonIndex));
			}
		}
	}

	public void Render(Repository repository)
	{
		SyncConnections(repository);
		SyncScores(repository);
	}

	private void SyncConnections(Repository repository)
	{
		var countData = repository.DataNetwork.DataConnections.Length;
		var countView = _groupConnections.childCount;
		var max = countData > countView ? countData : countView;

		for(var index = 0; index < max; index++)
		{
			if(index >= countData)
			{
				_groupScores.GetChild(index).GetComponent<RectTransform>().sizeDelta = Vector2.zero;
				continue;
			}

			var @object = index >= countView 
				? Instantiate(_groupConnections.GetChild(0).gameObject, _groupConnections, false) 
				: _groupConnections.GetChild(index).gameObject;

			var buttonIndex = index;
			@object.transform.SearchComponent<Button>(null).onClick.AddListener(() => repository.DataNetwork.Selection = buttonIndex);

			var record = repository.DataNetwork.DataConnections[index];
			@object.transform.SearchComponent<Text>(_ => _.name == "text").text = $"{record.DataNameRemote} .. {record.DataIpEndPoint}";
		}
	}

	private void SyncScores(Repository repository)
	{
		var countData = repository.DataMetaLocal.Scores.Length;
		var countView = _groupScores.childCount;
		var max = countData > countView ? countData : countView;

		for(var index = 0; index < max; index++)
		{
			if(index >= countData)
			{
				_groupScores.GetChild(index).GetComponent<RectTransform>().sizeDelta = Vector2.zero;
				continue;
			}

			var @object = index >= countView 
				? Instantiate(_groupScores.GetChild(0).gameObject, _groupScores, false) 
				: _groupScores.GetChild(index).gameObject;

			var record = repository.DataMetaLocal.Scores[index];
			@object.GetComponent<Text>().text = $"{record.Name} .. {record.Value}";
		}
	}

	private void OnButtonColor(int index)
	{
		//! 'this' is in closure
		var component = _buttonColor[index].GetComponent<RawImage>();
		// ReSharper disable once MergeConditionalExpression - Unity
		var color = ReferenceEquals(null, component) ? Color.black : component.color;
		_buttonColor[index].GetComponentInParent<IContext>().Resolve<Repository>().DataMetaLocal.BallColor = color;
		_buttonColor[index].Select();

		//Debug.Log($"select color: {color}");
	}

	private void OnDestroy()
	{
		foreach(Transform child in _groupConnections.transform)
		{
			var button = child.GetComponent<Button>();
			// ReSharper disable once UseNullPropagation
			if(ReferenceEquals(null, button))
			{
				continue;
			}
			button.onClick.RemoveAllListeners();
		}

		Array.ForEach(_buttonColor, _ => _.onClick.RemoveAllListeners());
	}
}