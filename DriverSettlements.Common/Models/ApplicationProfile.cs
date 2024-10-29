using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Models
{
    public class ApplicationProfile
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool Default { get; set; } = false;
        public bool Active { get; set; } = false;

        public ApplicationProfile() 
        { 
        }

        public ApplicationProfile(string text)
        {
            var tab = text.Split('|');
            Id = tab[0];
            Default = !tab[1].Equals("0");
            Name = tab[2];
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}",Id,Default?"1":"0",Name);
        }
    }
}
