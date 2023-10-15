using Employee.EventBus.Dtos;
using MassTransit;

namespace EmployeeManagement.Dlq.Api.Consumers
{
    public class DlqConsumer : IConsumer<EmployeeDto>
    {
        public Task Consume(ConsumeContext<EmployeeDto> context)
        {
            try
            {
                //Handle dlq message
                var dlqMessage = context.Message;
               
                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
