namespace YaqraApi.DTOs
{
    public class GenericResultDto<T>
    {
        public string ErrorMessage { get; set; }
        public bool Succeeded { get; set; }
        public T Result { get; set; }
    }
}
