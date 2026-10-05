using lab_01.DTOs.User;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace lab_01.ViewModels.Admin
{
    public class AdminUserEditViewModel
    {
        [ValidateNever]
        public UserReadDto User { get; set; }
            = null!;

        public UserUpdateDto Form { get; set; }
            = new();

        public string SelectedRole { get; set; }
            = "User";

        [ValidateNever]
        public IReadOnlyList<string> Roles { get; set; }
            = [];
    }
}