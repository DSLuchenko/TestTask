namespace Client.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

        public enum Statuses
        {
            New,
            Active,
            Blocked,
            Deleted
        }
    }
}
