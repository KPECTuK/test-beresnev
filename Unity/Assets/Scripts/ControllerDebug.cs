using Service.Input;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ControllerDebug : MonoBehaviour
{
	public void Update()
	{
		GetComponent<Text>().text = ExtensionInput.GetGravity().ToString();
	}
}
