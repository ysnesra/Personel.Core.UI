//using System;
//using System.Linq;


//namespace Personel.Core.UI.Security
//{
//    public class PersonelRoleProvider : RoleProvider
//    {
//        public override string ApplicationName
//        {
//            get { throw new NotImplementedException(); }

//            set { throw new NotImplementedException(); }
//        }
//        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
//        {
//            throw new NotImplementedException();
//        }

//        public override void CreateRole(string roleName)
//        {
//            throw new NotImplementedException();
//        }

//        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
//        {
//            throw new NotImplementedException();
//        }

//        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
//        {
//            throw new NotImplementedException();
//        }

//        public override string[] GetAllRoles()
//        {
//            throw new NotImplementedException();
//        }

//        public override string[] GetRolesForUser(string username)
//        {
//            PersonelDbEntities db = new PersonelDbEntities();
//            var users = db.Users.FirstOrDefault(x => x.Name == username);
//            return new string[] { users.Role };
//        }

//        public override string[] GetUsersInRole(string roleName)
//        {
//            throw new NotImplementedException();
//        }

//        public override bool IsUserInRole(string username, string roleName)
//        {
//            throw new NotImplementedException();
//        }

//        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
//        {
//            throw new NotImplementedException();
//        }

//        public override bool RoleExists(string roleName)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}