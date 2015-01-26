using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Models.ViewModels.Assets;
using QuoteFlow.Infrastructure.Extensions;

namespace QuoteFlow.Infrastructure.ModelBinders.Asset
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
            var assetVarData = new List<AssetVarEditRequest>();

            foreach (var key in assetVarNameKeys)
            {
                // this gets the value from the form collection, if it was in an input named "ViewModelName":
                var discriminator = bindingContext.ValueProvider.GetValue(key);
                if (discriminator == null)
                {
                    throw new InvalidOperationException("AssetVars parameter is not specified.");
                }

                var model = GetAssetVarValueDiscriminator(key, bindingContext, discriminator);
                assetVarData.Add(model);
            }

            var obj = (EditAssetRequest) Activator.CreateInstance(typeof(EditAssetRequest));
            obj.AssetVarValuesData = assetVarData;

            bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(EditAssetRequest));
            bindingContext.ModelMetadata.Model = obj;

            return obj;
        }

        /// <summary>
        /// Gets the asset var value from the binding context.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="bindingContext"></param>
        /// <param name="discriminator"></param>
        /// <returns></returns>
        private AssetVarEditRequest GetAssetVarValueDiscriminator(string key, ModelBindingContext bindingContext, ValueProviderResult discriminator)
        {
            if (key.IsNullOrEmpty())
            {
                return null;
            }

            var keys = key.Replace("AssetVarName_", String.Empty);
            int assetVarValueId = Convert.ToInt32(keys.Seperate("_"));
            int assetVarId = Convert.ToInt32(discriminator.AttemptedValue);

            var assetVarValueDiscriminator = bindingContext.ValueProvider.GetValue(string.Format("AssetVarValue_{0}", assetVarValueId.ToString()));
            if (assetVarValueDiscriminator == null)
            {
                throw new InvalidOperationException("AssetVarValue parameter is not specified.");
            }

            var assetVarValue = assetVarValueDiscriminator.AttemptedValue;

            return new AssetVarEditRequest(assetVarValueId, assetVarId, assetVarValue);
        }
    }
}