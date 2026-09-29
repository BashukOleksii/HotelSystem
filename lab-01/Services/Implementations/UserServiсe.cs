using AutoMapper;
using FluentValidation;
using lab_01.Common.Pagination;
using lab_01.DTOs.User;
using lab_01.Exceptions;
using lab_01.Models.Entities;
using lab_01.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace lab_01.Services.Implementations
{
    public class UserService : IUserService
    {
        private const string DefaultRole = "User";

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IMapper _mapper;

        private readonly IValidator<UserCreateDto> _createValidator;
        private readonly IValidator<UserUpdateDto> _updateValidator;
        private readonly IValidator<UserFilterDto> _filterValidator;

        public UserService(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            IValidator<UserCreateDto> createValidator,
            IValidator<UserUpdateDto> updateValidator,
            IValidator<UserFilterDto> filterValidator)
        {
            _userManager = userManager;
            _roleManager = roleManager;

            _mapper = mapper;

            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _filterValidator = filterValidator;
        }

        public async Task<UserReadDto> GetByIdAsync(
             string id)
        {
            User? user =
                await _userManager.FindByIdAsync(
                    id
                );

            if (user is null)
            {
                throw new NotFoundException(
                    nameof(User),
                    id
                );
            }

            return await MapUserAsync(
                user
            );
        }

        public async Task<PagedResult<UserReadDto>>
         SearchAsync(
        UserFilterDto filter)
        {
            await _filterValidator
                .ValidateAndThrowAsync(
                    filter
                );

            IQueryable<User> users =
                _userManager.Users
                    .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(
                filter.Search))
            {
                string search =
                    filter.Search.Trim();

                users = users.Where(user =>
                    user.UserName != null &&
                    user.UserName.Contains(search)
                    ||
                    user.Email != null &&
                    user.Email.Contains(search)
                    ||
                    user.PhoneNumber != null &&
                    user.PhoneNumber.Contains(search)
                );
            }

            int totalCount =
                await users.CountAsync();

            users =
                ApplySorting(
                    users,
                    filter
                );

            List<User> pageUsers =
                await users
                    .Skip(
                        (filter.Page - 1) *
                        filter.PageSize
                    )
                    .Take(
                        filter.PageSize
                    )
                    .ToListAsync();

            List<UserReadDto> items = [];

            foreach (User user in pageUsers)
            {
                items.Add(
                    await MapUserAsync(user)
                );
            }

            return new PagedResult<UserReadDto>
            {
                Items = items,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount
            };
        }

        private static IQueryable<User> ApplySorting(
            IQueryable<User> users,
            UserFilterDto filter)
        {
            return filter.SortBy?.ToLower() switch
            {
                "username" => filter.Descending
                    ? users.OrderByDescending(
                        user => user.UserName
                    )
                    : users.OrderBy(
                        user => user.UserName
                    ),

                "email" => filter.Descending
                    ? users.OrderByDescending(
                        user => user.Email
                    )
                    : users.OrderBy(
                        user => user.Email
                    ),

                "createdat" => filter.Descending
                    ? users.OrderByDescending(
                        user => user.CreatedAt
                    )
                    : users.OrderBy(
                        user => user.CreatedAt
                    ),

                _ => users.OrderBy(
                    user => user.UserName
                )
            };
        }

        public async Task<IReadOnlyList<string>>
         GetRolesAsync()
        {
            List<string> roles =
                await _roleManager.Roles
                    .AsNoTracking()
                    .Where(role =>
                        role.Name != null
                    )
                    .OrderBy(role =>
                        role.Name
                    )
                    .Select(role =>
                        role.Name!
                    )
                    .ToListAsync();

            return roles;
        }

        public async Task<UserReadDto> CreateAsync(
    UserCreateDto dto)
        {
            await _createValidator
                .ValidateAndThrowAsync(
                    dto
                );

            User user = new User
            {
                UserName =
                    dto.UserName.Trim(),

                Email =
                    dto.Email.Trim(),

                PhoneNumber =
                    dto.PhoneNumber?.Trim()
            };

            IdentityResult createResult =
                await _userManager.CreateAsync(
                    user,
                    dto.Password
                );

            EnsureSuccess(
                createResult
            );

            bool roleExists =
                await _roleManager.RoleExistsAsync(
                    DefaultRole
                );

            if (!roleExists)
            {
                await _userManager.DeleteAsync(
                    user
                );

                throw new NotFoundException(
                    $"Роль '{DefaultRole}' не знайдена."
                );
            }

            IdentityResult roleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    DefaultRole
                );

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(
                    user
                );

                EnsureSuccess(
                    roleResult
                );
            }

            return await MapUserAsync(
                user
            );
        }

        public async Task<UserReadDto> UpdateAsync(
            string id,
            UserUpdateDto dto)
        {
            await _updateValidator
                .ValidateAndThrowAsync(
                    dto
                );

            User? user =
                await _userManager.FindByIdAsync(
                    id
                );

            if (user is null)
            {
                throw new NotFoundException(
                    nameof(User),
                    id
                );
            }

            if (dto.UserName is not null)
            {
                user.UserName =
                    dto.UserName.Trim();
            }

            if (dto.Email is not null)
            {
                user.Email =
                    dto.Email.Trim();
            }

            if (dto.PhoneNumber is not null)
            {
                user.PhoneNumber =
                    dto.PhoneNumber.Trim();
            }

            user.UpdatedAt =
                DateTime.UtcNow;

            IdentityResult result =
                await _userManager.UpdateAsync(
                    user
                );

            EnsureSuccess(
                result
            );

            return await MapUserAsync(
                user
            );
        }

        public async Task<UserReadDto> ChangeRoleAsync(
            string id,
            string roleName)
        {
            if (string.IsNullOrWhiteSpace(
                roleName))
            {
                throw new ArgumentException(
                    "Назва ролі не може бути порожньою.",
                    nameof(roleName)
                );
            }

            User? user =
                await _userManager.FindByIdAsync(
                    id
                );

            if (user is null)
            {
                throw new NotFoundException(
                    nameof(User),
                    id
                );
            }

            bool roleExists =
                await _roleManager.RoleExistsAsync(
                    roleName
                );

            if (!roleExists)
            {
                throw new NotFoundException(
                    $"Роль '{roleName}' не знайдена."
                );
            }

            IList<string> currentRoles =
                await _userManager.GetRolesAsync(
                    user
                );

            if (!currentRoles.Contains(
                roleName))
            {
                IdentityResult addResult =
                    await _userManager.AddToRoleAsync(
                        user,
                        roleName
                    );

                EnsureSuccess(
                    addResult
                );
            }

            string[] rolesToRemove =
                currentRoles
                    .Where(role =>
                        role != roleName
                    )
                    .ToArray();

            if (rolesToRemove.Length > 0)
            {
                IdentityResult removeResult =
                    await _userManager
                        .RemoveFromRolesAsync(
                            user,
                            rolesToRemove
                        );

                EnsureSuccess(
                    removeResult
                );
            }

            user.UpdatedAt =
                DateTime.UtcNow;

            IdentityResult updateResult =
                await _userManager.UpdateAsync(
                    user
                );

            EnsureSuccess(
                updateResult
            );

            return await MapUserAsync(
                user
            );
        }

        public async Task DeleteAsync(
            string id)
        {
            User? user =
                await _userManager.FindByIdAsync(
                    id
                );

            if (user is null)
            {
                throw new NotFoundException(
                    nameof(User),
                    id
                );
            }

            IdentityResult result =
                await _userManager.DeleteAsync(
                    user
                );

            EnsureSuccess(
                result
            );
        }

        private static void EnsureSuccess(
            IdentityResult result)
        {
            if (result.Succeeded)
                return;

            throw new IdentityOperationException(
                result.Errors.Select(
                    error => error.Description
                )
            );
        }

        private async Task<UserReadDto> MapUserAsync(
             User user)
        {
            UserReadDto dto =
                _mapper.Map<UserReadDto>(
                    user
                );

            IList<string> roles =
                await _userManager.GetRolesAsync(
                    user
                );

            dto.Roles =
                roles.ToList();

            return dto;
        }
    }
}