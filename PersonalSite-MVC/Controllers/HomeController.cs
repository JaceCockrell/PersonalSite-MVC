using Microsoft.AspNetCore.Mvc;
using PersonalSite_MVC.Models;
using System.Diagnostics;
using MimeKit;
using MailKit.Net.Smtp;

namespace PersonalSite_MVC.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Resume()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel cvm)
        {
            //when a class has validation attributes, that validation should be checked before attempting to process any of the data provided.

            if (!ModelState.IsValid)
            {
                //send them back to the form. we can pass the object to the view so the form will contain the original information they provided.
                return View(cvm);
            }





            //to handle sending an email we have to install another nuget package and add some using statements.

            //To handle sending an email, we'll need to install another NuGet package
            //and add a few using statements.

            #region Email Setup Steps & Email Info

            //1. Go to Tools > NuGet Package Manager > Manage NuGet Packages for Solution
            //2. Go to the Browse tab and search for NETCore.MailKit
            //3. Click NETCore.MailKit
            //4. On the right, check the box next to the CORE1 project, then click "Install"
            //5. Once installed, return here
            //6. Add the following using statements & comments:
            //      - using MimeKit; //Added for access to MimeMessage class
            //      - using MailKit.Net.Smtp; //Added for access to SmtpClient class
            //7. Once added, return here to continue coding email functionality

            //MIME - multipurpose internet mail extensions - allows email to contain info other than ascii including audio, video, images html ect

            //SMPT - Simple mail transfer protocal - specializes in the collection and transfer of email data 

            #endregion

            //create the format for the message content we will recieve from the contact form
            string message = $"You have recieved a new email from your site's contact form!<br/>" +
                $"Sender: {cvm.Name}<br/>" +
                $"Email: {cvm.Email}<br/>" +
                $"Subject: {cvm.Subject}<br/>" +
                $"Message: {cvm.Message}";

            //create a MimeMessage object to assist with storing/transporting the email info from the contact form
            var mm = new MimeMessage();

            //we can access the credential for the email user from our appsetting.json file shown below
            mm.From.Add(new MailboxAddress("Sender", _config.GetValue<string>("Credentials:Email:User")));

            //The recipent of this email will be my personal email which is also stored in appsetting.json
            mm.To.Add(new MailboxAddress("Personal", _config.GetValue<string>("Credentials:Email:Recipient")));

            //subject will be the one provided by the user which is stored in the cvm object
            mm.Subject = cvm.Subject;

            //body of the message will be formatted with the string we created above
            mm.Body = new TextPart("HTML") { Text = message };

            //priority we can set it to urgent so it will be flagged in our email client
            mm.Priority = MessagePriority.Urgent;

            //the actual sender is our email from asp.net - we can add the users provided address to the list of ReplyTo addresses
            mm.ReplyTo.Add(new MailboxAddress("User", cvm.Email));

            //the using directive will create the SmtpClient object used to send the email
            //once all of the code has been executed it will close any open connections and dispose of the object for us.
            using (var client = new SmtpClient())
            {
                //connect to the mail server using the credentials in json
                client.Connect(_config.GetValue<string>("Credentials:Email:Client"));

                //log in to the mail server using the credentials for our email user
                client.Authenticate(

                    //username
                    _config.GetValue<string>("Credentials:Email:User"),


                    //password
                    _config.GetValue<string>("Credentials:Email:Password")


                    );
                //Its possible the mail server may be down when the user attempts to contact us, or that we have errors somewhere in our code.
                //so we can 'encapsulate' our code to send the message in a try/catch

                try
                {
                    //try to send the email
                    client.Send(mm);
                }
                catch (Exception ex)
                {
                    //if theres an issue we can store an error message in a viewbag to be displayed in the view
                    ViewBag.ErrorMessage = $"There was an error processing your request. Please " +
                        $"try again later.<br/>Error Message: {ex.StackTrace}";
                    //Return the user to the Contact View with thier form info intact
                    return View(cvm);
                    
                }

                

            }//end var client is disposed of automatically


            //if all goes well lets return a view that displays a confirmation to the user that states it was sent
            return View("EmailConfirmation", cvm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}