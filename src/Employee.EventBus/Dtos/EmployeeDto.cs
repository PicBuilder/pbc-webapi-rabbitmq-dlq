namespace Employee.EventBus.Dtos
{
    public class EmployeeDto : MessageBaseEvent
    {
        public string Name { get; set; } = default!;
    }

    public class MessageBaseEvent
    {
        public MessageBaseEvent()
        {
            MessageId = Guid.NewGuid();
            MessageCreated = DateTime.UtcNow;
        }

        public MessageBaseEvent(Guid id, DateTime created)
        {
            MessageId = id;
            MessageCreated = created;
        }

        public Guid MessageId { get; private set; }

        public DateTime MessageCreated { get; private set; }
    }
}