using lab_01.DTOs.User;

namespace lab_01.ViewModels.Admin
{
    public class AdminUserCreateViewModel
    {
        public UserCreateDto Form { get; set; }
            = new();

        public string SelectedRole { get; set; }
            = "User";

        public IReadOnlyList<string> Roles { get; set; }
            = [];
    }
}