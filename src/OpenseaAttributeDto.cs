using System;
using System.Collections.Generic;
using System.Text;

namespace HazelProject.NFTGenerator
{
    public class OpenseaAttributeDto
    {
        public string trait_type { get; set; }
        public string value { get; set; }

        public OpenseaAttributeDto() { }
        public OpenseaAttributeDto(string traitType, string value)
        {
            trait_type = traitType;
            this.value = value;
        }
    }
}
