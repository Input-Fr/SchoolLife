namespace Tasks.Task_4
{
    public class PuzzleTask : TaskManager
    {
        public void ValidPuzzle()
        {
            isTaskDone = true;
            task.gameObject.SetActive(false);

            Rewards();
        }
    }
}
