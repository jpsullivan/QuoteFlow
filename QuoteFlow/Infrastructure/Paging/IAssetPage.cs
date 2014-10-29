﻿namespace QuoteFlow.Infrastructure.Paging
{
    public interface IAssetPage
    {
        int PageNumber { get; }
        int Start { get; }
        bool IsCurrentPage();
    }
}