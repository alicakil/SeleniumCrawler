namespace Dashboard.Dtos
{
    public class ResponseDto<T>
    {
        public bool Result { get; set; }
        public string Msg { get; set; }
        public T Payload { get; set; }
    }
}
