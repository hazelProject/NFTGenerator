using System;
using System.Collections.Generic;
using System.Text;

namespace HazelProject.NFTGenerator
{
    public class Trait
    {
        #nullable enable
        public int Id { get; set; }
        public string Name { get; set; }
        public string Directory { get; set; }
        public int? PresenceChance { get; set; }
        public int[]? RarityWeights { get; set; }
        public int NumOfElements { get; set; }

        public Trait() { }

        public Trait(int id, string name, string directory, int? presenceChance = null, int[]? rarityWeights = null)
        {
            Id = id;
            Name = name;
            Directory = directory;
            PresenceChance = presenceChance;
            if (rarityWeights != null)
            {
                RarityWeights = new int[rarityWeights.Length];
                Array.Copy(rarityWeights, RarityWeights, rarityWeights.Length);
            }
        }
    }

}
