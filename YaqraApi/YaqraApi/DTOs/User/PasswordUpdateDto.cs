using System.ComponentModel.DataAnnotations;

namespace YaqraApi.DTOs.User
{
    public class PasswordUpdateDto
    {
        [Required, DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required, DataType(DataType.Password), Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}
