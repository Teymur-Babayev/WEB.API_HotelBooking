using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DataAccess.Repositories;
using BusinessLogic.Entities;
using BusinessLogic.DTOs;
using BusinessLogic.Specifications;

namespace BusinessLogic.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IMapper mapper;
        private readonly IJwtService jwtService;
        private readonly IRepository<RefreshToken> refreshTokenR;

        public AccountsService(UserManager<User> userManager,
                                SignInManager<User> signInManager,
                                IMapper mapper,
                                IJwtService jwtService,
                                IRepository<RefreshToken> refreshTokenR)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mapper = mapper;
            this.jwtService = jwtService;
            this.refreshTokenR = refreshTokenR;
        }

        public async Task Register(RegisterModel model)
        {
            // TODO: validation

            // check user
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user != null)
                throw new HttpException("Email is already exists.", HttpStatusCode.BadRequest);

            // create user
            var newUser = mapper.Map<User>(model);
            var result = await userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded)
                throw new HttpException(string.Join(" ", result.Errors.Select(x => x.Description)), HttpStatusCode.BadRequest);
        }

        public async Task<LoginResponseDto> Login(LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
                throw new HttpException("Invalid user login or password.", HttpStatusCode.BadRequest);

            //await signInManager.SignInAsync(user, true);

            // generate token
            return new LoginResponseDto
            {
                AccessToken = jwtService.CreateToken(jwtService.GetClaims(user)),
                RefreshToken = CreateRefreshToken(user.Id).Token
            };
        }

        private RefreshToken CreateRefreshToken(string userId)
        {
            var refeshToken = jwtService.CreateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refeshToken,
                UserId = userId,
                CreationDate = DateTime.UtcNow // Now vs UtcNow
            };

            refreshTokenR.Insert(refreshTokenEntity);
            refreshTokenR.Save();

            return refreshTokenEntity;
        }

        public async Task<UserTokens> RefreshTokens(UserTokens userTokens)
        {
            var refrestToken = await refreshTokenR.GetItemBySpec(new RefreshTokenSpecs.ByToken(userTokens.RefreshToken));

            if (refrestToken == null)
                throw new HttpException(Errors.InvalidToken, HttpStatusCode.BadRequest);

            var claims = jwtService.GetClaimsFromExpiredToken(userTokens.AccessToken);
            var newAccessToken = jwtService.CreateToken(claims);
            var newRefreshToken = jwtService.CreateRefreshToken();

            refrestToken.Token = newRefreshToken;

            // TODO: update creation time
            refreshTokenR.Update(refrestToken);
            refreshTokenR.Save();

            var tokens = new UserTokens()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            return tokens;
        }

        public async Task Logout(string refreshToken)
        {
            //await signInManager.SignOutAsync();

            var refrestTokenEntity = await refreshTokenR.GetItemBySpec(new RefreshTokenSpecs.ByToken(refreshToken));

            if (refrestTokenEntity == null)
                throw new HttpException(Errors.InvalidToken, HttpStatusCode.BadRequest);

            refreshTokenR.Delete(refrestTokenEntity);
            refreshTokenR.Save();
        }

        public async Task RemoveExpiredRefreshTokens()
        {
            var lastDate = jwtService.GetLastValidRefreshTokenDate();
            var expiredTokens = await refreshTokenR.GetListBySpec(new RefreshTokenSpecs.CreatedBy(lastDate));

            foreach (var i in expiredTokens)
            {
                refreshTokenR.Delete(i);
            }
            refreshTokenR.Save();
        }
    }
}
