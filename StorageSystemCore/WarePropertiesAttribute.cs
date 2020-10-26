using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystemCore
{
    /// <summary>
    /// Used to set attributes for methods and properties that contains ware informations that are used for the sql database and display.
    /// </summary>
    public class WareSeacheableAttribute : Attribute //[DataType("System.Double")],[DataType("System.Int[])] maybe as a seperate attribute. Will need a value for collection entry count 
    {
        /// <summary>
        /// The name of the attribute.
        /// </summary>
        private string name;
        /// <summary>
        /// The name of the attribute as it is in the sql database.
        /// </summary>
        private string sqlName;

        /// <summary>
        /// The default constructor for WareSeacheableAttribute class 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sqlName"></param>
        public WareSeacheableAttribute(string name, string sqlName)
        {
            this.name = name;
            this.sqlName = sqlName;
        }

        /// <summary>
        /// Gets and sets the name of the attribute.
        /// </summary>
        public string Name { get => name; set => name = value; }

        /// <summary>
        /// Gets and sets the name of the attribute as it is in the SQL database.
        /// </summary>
        public string SQLName { get => sqlName; set => sqlName = value; }
    }

    /// <summary>
    /// Used to set attributes for properties whoes valeus should not be overwritten. 
    /// </summary>
    public class ValueUniqueness : Attribute
    {
        public ValueUniqueness(bool isUnique)
        {
            IsUnique = isUnique;
        }
        /// <summary>
        /// Gets and sets the bool of the attribute.
        /// </summary>
        public bool IsUnique { get; }
    }
}
