using Model.Data;

namespace Model.Rules
{
	public interface IStrategyGame
	{
		StatusMatch Status { get; }
		void UpdateModel(Repository repository);
	}
}
