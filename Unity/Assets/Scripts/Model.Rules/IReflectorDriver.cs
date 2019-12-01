using Model.Data;

namespace Model.Rules
{
	public interface IReflectorDriver
	{
		void UpdateReflector(Repository repository, ModelReflector reflector);
	}
}
