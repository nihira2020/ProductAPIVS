using System;
using System.Collections.Generic;

namespace ProductAPIVS.Models
{
    public partial class TblUser
    {
        public string Userid { get; set; } = null!;
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public int? JobId { get; set; }
    }
}
