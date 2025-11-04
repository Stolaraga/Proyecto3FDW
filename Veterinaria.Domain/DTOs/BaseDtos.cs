using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veterinaria.Domain.DTOs
{


    public abstract record BaseCreateDto;


    public abstract record BaseUpdateDto
    {
        public Guid Id { get; init; }
    }


}
