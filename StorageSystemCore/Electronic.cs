using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystemCore
{
    /// <summary>
    /// The electronic ware class.
    /// </summary>
    [WareType("Electronic")]
    sealed class Electronic : Ware
    {
        /// <summary>
        /// The basic electronic consturctor
        /// </summary>
        /// <param name="name">The name of the ware.</param>
        /// <param name="id">The ID of the ware.</param>
        /// <param name="amount">The unit amount of the ware.</param>
        /// <param name="warePublisher">The event publisher class.</param>
        public Electronic(string name, string id, int amount, WarePublisher warePublisher) : base(name, id, amount, warePublisher)
        {
        }

        /// <summary>
        /// Default electronic consturctor with product information added.
        /// </summary>
        /// <param name="name">The name of the ware.</param>
        /// <param name="id">The ID of the ware.</param>
        /// <param name="information">Information about the ware.</param>
        /// <param name="amount">The unit amount of the ware.</param>
        /// <param name="warePublisher">The event publisher class.</param>
        public Electronic(string name, string id, int amount, string information, WarePublisher warePublisher) : base(name, id, amount, information, warePublisher)
        {
        }
    }
}
