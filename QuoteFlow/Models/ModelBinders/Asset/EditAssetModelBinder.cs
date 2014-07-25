using System;
using System.Collections.Generic;
using System.Web.Mvc;
using QuoteFlow.Models.ViewModels.Assets;

namespace QuoteFlow.Models.ModelBinders.Asset
{
    public class EditAssetModelBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            if (modelType == typeof(EditAssetRequest)) // so it will leave the other VM to the default implementation.
            {
                // this gets the value from the form collection, if it was in an input named "ViewModelName":
                var discriminator = bindingContext.ValueProvider.GetValue("AssetVars");
                if (discriminator == null) {
                    throw new InvalidOperationException("AssetVars parameter is not specified.");
                }

                Type instantiationType;
//                if (discriminator.AttemptedValue == "SomethingSomething") {
//                    instantiationType = typeof (ReliefVM);
//                } else {
//                    instantiationType = typeof(List<AssetVar>);
//                }
                instantiationType = typeof(List<AssetVar>);
                    

                var obj = Activator.CreateInstance(instantiationType);
                bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, instantiationType);
                bindingContext.ModelMetadata.Model = obj;
                return obj;
            }
            return base.CreateModel(controllerContext, bindingContext, modelType);
        }
    }
}