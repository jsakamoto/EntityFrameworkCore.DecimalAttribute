using System;
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
            var property = builder1?.Property(builderArg.Name) ?? builder2.Property(builderArg.Name);
            if (property == null) throw new Exception($"Could not determind property \"{builderArg.Name}\" of \"{builder2.OwnedEntityType.Name}\"");
            property.HasColumnType($"decimal({builderArg.Attribute.Precision}, {builderArg.Attribute.Scale})");
        }
    }
}
