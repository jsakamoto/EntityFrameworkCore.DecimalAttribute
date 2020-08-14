using System;
using System.ComponentModel;

namespace Toolbelt.ComponentModel.DataAnnotations.Schema
{
    /// <summary>
    /// [deprecated] Please use [Decimal] attribute with "using Toolbelt.ComponentModel.DataAnnotations.Schema.Alias;".
    /// </summary>
    [Obsolete("Please use [Decimal] attribute with \"using Toolbelt.ComponentModel.DataAnnotations.Schema.Alias;\".")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DecimalAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets a number of the precision of the decimal column type.
        /// </summary>
        public int Precision { get; }

        /// <summary>
        /// Gets or sets a number of the scale of the decimal column type.
        /// </summary>
        public int Scale { get; }

        /// <summary>
        /// Initializes a new DecimalAttribute instancefor annotate this property to be decimal column type.
        /// </summary>
        public DecimalAttribute() : this(18, 2)
        {
        }

        /// <summary>
        /// Initializes a new DecimalAttribute instance for annotate this property to be decimal column type with the given precision and scale.
        /// </summary>
        /// <param name="precision">A number of the precision of the decimal type column.</param>
        /// <param name="scale">A number of the precision of the decimal type column.</param>
        public DecimalAttribute(int precision, int scale)
        {
            Precision = precision;
            Scale = scale;
        }
    }
}
