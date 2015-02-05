using System;
using System.Linq;
using System.Text.RegularExpressions;
using QuoteFlow.Api;
using QuoteFlow.Api.Jql.Function;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Jql.Function
{
    /// <summary>
    /// Function that produces the end of the month as the value.
    /// </summary>
    public abstract class AbstractDateFunction : AbstractJqlFunction
    {
        private const int MinExpectedArgs = 0;
        private const int MaxExpectedArgs = 1;
        private static readonly Regex DurationPattern = new Regex("([-\\+]?)(\\d+)([mhdwMy]?)");

        protected internal enum UNIT
        {
            YEAR,
            MONTH,
            WEEK,
            DAY,
            HOUR,
            MINUTE
        }

        internal AbstractDateFunction(TimeZoneManager timeZoneManager)
            : this(RealClock.Instance, timeZoneManager)
        {
        }

        internal AbstractDateFunction(Clock clock, TimeZoneManager timeZoneManager)
        {
            this.clock = clock;
            this.timeZoneManager = timeZoneManager;
        }

        public IMessageSet Validate(User searcher, FunctionOperand operand, ITerminalClause terminalClause)
        {
            IMessageSet messageSet = (new NumberOfArgumentsValidator(MinExpectedArgs, MaxExpectedArgs)).Validate(operand);

            if (operand.Args.Count == 1)
            {
                string duration = operand.Args.ElementAt(0);
                if (!DurationPattern.Matcher(duration).matches())
                {
                    messageSet.AddErrorMessage(string.Format("jira.jql.date.function.duration.incorrect: {0}", operand.Name));
                }
            }
            return messageSet;
        }

        protected internal int GetDurationAmount(string duration)
        {
            Matcher matcher = DurationPattern.matcher(duration);
            try
            {
                if (matcher.matches())
                {
                    if (matcher.groupCount() > 1)
                    {
                        if (matcher.group(1).Equals("+"))
                        {
                            return Convert.ToInt32(matcher.group(2));
                        }
                        if (matcher.group(1).Equals("-"))
                        {
                            return -Convert.ToInt32(matcher.group(2));
                        }
                    }
                }
                return Convert.ToInt32(matcher.group(2));
            }
            catch (NumberFormatException e)
            {
                // This should never happen as we have already formatted.
                // But can when JQL calls getValues even after a validation failure
                return 0;
            }
        }

        protected internal int GetDurationUnit(string duration)
        {
            Matcher matcher = DurationPattern.matcher(duration);
            if (matcher.matches())
            {
                if (matcher.groupCount() > 2)
                {
                    string unitGroup = matcher.group(3);
                    if (unitGroup.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return DateTime.YEAR;
                    }
                    if (unitGroup.Equals("M"))
                    {
                        return DateTime.MONTH;
                    }
                    if (unitGroup.Equals("w", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return DateTime.WEEK_OF_MONTH;
                    }
                    if (unitGroup.Equals("d", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return DateTime.DAY_OF_MONTH;
                    }
                    if (unitGroup.Equals("h", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return DateTime.HOUR_OF_DAY;
                    }
                    if (unitGroup.Equals("m"))
                    {
                        return DateTime.MINUTE;
                    }
                }
            }

            return -1;
        }

        public int MinimumNumberOfExpectedArguments
        {
            get { return 0; }
        }

        public IQuoteFlowDataType DataType
        {
            get { return QuoteFlowDataTypes.Date; }
        }
    }
}
