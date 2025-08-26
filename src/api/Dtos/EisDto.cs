using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace NCoreWebApp.Dtos
{
    [DataContract]
    public class EisDto
    {
        [DataMember]
        public string ID { get; set; } = string.Empty;
        [DataMember]
        public string Title { get; set; } = string.Empty;
        [DataMember]
        public string ModelImage { get; set; } = string.Empty;
        [DataMember]
        public string Description { get; set; } = string.Empty;
        [DataMember]
        public string Phase { get; set; } = string.Empty;
    }
}

