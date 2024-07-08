using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SushiSpace.Application.Repositories.Abstractions;
using SushiSpace.Bussines.Services.Abstractions;
using SushiSpace.Core.DTOs;
using SushiSpace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Bussines.Services.Concretes
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public CommentService(ICommentRepository repository, IMapper mapper, UserManager<User> userManager)
        {
            _repository = repository;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<bool> Create(CommentDTO commentDTO)
        {
            var user=await _userManager.FindByNameAsync(commentDTO.UserName);
            
            var comment = _mapper.Map<Comment>(commentDTO);
            comment.User = user;
            var result = await _repository.Create(comment);
            result.CreateTime = DateTime.Now;
            
            if (result is not null)
            {
                return true;
            }
            return false;
        }

        public async Task<List<CommentDTO>> GetAll(int productId)
        {
            var commentdto= await _repository.GetAll(null,c=>c.ProductId==productId, "Product","User");
            
            List<CommentDTO> result= new List<CommentDTO>();
            foreach (var comment in commentdto)
            {
                var com=_mapper.Map<CommentDTO>(comment);
                com.UserName = comment.User.UserName;
                result.Add(com);
            }
            return result;
        }

        public async Task<bool> Delete(int id)
        {
            var result = await _repository.Delete(id);
            return result;
        }
    }
}
