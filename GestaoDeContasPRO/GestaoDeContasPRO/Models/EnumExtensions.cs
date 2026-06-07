using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GestaoDeContasPRO.Models
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var member = value.GetType()
                .GetMember(value.ToString())
                .FirstOrDefault();

            var displayAttribute = member?
                .GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.Name ?? value.ToString();
        }
    }

    public enum ActionType
    {
        [Display(Name = "Débito")]
        DEBIT,

        [Display(Name = "Crédito")]
        CREDIT
    }
}
