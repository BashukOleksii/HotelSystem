using lab_01.Common.Pagination;
using lab_01.DTOs.User;

namespace lab_01.ViewModels.Admin
{
    public class AdminUserIndexViewModel
    {
        public UserFilterDto Filter { get; set; }
            = new();

        public PagedResult<UserReadDto> Users { get; set; }
            = new();
    }
}