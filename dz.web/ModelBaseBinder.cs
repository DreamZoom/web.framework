using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace dz.web
{
    public class ModelBaseBinder : DefaultModelBinder
    {

        
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (!controllerContext.Controller.ViewData.ContainsKey("BindModelType")) return null;

            string modeltype = controllerContext.Controller.ViewData["BindModelType"].ToString();

            model.ModelBase model = null;

            foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {

                if (assembly.GetName().Name == "Anonymously Hosted DynamicMethods Assembly") continue;

                Type serviceType = assembly.GetTypes().FirstOrDefault(m => m.Name == (modeltype));
                if (serviceType != null)
                {
                    model= assembly.CreateInstance(serviceType.FullName) as model.ModelBase; 
                }

            }
            if(model==null) return null;

            bindingContext.ModelMetadata = new ModelMetadata(new DataAnnotationsModelMetadataProvider(),null,null,model.GetType(),bindingContext.ModelMetadata.PropertyName); 
                     
            return base.BindModel(controllerContext, bindingContext);
        }
    }
}
