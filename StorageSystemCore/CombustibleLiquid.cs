using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystemCore
{
    /// <summary>
    /// The combustible liquid ware class.
    /// </summary>
    [WareType("Combustible Liquid")]
    class CombustibleLiquid : Liquid
    {
        /// <summary>
        /// The flammable category of the liquid. Goes from 1 (dangerous) to 4 (safest).
        /// </summary>
        protected byte? category;
        /// <summary>
        /// The flashpount of the liquid.
        /// </summary>
        protected float[] flashPoint;

        /// <summary>
        /// Most basic constructor, derived from Ware. Category, boilingPoint, information, and flashPoint need to be given after.
        /// </summary>
        /// <param name="name">The name of the ware.</param>
        /// <param name="id">The ID of the ware.</param>
        /// <param name="amount">The amount of the ware.</param>
        /// <param name="warePublisher"The event publisher class.></param>
        public CombustibleLiquid(string name, string id, int amount, WarePublisher warePublisher) : base(name, id, amount, warePublisher)
        {
            category = null;
            boilingPoint = null;
            flashPoint = null;
        }

        /// <summary>
        /// Sets basic ware variables and ware inforamtion.
        /// </summary>
        /// <param name="name">The name of the ware.</param>
        /// <param name="id">The ID of the ware.</param>
        /// <param name="amount">The amount of the ware.</param>
        /// <param name="information">Information about the ware.</param>
        /// <param name="warePublisher">The event publisher class.</param>
        public CombustibleLiquid(string name, string id, int amount, string information, WarePublisher warePublisher) : base(name, id, amount, information, warePublisher)
        {
            category = null;
            boilingPoint = null;
            flashPoint = null;
        }

        /// <summary>
        /// Basic liquid constructor, derived from Liquid. Category and flashPoints need to be given after. 
        /// </summary>
        /// <param name="name">The name of the ware.</param>
        /// <param name="id">The ID of the ware.</param>
        /// <param name="amount">The amount of the ware.</param>
        /// <param name="minTemp">The minimum temperature of the ware.</param>
        /// <param name="boilingPoint">The maximum temperature of the ware.</param>
        /// <param name="warePublisher">The event publisher class.</param>
        public CombustibleLiquid(string name, string id, int amount, double minTemp, float boilingPoint, WarePublisher warePublisher) : base(name, id, amount, minTemp, boilingPoint, warePublisher)
        {
            category = null;
            flashPoint = null;
        }

        /// <summary>
        /// Sets all variables that are inheriented from Liquid
        /// </summary>
        /// <param name="name">The name of the ware.</param>
        /// <param name="id">The ID of the ware.</param>
        /// <param name="amount">The amount of the ware.</param>
        /// <param name="information">Information about the ware.</param>
        /// <param name="minTemp">The minimum temperature of the ware.</param>
        /// <param name="boilingPoint">The maximum temperature of the ware.</param>
        /// <param name="warePublisher">The event publisher class.</param>
        public CombustibleLiquid(string name, string id, int amount, string information, double minTemp, float boilingPoint, WarePublisher warePublisher) : base(name, id, amount, information, minTemp, boilingPoint, warePublisher)
        {
            category = null;
            flashPoint = null;
        }

        /// <summary>
        /// Constructor for a combustible liquid. Information of the ware is not set.
        /// </summary>
        /// <param name="name">The name of the ware.</param>
        /// <param name="id">The ID of the ware.</param>
        /// <param name="amount">The amount of the ware.</param>
        /// <param name="minTemp">The minimum temperature of the ware.</param>
        /// <param name="boilingPoint">The maximum temperature of the ware.</param>
        /// <param name="category">The danger category of the ware.</param>
        /// <param name="flashPoint">The flashpoint of the ware.</param>
        /// <param name="warePublisher">The event publisher class.</param>
        public CombustibleLiquid(string name, string id, int amount, double minTemp, byte category, float boilingPoint, float[] flashPoint, WarePublisher warePublisher) : base(name, id, amount, minTemp, boilingPoint, warePublisher)
        {
            this.category = category;
            this.flashPoint = flashPoint;
        }

        /// <summary>
        /// Constructor for a combustible liquid. All variables are set. 
        /// </summary>
        /// <param name="name">The name of the ware.</param>
        /// <param name="id">The ID of the ware.</param>
        /// <param name="amount">The amount of the ware.</param>
        /// <param name="information">Information about the ware.</param>
        /// <param name="minTemp">The minimum temperature of the ware.</param>
        /// <param name="boilingPoint">The maximum temperature of the ware.</param>
        /// <param name="category">The danger category of the ware.</param>
        /// <param name="flashPoint">The flashpoint of the ware.</param>
        /// <param name="warePublisher">The event publisher class.</param>
        public CombustibleLiquid(string name, string id, int amount, string information, double minTemp, byte category, float boilingPoint, float[] flashPoint, WarePublisher warePublisher) : base(name, id, amount, information, minTemp, boilingPoint, warePublisher)
        {
            this.category = category;
            this.flashPoint = flashPoint;
        }


        /// <summary>
        /// Gets the category of the liquid. Null indicates the category has not been set.
        /// </summary>
        /// <value>Contains the danger category value.</value>
        [WareSeacheable("Category", "dangerCategory")]
        public byte? Category 
        { get => category; 
            set 
            {
                if (value > 4 || value < 0)
                    category = null;
                else
                    category = value;
            }
        }


        /// <summary>
        /// Gets the flash point of the liquid. Null indicates the flashpoint has not been set. 
        /// </summary>
        /// <value>Contains the flash points of the liquid.</value>
        [WareSeacheable("Flash Point", "flashPoint")]
        public float[] FlashPoint { get => flashPoint; set => flashPoint = value; }



    }
}
