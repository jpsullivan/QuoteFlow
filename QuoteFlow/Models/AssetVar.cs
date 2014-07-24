﻿using System;
using System.Collections.Generic;

namespace QuoteFlow.Models
{
    public class AssetVar
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ValueType { get; set; }

        public int OrganizationId { get; set; }

        public bool Enabled { get; set; }

        public DateTime CreatedUtc { get; set; }

        public int CreatedBy { get; set; }

        /// <summary>
        /// Manually set; Not contained within a standard db result.
        /// </summary>
        public AssetVarValue Value { get; set; }
    }
}