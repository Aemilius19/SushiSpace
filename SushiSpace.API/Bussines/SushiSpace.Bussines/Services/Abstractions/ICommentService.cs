using SushiSpace.Core.DTOs;
using SushiSpace.Core.Entities;

namespace SushiSpace.Bussines.Services.Abstractions
{
    public interface ICommentService
    {
        Task<bool> Create(CommentDTO commentDTO);
        Task<List<CommentDTO>> GetAll(int productId);
        Task<bool> Delete(int id);

       
    }
}