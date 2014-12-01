﻿using System;
using QuoteFlow.Models.Assets;

namespace QuoteFlow.Models
{
    public class Manufacturer : IAssetConstant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int OrganizationId { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreationDate { get; set; }

        public ManufacturerLogo Logo { get; set; }

        string IAssetConstant.Id { get; set; }
    }
}