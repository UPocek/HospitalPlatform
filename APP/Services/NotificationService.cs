using System.Net.Mail;
using MongoDB.Driver;

public class NotificationService : CronJobService
{
    public NotificationService(IScheduleConfig<TransferService> config)
        : base(config.CronExpression, config.TimeZoneInfo)
    {
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return base.StartAsync(cancellationToken);
    }

    public override Task DoWork(CancellationToken cancellationToken)
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        var database = client.GetDatabase("USI");


        var notificationCollection = database.GetCollection<DrugNotification>("Notifications");
        var notifications = notificationCollection.Find(e => true).ToList();
        DateTime dateToday = DateTime.Today;


        foreach (var notification in notifications)
        {
   
            DateTime endDate = DateTime.Parse(notification.EndDate);
            if (endDate> dateToday){
                notificationCollection.DeleteOne(item => item._Id == notification._Id);
            }

            var time = DateTime.Parse(notification.Time);

            if (time.Hour == dateToday.Hour && time.Minute == dateToday.Minute){
                var smptClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new System.Net.NetworkCredential("teamnineMedical@gmail.com", "teamnine"),
                    EnableSsl = true,
                };

                string message = "Don't forget to take " + notification.Drug + "!";


                var mailMessage = new MailMessage
                {
                    From = new MailAddress("teamnineMedical@gmail.com"),
                    Subject = "TeamNine Medical Team - REMINDER",
                    Body = message,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(notification.Patient);
                smptClient.Send(mailMessage);
            }
        }

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }
}