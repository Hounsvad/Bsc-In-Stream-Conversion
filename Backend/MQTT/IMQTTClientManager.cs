using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bsc_In_Stream_Conversion
{
    public interface IMQTTClientManager
    {
        Task<Guid> Subscribe(string topic, Func<string, Task> messageCallback);

        Task Unsubscribe(Guid subId);

        Task PublishMessageAsync(string topic, string message);

    }
}
