using Model;

namespace App
{
	public interface ITransition
	{
		bool Execute(Repository repository);

		void Release();
	}
}
