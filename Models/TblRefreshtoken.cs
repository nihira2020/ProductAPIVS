using System;
using System.Collections.Generic;

namespace ProductAPIVS.Models
{
    public partial class TblRefreshtoken
    {
        public string UserId { get; set; } = null!;
        public string? TokenId { get; set; }
        public string? RefreshToken { get; set; }
        public bool? IsActive { get; set; }
    }
}
