using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority.RDS
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RequiredItemAttribute : ValidationAttribute
    {
        protected readonly List<ValidationResult> validationResults = new List<ValidationResult>();

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var list = value as IEnumerable;
            if (list == null)
                return new ValidationResult($"数组属性 {validationContext.DisplayName} 不能为空");

            var isValid = true;
            int count = 0;

            foreach (var item in list)
            {
                ++count;
                //var isItemValid = Validator.TryValidateObject(item, validationContext, validationResults, true);
                //isValid &= isItemValid;
            }

            if (count == 0)
                return new ValidationResult($"数组属性 {validationContext.DisplayName} 不能为空");
            return ValidationResult.Success;
        }
    }
}
