namespace Service.Input
{
	public class InputProviderLocalForLocalGyro : IInputProvider
	{
		public bool IsInputLeft()
		{
			return ExtensionInput.IsGyroLeft();
		}

		public bool IsInputRight()
		{
			return ExtensionInput.IsGyroRight();
		}
	}
}
