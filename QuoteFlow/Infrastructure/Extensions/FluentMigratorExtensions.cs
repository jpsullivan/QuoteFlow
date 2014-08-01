using FluentMigrator.Builders.Create.Table;

namespace QuoteFlow.Infrastructure.Extensions
{
    internal static class FluentMigratorExtensions
    {
        public static ICreateTableColumnOptionOrWithColumnSyntax AsMaxString(
            this ICreateTableColumnAsTypeSyntax createTableColumnAsTypeSyntax)
        {
            return createTableColumnAsTypeSyntax.AsString(int.MaxValue);
        }
    }
}