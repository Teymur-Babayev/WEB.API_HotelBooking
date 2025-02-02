using BusinessLogic.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Validators
{
    public class RoomValidator : AbstractValidator<RoomDto>
    {
        public RoomValidator() 
        {
            RuleFor(x => x.Price)
                 .NotEmpty()
                 .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} can not be negative.");

            RuleFor(x => x.CategoryId)
                .NotEmpty();

            RuleFor(x => x.ImageUrl)
               .NotEmpty()
               .Must(LinkMustBeAUri).WithMessage("{PropertyName} must be a valid URL address.");

        }

        private static bool LinkMustBeAUri(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
            {
                return false;
            }

            //Courtesy of @Pure.Krome's comment and https://stackoverflow.com/a/25654227/563532
            Uri outUri;
            return Uri.TryCreate(link, UriKind.Absolute, out outUri)
                   && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
