using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ClientMessageDto
    {
        public DateTimeOffset TimeOfReading { get; set; }
        public decimal Reading { get; set; }
        public Guid ReadingId { get; set; }
        public int ThreadId { get; set;}
        public int NumberOfThreads { get; set; }
    }
}
