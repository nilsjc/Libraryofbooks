using LibraryDataService.DTOs;

namespace LibraryWebAPI.GraphQL.Types
{
    public class UserType
    {
        public string UserName { get; set; } = default!;

        public static UserType FromDTO(UserDTO dto)
        {
            return new UserType
            {
                UserName = dto.UserName
            };
        }
    }
}
