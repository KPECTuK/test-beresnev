namespace Model
{
	public class StrategyDefault : IStrategyGame
	{
		public void UpdateModel(Repository repository) { }

		public bool CheckComplete(Repository repository)
		{
			return true;
		}
	}
}
