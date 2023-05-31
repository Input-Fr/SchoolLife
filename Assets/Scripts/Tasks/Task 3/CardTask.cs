namespace Tasks.Task_3
{
	public class CardTask : TaskManager
	{
		public void ValidAccess()
		{
			isTaskDone = true;
			task.gameObject.SetActive(false);

			Rewards();
		}
	}
}
