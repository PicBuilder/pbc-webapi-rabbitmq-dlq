using Employee.EventBus.Data.Interfaces;
using Employee.EventBus.Dtos;
using MassTransit;

namespace Employee.EventBus.Consumers
{
    public class EmployeeDtoEventConsumer : IConsumer<EmployeeDto>
    {
        private readonly IApplicationDbContext _context;

        public EmployeeDtoEventConsumer(IApplicationDbContext context)
        {
            _context = context;
        }

        public Task Consume(ConsumeContext<EmployeeDto> context)
        {
            try
            {
                throw new Exception("Simulate some error on queue to init dlq flow");

                //IMPORTANT: below would be normal operation - uncomment code below and comment above to see how db changes would get saved

                //Entities.Employee entity = new Entities.Employee()
                //{
                //    Name = context.Message.Name,
                //};

                //_context.Employee.Add(entity);
                //var result = _context.SaveChanges();

                //return Task.CompletedTask;
            }
            catch (Exception)
            {
                //so we can send to mass transit dlq
                throw;
            }            
        }
    }
}
