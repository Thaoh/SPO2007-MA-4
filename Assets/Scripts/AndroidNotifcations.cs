using UnityEngine;
using UnityEngine.Android;
using Unity.Notifications.Android;

public static class AndroidNotifcations
{
    public const string NOTIFICATIONPERMISSION = "android.permission.POST_NOTIFICATIONS";
    public const string CHANNELID = "default_channel";
    
    
    public static void RequestPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(NOTIFICATIONPERMISSION))
        {
            Permission.RequestUserPermission(NOTIFICATIONPERMISSION);
        }
    }
    public static void RegisterNotificationChannel()
    {
        var channel = new AndroidNotificationChannel(
            CHANNELID, "Default Channel", "DEsc", Importance.Default);

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }
    public static void SendNotification(string title, string content, double fireTimeInHours)
    {
        var notification = new AndroidNotification(
            title, content, System.DateTime.Now.AddHours(fireTimeInHours));
        notification.SmallIcon = "icon_0";
        notification.LargeIcon = "icon_1";

        AndroidNotificationCenter.SendNotification(notification, CHANNELID);
    }
}
