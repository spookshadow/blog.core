using AutoMapper;
using Blog.Core.Model.Models;
using Blog.Core.Model.VeiwModels;

/// <summary>
/// AutoMapper 映射类
/// </summary>
public class CustomProfile : Profile
{
    /// <summary>
    /// 配置构造函数，用来创建关系映射
    /// </summary>
    public CustomProfile()
    {
        CreateMap<Advertisement, AdvertisementVM>();
    }
}