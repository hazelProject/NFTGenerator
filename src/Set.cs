using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HazelProject.NFTGenerator
{
    public class Set : IEquatable<Set>
    {
        public int Id;
        public Dictionary<string, int> SelectedTraits { get; set; }

        public Set(Dictionary<string, int> selectedTraits, int id)
        {
            SelectedTraits = selectedTraits;
            Id = id;
        }

        public bool Equals([AllowNull] Set other)
        {
            if (other == null)
                return false;
            if (SelectedTraits.Keys.Count != other.SelectedTraits.Count)
                return false;

            foreach (var selectedTrait in SelectedTraits)
            {
                if (!other.SelectedTraits.ContainsKey(selectedTrait.Key))
                    return false;

                if (selectedTrait.Value != other.SelectedTraits[selectedTrait.Key])
                    return false;
            }

            return true;
        }
    }
}
