namespace WebToDoList.Models
{
    public class SubTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public int TaskId { get; set; }

        // Relación con Task
        public Task Task { get; set; }
    }
}
