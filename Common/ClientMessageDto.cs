using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ClientMessageDto
    {
        public long TimeOfReading { get; set; }
        public decimal Reading { get; set; }
        public Guid ReadingId { get; set; }
        public int ThreadId { get; set;}
        public int NumberOfThreads { get; set; }

        public ClientMessageDto(ReadingDto reading, int threadId, int numberOfThreads, decimal newValue)
        {
            TimeOfReading = reading.TimeOfReading;
            ReadingId = reading.ReadingId;
            ThreadId = threadId;
            NumberOfThreads = numberOfThreads;
            Reading = newValue;
        }
    }
}
