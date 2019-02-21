using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toolbelt.ComponentModel.DataAnnotations.Schema;
using Toolbelt.EntityFrameworkCore.Metadata.Builders;

namespace Toolbelt.ComponentModel.DataAnnotations
{
    public static class AttributedDecimalBuilderExtension
    {
        public static void BuildDecimalColumnTypeFromAnnotations(this ModelBuilder modelBuilder)
        {
            AnnotationBasedModelBuilder.Build<DecimalAttribute>(modelBuilder, Build);
        }

        private static void Build(EntityTypeBuilder builder1, ReferenceOwnershipBuilder builder2, AnnotatedProperty<DecimalAttribute> builderArg)
        {
            var props = builder1?.Metadata.GetProperties() ?? builder2.Metadata.Properties;
            props.First(p => p.Name == builderArg.Name)
                .Relational()
                .ColumnType = $"decimal({builderArg.Attribute.Precision}, {builderArg.Attribute.Scale})";
        }
    }
}
