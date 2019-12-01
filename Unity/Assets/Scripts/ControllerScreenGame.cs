using System;
using Model.Data;
using Model.Rules;
using UnityEngine;
using UnityEngine.UI;

public class ControllerScreenGame : ControllerScreen
{
	private RectTransform _reflectorRemote;
	private RectTransform _reflectorLocal;
	private RectTransform _ball;
	private RectTransform _court;

	private Text _debugRemote;
	private Text _debugLocal;
	private Text _debugBall;

	private new void Awake()
	{
		base.Awake();

		_ball = transform.Find("ball")?.GetComponent<RectTransform>() ?? throw new Exception("can't find screen BALL");
		_court = transform.Find("court")?.GetComponent<RectTransform>() ?? throw new Exception("can't find screen COURT");
		_reflectorRemote = transform.Find("reflector-remote")?.GetComponent<RectTransform>() ?? throw new Exception("can't find screen REFLECTOR REMOTE");
		_reflectorLocal = transform.Find("reflector-local")?.GetComponent<RectTransform>() ?? throw new Exception("can't find screen REFLECTOR LOCAL");

		_debugRemote = _reflectorRemote.Find("text")?.GetComponent<Text>();
		_debugLocal = _reflectorLocal.Find("text")?.GetComponent<Text>();
		_debugBall = _ball.Find("text")?.GetComponent<Text>();

		_debugRemote?.gameObject.SetActive(false);
		_debugLocal?.gameObject.SetActive(false);
		_debugBall?.gameObject.SetActive(false);
	}

	public void Render(Repository repository)
	{
		// aspect
		var aspect = repository.DataCourt.TotalWidth() / repository.DataCourt.TotalHeight();
		var elementSizes = RectTransform.rect.size;
		float height;
		float width;
		if(elementSizes.x < elementSizes.y)
		{
			height = elementSizes.x * aspect;
			width = elementSizes.x;
		}
		else
		{
			height = elementSizes.y;
			width = elementSizes.y * aspect;
		}

		// court
		_court.sizeDelta = new Vector2(width, height);

		// reflector remote: pivot is at mid pos
		_reflectorRemote.sizeDelta = new Vector2(
			repository.DataReflectorRemote.HalfSize * width,
			_reflectorRemote.sizeDelta.y); //! size depends of reflector shape 
		_reflectorRemote.anchoredPosition = new Vector2(
			width * repository.DataReflectorRemote.Position.x * .5f,
			height * repository.DataReflectorRemote.Position.y * .5f); 

		// reflector local:  pivot is at mid pos
		_reflectorLocal.sizeDelta = new Vector2(
			repository.DataReflectorLocal.HalfSize * width, 
			_reflectorLocal.sizeDelta.y);
		_reflectorLocal.anchoredPosition = new Vector2(
			width * repository.DataReflectorLocal.Position.x * .5f,
			height * repository.DataReflectorLocal.Position.y * .5f);

		// ball
		_ball.sizeDelta = aspect * repository.DataBall.Radius * new Vector2(width, height);
		_ball.anchoredPosition = .5f *
			aspect *
			new Vector2(
				width * repository.DataBall.Position.x,
				height * repository.DataBall.Position.y);
		_ball.SearchComponent<RawImage>(null).color = repository.DataBall.Color;

		//_debugRemote.text = repository.DataReflectorRemote.Position.ToString("0.000");
		//_debugLocal.text = repository.DataReflectorLocal.Position.ToString("0.000");
		//_debugBall.text = repository.DataBall.Position.ToString("0.000");
	}
}
