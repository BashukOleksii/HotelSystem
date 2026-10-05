using lab_01.DTOs.User;

namespace lab_01.ViewModels.Admin
{
    public class AdminUserEditViewModel
    {
        public UserReadDto User { get; set; }
            = null!;

        public UserUpdateDto Form { get; set; }
            = new();

        public string SelectedRole { get; set; }
            = "User";

        public IReadOnlyList<string> Roles { get; set; }
            = [];
    }
}