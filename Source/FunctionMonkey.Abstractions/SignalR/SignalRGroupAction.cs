namespace FunctionMonkey.Abstractions.SignalR
{
    public enum GroupActionEnum
    {
        Add,
        Remove
    }

    public class SignalRGroupAction
    {
        public string UserId { get; set; }

        public string GroupName { get; set; }

        public GroupActionEnum Action { get; set; }
    }
}
