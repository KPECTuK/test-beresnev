namespace Service.Input
{
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
}
