using System;
using System.Collections.Generic;
using System.Text;

namespace HazelProject.NFTGenerator
{
    public class MetadataRecord
    {
         public int Id { get; set; }
        public Dictionary<string, string> Data {get;set;}
        public MetadataRecord() { }

        public MetadataRecord(int id, Dictionary<string, string> data)
        {
            Id = id;
            Data = data;
        }
    }
}
