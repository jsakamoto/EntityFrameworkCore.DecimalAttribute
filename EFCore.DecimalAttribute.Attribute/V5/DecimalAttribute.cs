using System;

namespace Toolbelt.ComponentModel.DataAnnotations.Schema.V5
{
    /// <summary>
    /// Represents an attribute that is placed on a property to indicate that the database column is decimal with specified precision and scale.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
#pragma warning disable CS0618 // Type or member is obsolete
    public class DecimalAttribute : Toolbelt.ComponentModel.DataAnnotations.Schema.DecimalAttribute
#pragma warning restore CS0618 // Type or member is obsolete
    {
        /// <summary>
        /// Initializes a new DecimalAttribute instancefor annotate this property to be decimal column type.
        /// </summary>
        public DecimalAttribute() : base(18, 2)
        {
        }

        /// <summary>
        /// Initializes a new DecimalAttribute instance for annotate this property to be decimal column type with the given precision and scale.
        /// </summary>
        /// <param name="precision">A number of the precision of the decimal type column.</param>
        /// <param name="scale">A number of the precision of the decimal type column.</param>
        public DecimalAttribute(int precision, int scale) : base(precision, scale)
        {
        }
    }
}
