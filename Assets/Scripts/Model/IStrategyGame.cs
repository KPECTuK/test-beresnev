namespace Model
{
	public interface IStrategyGame
	{
		void UpdateModel(Repository repository);

		bool CheckComplete(Repository repository);
	}
}
