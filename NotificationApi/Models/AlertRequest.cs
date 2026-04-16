using NotificationApi.Models;

public class AlertRequest
{
    public string Message { get; set; } = string.Empty;
    public bool IsUrgent { get; set; } = false;
}