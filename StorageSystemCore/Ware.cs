using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystemCore
{
    /// <summary>
    /// The basic ware class, abstract, that all different types of wares should inherence from.
    /// </summary>
    [WareType("None")] //Ware is abstract and cannot be initialisated
    public abstract class Ware //: IConversion
    {
        /// <summary>
        /// The name of the ware.
        /// </summary>
        protected string name;
        /// <summary>
        /// The unique ID of the ware.
        /// </summary>
        protected string id;
        /// <summary>
        /// The amount of the ware. 
        /// </summary>
        protected int amount;  
        /// <summary>
        /// Information about the ware. 
        /// </summary>
        protected string information;

        private Ware() { }

        /// <summary>
        /// Default Ware constructor.
        /// </summary>
        /// <param name="name">The name of the ware.</param>
        /// <param name="id">The ID of the ware.</param>
        /// <param name="amount">The unit amount of the ware.</param>
        /// <param name="warePublisher">The event publisher class.</param>
        public Ware(string name, string id, int amount, WarePublisher warePublisher)
        {
            this.name = name;
            this.id = id;
            this.amount = amount;
            warePublisher.RaiseAddEvent += AddAmountEventHandler;
            warePublisher.RaiseRemoveEvent += RemoveAmountEvnetHandler;
            warePublisher.RaiseGetTypeEvent += GetTypeEventHandler;
            warePublisher.RaiseAlterWareEvent += AlterWareEventHandler;
        }

        //have attributes for constructors, if Type does not contain a way to find them
        /// <summary>
        /// Default Ware consturctor with product information added.
        /// </summary>
        /// <param name="name">The name of the ware.</param>
        /// <param name="id">The ID of the ware.</param>
        /// <param name="information">Information about the ware.</param>
        /// <param name="amount">The unit amount of the ware.</param>
        /// <param name="warePublisher">The event publisher class.</param>
        public Ware(string name, string id, int amount, string information, WarePublisher warePublisher) : this(name, id, amount, warePublisher)
        {
            this.information = information;
        }


        /// <summary>
        /// Gets the name of the ware.
        /// </summary>
        /// <value></value>
        [WareSeacheable("Name", "name")]
        public string Name { get => name; set => name = value; }


        /// <summary>
        /// Gets the amount of the ware.
        /// </summary>
        /// <value></value>
        [WareSeacheable("Amount", "amount")]
        public int Amount { get => amount; set => amount = value; }


        /// <summary>
        /// Gets the ware information
        /// </summary>
        /// <value>Test</value>
        [WareSeacheable("Information", "information")]
        public string Information { get => information; set => information = value; }


        /// <summary>
        /// Gets the ID of the ware.
        /// </summary>
        /// <value></value>
        [WareSeacheable("ID", "id"), ValueUniqueness(true)]
        public string ID { get => id; set => id = value; }

        /// <summary>
        /// Add the <paramref name="amount"/> to the unit amount of the ware.
        /// </summary>
        /// <param name="amount"></param>
        protected virtual void Add(int amount)
        {
            this.amount += amount;
        }

        /// <summary>
        /// Removes the <paramref name="amount"/> from the unit amount of the ware.
        /// </summary>
        /// <param name="amount"></param>
        protected virtual void Remove(int amount)
        {
                this.amount -= amount;
        }

        protected virtual void AddInformation(string info)
        {
            information = info;
        }

        /// <summary>
        /// Adds the amount of wares as specificed in <paramref name="e"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddAmountEventHandler(object sender, ControlEvents.AddEventArgs e)
        {
            if (e.ID == id)
                Add(e.AmountToAdd);
        }

        /// <summary>
        /// Removes the amount of wares as specificed in <paramref name="e"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RemoveAmountEvnetHandler(object sender, ControlEvents.RemoveEventArgs e)
        {
            if (e.ID == id)
                Remove(e.AmountToRemove);
        }

        /// <summary>
        /// Get the type of the instance via an event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GetTypeEventHandler(object sender, ControlEvents.GetTypeEventArgs e)
        {
            e.Add(id, GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="TargetException"></exception>
        /// <exception cref="TargetInvocationException"></exception>
        protected void AlterWareEventHandler(object sender, ControlEvents.AlterValueEventArgs e)
        {
            if(e.ID == id)
            {
                PropertyInfo[] propertyInfos = GetType().GetProperties();
                foreach(PropertyInfo propertyInfo in propertyInfos)
                {
                    foreach(Attribute attribute in propertyInfo.GetCustomAttributes())
                    {
                        if(attribute.GetType() == typeof(WareSeacheableAttribute))
                        {
                            WareSeacheableAttribute seacheableAttribute = attribute as WareSeacheableAttribute;
                            if(seacheableAttribute.Name == e.PropertyName)
                            {
                                if (e.MultieValueArray != null) //sets an array property
                                {
                                    Type type = e.MultieValueArray[0].GetType();
                                    MethodInfo foundMethod = this.GetType().GetMethod("ArrayConversion", BindingFlags.Instance | BindingFlags.NonPublic);
                                    MethodInfo genericVersion = foundMethod.MakeGenericMethod(type);
                                    var array = genericVersion.Invoke(this, new object[]  { e.MultieValueArray });
                                    try
                                    {
                                        propertyInfo.SetValue(this, array);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                    goto done;
                                }
                                else //sets a non-array property
                                { 
                                    foreach (Attribute attributeValueUnique in propertyInfo.GetCustomAttributes())
                                    {
                                        if(attributeValueUnique.GetType() == typeof(ValueUniqueness))
                                        {
                                            ValueUniqueness valueUniqueness = attributeValueUnique as ValueUniqueness;
                                            if (valueUniqueness.IsUnique) //currently, IDs are not allowed to be overwritten
                                                goto done; //if allowed to overwrite, need to check if the ID structure is correct 
                                        }
                                    }
                                    try
                                    {
                                        propertyInfo.SetValue(this, e.SingleValue);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                    goto done;
                                }
                            }
                        }
                    }
                }
            }
            done:;
        }

        /// <summary>
        /// Removes the subscriptions...
        /// </summary>
        /// <param name="warePublisher"></param>
        public void RemoveSubscriptions(WarePublisher warePublisher) //move this parameter into the class scope
        {
            warePublisher.RaiseAddEvent -= AddAmountEventHandler;
            warePublisher.RaiseRemoveEvent -= RemoveAmountEvnetHandler;
            warePublisher.RaiseGetTypeEvent -= GetTypeEventHandler;
            warePublisher.RaiseAlterWareEvent -= AlterWareEventHandler;
        }

        /// <summary>
        /// Converts an object[] <paramref name="arrayToConvert"/> to the datatype <typeparamref name="T"/>.
        /// Given how the method is written it is needed be called via reflection as a generic method
        /// </summary>
        /// <typeparam name="T">The type that should be converted to</typeparam>
        /// <param name="arrayToConvert">The array to convert</param>
        /// <returns>Returns a converted version of <paramref name="arrayToConvert"/>.</returns>
        protected T[] /*IConversion.*/ArrayConversion<T>(object[] arrayToConvert)
        {
            T[] convertedArray = new T[arrayToConvert.Length];
            for (int i = 0; i < convertedArray.Length; i++)
            {
                convertedArray[i] = (T)Convert.ChangeType(arrayToConvert[i], typeof(T));
            }
            return convertedArray;
        }

    }
}
