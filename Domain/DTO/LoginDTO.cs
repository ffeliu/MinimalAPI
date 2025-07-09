using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimalapi.domain.dto
{
    public record LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}