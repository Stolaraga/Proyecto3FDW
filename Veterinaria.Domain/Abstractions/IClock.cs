using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veterinaria.Domain.Abstractions
{
    public interface IClock
    {
        DateOnly Today { get; }
        DateTime Now { get; }
    }

}
