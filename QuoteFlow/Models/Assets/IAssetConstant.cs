﻿namespace QuoteFlow.Models.Assets
{
    /// <summary>
    /// Abstraction to represent any of the various constants like <seealso cref="Manufacturer"/>.
    /// </summary>
    public interface IAssetConstant
    {
        string Id { get; set; }

        string Name { get; set; }
    }
}