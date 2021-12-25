using System;
using System.Collections.Generic;
using System.Text;

namespace HazelProject.NFTGenerator
{
    public class OpenseaMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public List<OpenseaAttributeDto> Attributes { get; set; }
        public OpenseaMetadata() { }

        public OpenseaMetadata(string name, string description, string image, List<OpenseaAttributeDto> attributes)
        {
            Name = name;
            Description = description;
            Image = image;
            Attributes = attributes;
        }

    }
}
