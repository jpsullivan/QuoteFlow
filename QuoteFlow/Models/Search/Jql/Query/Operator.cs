using System.ComponentModel.DataAnnotations;

namespace QuoteFlow.Models.Search.Jql.Query
{
    public enum Operator
    {
        [Display(Name = "~")]
        LIKE,
        [Display(Name = "!~")]
        NOT_LIKE,
        [Display(Name = "=")]
        EQUALS,
        [Display(Name = "!=")]
        NOT_EQUALS,
        [Display(Name = "in")]
        IN,
        [Display(Name = "not in")]
        NOT_IN,
        [Display(Name = "is")]
        IS,
        [Display(Name = "is not")]
        IS_NOT,
        [Display(Name = "<")]
        LESS_THAN,
        [Display(Name = "<=")]
        LESS_THAN_EQUALS,
        [Display(Name = ">")]
        GREATER_THAN,
        [Display(Name = ">=")]
        GREATER_THAN_EQUALS,
        [Display(Name = "was")]
        WAS,
        [Display(Name = "was not")]
        WAS_NOT,
        [Display(Name = "was in")]
        WAS_IN,
        [Display(Name = "was not in")]
        WAS_NOT_IN,
        [Display(Name = "changed")]
        CHANGED,
        [Display(Name = "not changed")]
        NOT_CHANGED,
        [Display(Name = "before")]
        BEFORE,
        [Display(Name = "after")]
        AFTER,
        [Display(Name = "from")]
        FROM,
        [Display(Name = "to")]
        TO,
        [Display(Name = "on")]
        ON,
        [Display(Name = "during")]
        DURING,
        [Display(Name = "by")]
        BY
    }
}