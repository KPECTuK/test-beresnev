namespace App
{
	// provider - the same
	public interface IContext
	{
		T Resolve<T>() where T : class;
	}
}
