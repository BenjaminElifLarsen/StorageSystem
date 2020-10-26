using System;
using System.Collections.Generic;

namespace StorageSystemCore
{
    /// <summary>
    /// Event class with events related to the control of the program.
    /// </summary>
    public class ControlEvents : EventArgs
    {
        /// <summary>
        /// Class that holds event data of the ware creation system.
        /// </summary>
        public class CreateWareEventArgs
        {
            /// <summary>
            /// Base constructor for the ware creator event data.
            /// </summary>
            public CreateWareEventArgs()
            {
            }
        }

        /// <summary>
        /// Class taht holds event data of the remove ware creator event system.
        /// </summary>
        public class RemoveWareCreatorEventArgs
        {
            /// <summary>
            /// Base constructor for the remove ware creator event data.
            /// </summary>
            public RemoveWareCreatorEventArgs()
            {
            }
        }

        /// <summary>
        /// Class that holds event data of the ware amount adding system.
        /// </summary>
        public class AddEventArgs
        {
            /// <summary>
            /// Base constructor for the add event data.
            /// </summary>
            /// <param name="ID">The ID of the ware.</param>
            /// <param name="amountToAdd">The amount to add.</param>
            public AddEventArgs(string ID, int amountToAdd)
            {
                this.ID = ID;
                AmountToAdd = amountToAdd;
            }
            /// <summary>
            /// Gets and sets the ID of the ware.
            /// </summary>
            public string ID { get; }

            /// <summary>
            /// Gets and sets the amount to add.
            /// </summary>
            public int AmountToAdd { get; }
        }

        /// <summary>
        /// Class that holds event data of the ware amount removal system.
        /// </summary>
        public class RemoveEventArgs
        {
            /// <summary>
            /// Base constructor for the remove event data.
            /// </summary>
            /// <param name="ID">The ID of the ware.</param>
            /// <param name="amountToRemove">The amount to remove.</param>
            public RemoveEventArgs(string ID, int amountToRemove)
            {
                this.ID = ID;
                AmountToRemove = amountToRemove;
            }
            /// <summary>
            /// Get and sets the ID of the ware.
            /// </summary>
            public string ID { get; }

            /// <summary>
            /// Gets and sets the amount to remove. 
            /// </summary>
            public int AmountToRemove { get; set; }
        }

        /// <summary>
        /// Class that holds event data of getting the ½ware type system.
        /// </summary>
        public class GetTypeEventArgs
        {
            private Dictionary<string,Type> _types = new Dictionary<string, Type>();
            /// <summary>
            /// Base constructor for the getting type event data.
            /// </summary>
            /// <param name="ID">The ID of the ware to find the type of.</param>
            public GetTypeEventArgs(string ID)
            {
                this.ID = ID;
            }
            public string ID { get; }

            /// <summary>
            /// Returns the type of the ware with the <paramref name="ID"/>.
            /// </summary>
            /// <param name="ID">The ID to find its type of</param>
            /// <returns>Returns the type of the ware with the <paramref name="ID"/></returns>
            /// <exception cref="KeyNotFoundException"></exception>
            public Type GetType(string ID)
            {
                try { 
                    return _types[ID];
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            /// <summary>
            /// Wares add their type as values using their ID as the key.
            /// </summary>
            /// <param name="ID">The key.</param>
            /// <param name="type">The value.</param>
            public void Add(string ID, Type type)
            {
                _types.Add(ID, type);
            }

        }

        /// <summary>
        /// Class that holds event data of the ware data altering system.
        /// </summary>
        public class AlterValueEventArgs
        {
            /// <summary>
            /// Alters a datatype' old value, belonging to <paramref name="property"/>, to <paramref name="newValue"/> on a ware with the <paramref name="ID"/>.
            /// </summary>
            /// <param name="ID">The ID of the ware.</param>
            /// <param name="newValue">The new value to overwrite the old value.</param>
            /// <param name="property">The WareSeacheableAttribute Name</param>
            public AlterValueEventArgs(string ID, object newValue, string property)
            {
                this.ID = ID;
                SingleValue = newValue;
                PropertyName = property;
            }

            /// <summary>
            /// Alters the datatype array's old value, belonging to <paramref name="property"/>, to <paramref name="newValue"/> on a ware with the <paramref name="ID"/>.
            /// </summary>
            /// <param name="ID">The ID of the ware.</param>
            /// <param name="newValues">The new value to overwrite the old value.</param>
            /// <param name="property">The WareSeacheableAttribute Name</param>
            public AlterValueEventArgs(string ID, object[] newValues, string property)
            {
                this.ID = ID;
                MultieValueArray = newValues;
                PropertyName = property;
            }
            /// <summary>
            /// The ID of the ware.
            /// </summary>
            public string ID { get; }
            /// <summary>
            /// Holds a single value.
            /// </summary>
            /// <value>Can be null or a value that fits into a specific type.</value>
            public object SingleValue { get; }
            /// <summary>
            /// Holds an array or null.
            /// </summary>
            /// <value>Can be null or a value that fits into a specific array type.</value> 
            public object[] MultieValueArray { get; }
            /// <summary>
            /// The WareSeacheableAttribute Name.
            /// </summary>
            public string PropertyName { get; }
        }

        /// <summary>
        /// Class that holds event data of the input control system.
        /// </summary>
        public class KeyEventArgs
        {
            /// <summary>
            /// Base constructor for the consoleKey event data.
            /// </summary>
            /// <param name="key">The ConsoleKeyInfo to be transmitted.</param>
            public KeyEventArgs(ConsoleKey key) //maybe instead of doing this like the rest, do it Input/Output files
            { //so instead of sending the key to all, even those not needing it at the moment, each can contact the Input and get a key
                Key = key;
            }
            /// <summary>
            /// Gets and sets the consoleKeyInfo key. 
            /// </summary>
            public ConsoleKey Key { get; }
        }


    }
}
