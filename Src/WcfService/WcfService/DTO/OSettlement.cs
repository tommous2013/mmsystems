using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfService.DTO
{
    [DataContract]
    public class OSettlement : ORoad
    {
        [DataMember]
        public bool Upgraded { get; set; }
    }
}