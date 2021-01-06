using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace is4FirstDemo.IdentityUserStore
{
    public class UserStore1
    {
        private readonly UserStoreDbContext1 _dbContext;
        public UserStore1(UserStoreDbContext1 dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// 根据SubjectID查询用户信息
        /// </summary>
        /// <param name="subjectId">用户id</param>
        /// <returns></returns>
        public IdentityUser1 FindBySubjectId(string subjectId)
        {
            return _dbContext.Set<IdentityUser1>().Where(r => r.SubjectId.Equals(subjectId)).Include(r => r.IdentityUserClaims).SingleOrDefault();
        }
        /// <summary>
        /// 根据用户名查询用户
        /// </summary>
        /// <param name="username">用户</param>
        /// <returns></returns>
        public IdentityUser1 FindByUsername(string username)
        {
            return _dbContext.Set<IdentityUser1>().Where(r => r.Username.Equals(username)).Include(r => r.IdentityUserClaims).SingleOrDefault();
        }
        /// <summary>
        /// 验证登录密码
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ValidateCredentials(string username, string password)
        {
            //password = Config.MD5Str(password);
            var user = _dbContext.Set<IdentityUser1>().Where(r => r.Username.Equals(username)
            && r.Password.Equals(password)).Include(r => r.IdentityUserClaims).SingleOrDefault();
            return user != null;
        }
    }
}
