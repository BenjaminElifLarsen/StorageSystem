using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystemCore
{
    /// <summary>
    /// Contains the type of the ware. 
    /// </summary>
    public class WareTypeAttribute : Attribute
    {
        private string type;

        /// <summary>
        /// The basic constructor
        /// </summary>
        /// <param name="type"></param>
        public WareTypeAttribute(string type)
        {
            this.type = type;
        }

        /// <summary>
        /// Gets the type of the ware. 
        /// </summary>
        public string Type { get => type; }

    }
}
