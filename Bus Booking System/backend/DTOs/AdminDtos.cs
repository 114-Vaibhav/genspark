namespace backend.DTOs
{
    public class PendingOperatorResponse
    {
        public Guid OperatorId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Approved { get; set; }
        public bool IsActive { get; set; }
    }
}
