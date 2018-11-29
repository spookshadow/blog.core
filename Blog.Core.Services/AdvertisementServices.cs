using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Core.Common;
using Blog.Core.IRepository;
using Blog.Core.IServices;
using Blog.Core.Model.Models;
using Blog.Core.Services.Base;

namespace Blog.Core.Services
{
    public class AdvertisementServices : BaseServices<Advertisement>, IAdvertisementServices
    {
        IAdvertisementRepository dal;
        public AdvertisementServices(IAdvertisementRepository dal)
        {
            this.dal = dal;
            base.baseDal = dal;
        }

        [Caching(AbsoluteExpiration = 10)]
        public Task ActionTest(int id)
        {
            return Task.Run(() => { Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxx"); });
        }

        [Caching(AbsoluteExpiration = 10)]
        public List<Advertisement> QueryTest(int id)
        {
            var tk = dal.Query(t => t.Id == id);
            tk.ConfigureAwait(false);
            return tk.Result;
        }
    }
}
