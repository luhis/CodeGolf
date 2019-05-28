using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CodeGolf.Web
{
    public class StringBinder : IModelBinder
    {
        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue ?? string.Empty;
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value, value);
            bindingContext.Result = ModelBindingResult.Success(value);
            return Task.CompletedTask;
        }
    }
}