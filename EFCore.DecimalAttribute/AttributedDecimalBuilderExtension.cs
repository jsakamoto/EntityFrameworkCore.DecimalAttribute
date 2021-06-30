using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Toolbelt.ComponentModel.DataAnnotations.Schema;
using Toolbelt.EntityFrameworkCore.Metadata.Builders;

namespace Toolbelt.ComponentModel.DataAnnotations
{
    public static class AttributedDecimalBuilderExtension
    {
        public static void BuildDecimalColumnTypeFromAnnotations(this ModelBuilder modelBuilder)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            AnnotationBasedModelBuilder.Build<DecimalAttribute>(modelBuilder, Build);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "<Pending>")]
#pragma warning disable CS0618 // Type or member is obsolete
        private static void Build(EntityTypeBuilder builder1, OwnedNavigationBuilder builder2, AnnotatedProperty<DecimalAttribute> builderArg)
#pragma warning restore CS0618 // Type or member is obsolete
        {
            var property = builder1?.Property(builderArg.Name) ?? builder2.Property(builderArg.Name);
            if (property == null) throw new Exception($"Could not determind property \"{builderArg.Name}\" of \"{(builder2.OwnedEntityType as EntityType).Name}\"");
            property.HasColumnType($"decimal({builderArg.Attribute.Precision}, {builderArg.Attribute.Scale})");
        }
    }
}
