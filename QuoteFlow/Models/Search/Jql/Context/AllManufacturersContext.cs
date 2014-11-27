﻿namespace QuoteFlow.Models.Search.Jql.Context
{
    public class AllManufacturersContext : IManufacturerContext
    {
        private static readonly AllManufacturersContext INSTANCE = new AllManufacturersContext();

		public static AllManufacturersContext Instance
		{
			get { return INSTANCE; }
		}

        private AllManufacturersContext()
		{
			//Don't create me.
		}

        public int ManufacturerId { get { return 0; } }
        
        public bool All { get { return true; } }

        public override string ToString()
        {
            return "All Manufacturers Context";
        }
    }
}