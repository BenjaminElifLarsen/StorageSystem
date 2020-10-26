using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystemCore
{
    /// <summary>
    /// Publisher class that all objects, that needs to react either create a ware or interact with a ware, should be subscribed too.
    /// </summary>
    public class Publisher
    {
        private static WarePublisher warePublisher = new WarePublisher();

        /// <summary>
        /// The default constructor.
        /// </summary>
        public Publisher()
        {
        }

        /// <summary>
        /// Gets the ware publisher class instant. 
        /// </summary>
        public static WarePublisher PubWare { get => warePublisher; }

    }
}
