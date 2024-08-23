namespace WebToDoList.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public int UserId { get; set; }
        public string Priority { get; set; }
        public string Tags { get; set; }

        // Relación uno a muchos
        public ICollection<SubTask>? SubTasks { get; set; }

        // Relación con User
        public User? User { get; set; }
    }
}
