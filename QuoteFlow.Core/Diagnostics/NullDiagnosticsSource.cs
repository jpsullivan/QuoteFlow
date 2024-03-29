﻿using System.Diagnostics.CodeAnalysis;

namespace QuoteFlow.Core.Diagnostics
{
    public class NullDiagnosticsSource : IDiagnosticsSource
    {
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")]
        public static readonly NullDiagnosticsSource Instance = new NullDiagnosticsSource();

        private NullDiagnosticsSource() { }

        public void TraceEvent(System.Diagnostics.TraceEventType type, int id, string message, string member = null, string file = null, int line = 0)
        {
            // No-op!
        }
    }
}
