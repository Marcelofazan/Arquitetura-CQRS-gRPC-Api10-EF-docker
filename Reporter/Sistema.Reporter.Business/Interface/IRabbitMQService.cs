using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema.Reporter.Business.Interface
{
    public interface IRabbitMQService
    {
        Task StartListening(string queueName, Action<string> messageProcessor);
    }
}
