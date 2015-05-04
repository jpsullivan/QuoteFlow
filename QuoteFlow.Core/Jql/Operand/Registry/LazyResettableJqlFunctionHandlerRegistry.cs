using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Query.Operand.Registry;

namespace QuoteFlow.Core.Jql.Operand.Registry
{
    /// <summary>
    /// Keeps a cache of function names against the operand handlers for those functions.
    /// </summary>
    public class LazyResettableJqlFunctionHandlerRegistry : IJqlFunctionHandlerRegistry
    {
//        private readonly PluginAccessor pluginAccesor;
//        private readonly ModuleDescriptors.Orderings moduleDescriptorOrderings;
//        private readonly I18nHelper i18nHelper;
//
//        private readonly ResettableLazyReference<IDictionary<string, FunctionOperandHandler>> registryRef = new ResettableLazyReferenceAnonymousInnerClassHelper();
//
//        private class ResettableLazyReferenceAnonymousInnerClassHelper : ResettableLazyReference<IDictionary<string, FunctionOperandHandler>>
//        {
//            public ResettableLazyReferenceAnonymousInnerClassHelper()
//            {
//            }
//
//            protected internal override IDictionary<string, FunctionOperandHandler> Create()
//            {
//                return outerInstance.loadFromJqlFunctionModuleDescriptors();
//            }
//        }
//
//        internal LazyResettableJqlFunctionHandlerRegistry(PluginAccessor pluginAccesor, ModuleDescriptors.Orderings moduleDescriptorOrderings, I18nHelper i18nHelper)
//        {
//            this.pluginAccesor = pluginAccesor;
//            this.moduleDescriptorOrderings = moduleDescriptorOrderings;
//            this.i18nHelper = i18nHelper;
//        }
//
//        internal virtual IDictionary<string, FunctionOperandHandler> LoadFromJqlFunctionModuleDescriptors()
//        {
//            IList<JqlFunctionModuleDescriptor> jqlFunctionModuleDescriptors = pluginAccesor.getEnabledModuleDescriptorsByClass(typeof(JqlFunctionModuleDescriptor));
//
//            IList<JqlFunctionModuleDescriptor> sortedJqlModuleDescriptors = moduleDescriptorOrderings.byOrigin().compound(moduleDescriptorOrderings.natural()).sortedCopy(jqlFunctionModuleDescriptors);
//
//            var registry = new ConcurrentDictionary<string, FunctionOperandHandler>();
//            foreach (JqlFunctionModuleDescriptor descriptor in sortedJqlModuleDescriptors)
//            {
//                var jqlFunction = descriptor.Module;
//
//                Option<string> functionNameFromPlugin = SafePluginPointAccess.call(new CallableAnonymousInnerClassHelper(this, jqlFunction));
//                if (functionNameFromPlugin.Empty) // error in plugin code, pretend the jqlFunction was not found
//                {
//                    continue;
//                }
//                string functionName = CaseFolding.foldString(functionNameFromPlugin.get());
//
//                var operandHandler = new FunctionOperandHandler(jqlFunction, i18nHelper);
//
//                var handler = registry.PutIfAbsent(functionName, operandHandler);
//                if (handler != null) // The function has already been registered.
//                {
//                    log.error(string.Format("Plugin '{0}' defined a function with the name: '{1}' but one with that" + " name already exists.", descriptor.Plugin.Name, functionName));
//                }
//            }
//            return registry;
//        }
//
//        /// <summary>
//        /// Resets the cache. It will be populated again when
//        /// <see cref="#getOperandHandler(com.atlassian.query.operand.FunctionOperand)"/> or <see cref="#getAllFunctionNames()"/>
//        /// gets called.
//        /// </summary>
//        internal virtual void Reset()
//        {
//            registryRef.Reset();
//        }
//
//        public virtual FunctionOperandHandler getOperandHandler(FunctionOperand operand)
//        {
//            IDictionary<string, FunctionOperandHandler> registry = registryRef.get();
//            FunctionOperandHandler handler = registry[CaseFolding.foldString(operand.Name)];
//            if (handler == null)
//            {
////                if (log.DebugEnabled)
////                {
////                    log.debug("Unable to find handler for function '" + operand.Name + "'.");
////                }
//            }
//            return handler;
//        }
//
//        public virtual IEnumerable<string> AllFunctionNames
//        {
//            get
//            {
//                ICollection<FunctionOperandHandler> functionOperandHandlers = registryRef.get().values();
//                IList<string> functionNames = new List<string>(functionOperandHandlers.Count);
//                foreach (FunctionOperandHandler functionOperandHandler in functionOperandHandlers)
//                {
//                    CollectionUtils.addIgnoreNull(functionNames, SafePluginPointAccess.call(new CallableAnonymousInnerClassHelper2(this)).OrNull);
//                }
//                functionNames.Sort();
//                return functionNames;
//            }
//        }

        public FunctionOperandHandler GetOperandHandler(FunctionOperand operand)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> AllFunctionNames
        {
            get { throw new NotImplementedException(); }
        }
    }

}