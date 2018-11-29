using Blog.Core.Model.Models;
using Blog.Core.IRepository.Base;
namespace Blog.Core.IRepository
{
    public interface IGuestBookRepository : IBaseRepository<GuestBook>
    {
    }
}