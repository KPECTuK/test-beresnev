namespace Service.Input
{
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
}
