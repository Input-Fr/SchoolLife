namespace Tasks.Task_2
{
	public class CodeTask : TaskManager
	{
		public void CorrectCode()
		{
			isTaskDone = true;
			task.gameObject.SetActive(false);
		}
	}
}
