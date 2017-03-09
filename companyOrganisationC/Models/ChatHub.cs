using Microsoft.AspNet.SignalR;

namespace companyOrganisationC.Models
{
    public class ChatHub : Hub
    {
        public void SendMessage(string timeStamp, string username, string message)
        {
            Clients.All.showMessage(timeStamp, username, message);
        }
    }
}