using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuoteFlow.Models.ViewModels.Assets;

namespace QuoteFlow.Models.ModelBinders.Asset
{
    public class EditAssetModelBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            if (modelType != typeof (EditAssetRequest))
            {
                return base.CreateModel(controllerContext, bindingContext, modelType);
            }

            var assetVarNameKeys = controllerContext.HttpContext.Request.Form.Cast<string>().Where(element => element.Contains("AssetVarName_")).ToList();
            var assetVarValueKeys = controllerContext.HttpContext.Request.Form.Cast<string>().Where(element => element.Contains("AssetVarValue_")).ToList();

            var assetVarData = new Dictionary<int, object>();
            var assetVarValuesData = new Dictionary<int, object>();

            foreach (var key in assetVarNameKeys)
            {
                // this gets the value from the form collection, if it was in an input named "ViewModelName":
                var discriminator = bindingContext.ValueProvider.GetValue(key);
                if (discriminator == null)
                {
                    throw new InvalidOperationException("AssetVars parameter is not specified.");
                }

                int id = Convert.ToInt32(key.Replace("AssetVarName_", String.Empty));
                assetVarData.Add(id, discriminator.AttemptedValue);
            }

            foreach (var key in assetVarValueKeys)
            {
                // this gets the value from the form collection, if it was in an input named "ViewModelName":
                var discriminator = bindingContext.ValueProvider.GetValue(key);
                if (discriminator == null)
                {
                    throw new InvalidOperationException("AssetVarValues parameter is not specified.");
                }

                int id = Convert.ToInt32(key.Replace("AssetVarValue_", String.Empty));
                assetVarValuesData.Add(id, discriminator.AttemptedValue);
            }

            var obj = (EditAssetRequest) Activator.CreateInstance(typeof(EditAssetRequest));
            obj.AssetVarsData = assetVarData;
            obj.AssetVarValuesData = assetVarValuesData;

            bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(EditAssetRequest));
            bindingContext.ModelMetadata.Model = obj;

            return obj;
        }
    }
}