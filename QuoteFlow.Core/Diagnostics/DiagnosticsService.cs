﻿using System;
using System.Diagnostics;
using System.Globalization;

namespace QuoteFlow.Core.Diagnostics
{
    public class DiagnosticsService : IDiagnosticsService
    {
        public DiagnosticsService()
        {
            Trace.AutoFlush = true;
        }

        public IDiagnosticsSource GetSource(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Strings.ParameterCannotBeNullOrEmpty, "name"), nameof(name));
            }
            return new TraceDiagnosticsSource(name);
        }
    }
}