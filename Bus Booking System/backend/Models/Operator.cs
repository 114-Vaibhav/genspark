namespace backend.Models
{
    public class Operator
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public bool Approved { get; set; }
        public bool IsActive { get; set; }

        public string Address { get; set; } = string.Empty;
    }
}
