using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.SharedKernel.DTO.JWT
{
    public static class JwtAuthOption
    {
        public const string Key = "b21567604f334653b5754470d73a0396";
        public const string Issuer  = "www.gold.ir";
        public const string Audience  = "www.gold.ir";
        public const bool ValidateIssuer  = true;
        public const bool ValidateAudience  = true;
        public const bool ValidateLifetime  = true;
        public const bool ValidateIssuerSigningKey  = true;
     
    }
}
