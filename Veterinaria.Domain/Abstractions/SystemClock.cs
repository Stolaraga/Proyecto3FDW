using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veterinaria.Domain.Abstractions
{
    public sealed class SystemClock : IClock
    {
        public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
        public DateTime Now => DateTime.Now;
    }

}
