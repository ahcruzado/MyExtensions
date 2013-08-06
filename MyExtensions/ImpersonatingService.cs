using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Web.Security;
using System.Configuration;
using System.Web.Configuration;

namespace MyExtensions
{
    public class ImpersonatingService
    {
        HttpContextBase _Context;
        IPrincipal _CurrentUser { get { return _Context.User; } }

        public ImpersonatingService(HttpContextBase context)
        {
            _Context = context;
            _ImpersonationStack = new Lazy<Stack<string>>(() =>
            {
                string result = "";
                if (_CurrentUser != null)
                {
                    var identity = (FormsIdentity)_CurrentUser.Identity;
                    if (identity != null)
                        result = identity.Ticket.UserData;
                }
                var userDataList = result.ToCollection<string>().Where(s => !string.IsNullOrEmpty(s));
                return new Stack<string>(userDataList.Reverse());
            });
        }

        public void ClearImpersonation()
        {
            while (ImpersonationStack.Count() != 0)
            {
                Disimpesonate();
            }
        }

        public void Disimpesonate()
        {
            var userName = ImpersonationStack.Pop();
            setCurrentUser(userName);
        }

        public void Impersonate(string userName)
        {
            ImpersonationStack.Push(_CurrentUser.Identity.Name);
            setCurrentUser(userName);
        }

        private void setCurrentUser(string userName)
        {
            SessionStateSection section = ConfigurationManager.GetSection("system.web/sessionState") as SessionStateSection;

            var userData = ImpersonationStack.ToStringAsCollection() ?? "";
            var ticket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now + section.Timeout, true, userData);
            string encTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            _Context.Response.Cookies.Add(faCookie);
        }

        public string GetRealUserName()
        {
            var result = ImpersonationStack.LastOrDefault();
            if (result == null)
                result = _CurrentUser.Identity.Name;
            return result;
        }

        public IPrincipal GetImpersonatorPrincipal()
        {
            IPrincipal result = null;
            var impersonatorUserName = GetRealUserName();
            if (impersonatorUserName != null)
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(impersonatorUserName, true, TimeSpan.FromMinutes(30).Minutes);
                result = new RolePrincipal(new FormsIdentity(ticket));
            }
            return result;
        }

        public bool IsImpersonating()
        {
            return ImpersonationStack.Any();
        }

        public Lazy<Stack<string>> _ImpersonationStack;
        public Stack<string> ImpersonationStack
        {
            get
            {
                return _ImpersonationStack.Value;
            }
        }
    }
}