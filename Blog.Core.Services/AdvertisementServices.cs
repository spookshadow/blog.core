using Blog.Core.IRepository;
using Blog.Core.IServices;
using Blog.Core.Model;
using Blog.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Blog.Core.Services
{
    public class AdvertisementServices : BaseServices<Advertisement>, IAdvertisementServices
    {

    }
}
