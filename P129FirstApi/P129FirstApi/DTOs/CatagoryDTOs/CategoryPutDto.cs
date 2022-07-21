using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P129FirstApi.DTOs.CatagoryDTOs
{
    public class CategoryPutDto
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Sekli { get; set; }
        public bool Esasdirmi { get; set; }
        public Nullable<int> AidOlduguUstCategoryId { get; set; }
    }

    public class CategoryPutDtoValidator : AbstractValidator<CategoryPutDto>
    {
        public CategoryPutDtoValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id Mecburidi");

            RuleFor(x => x.Ad)
                .MaximumLength(255).WithMessage("Ad Maksimum 255 Simvol Ola Biler")
                .MinimumLength(10).WithMessage("Ad Minimum 10 Simvol Ola Biler")
                .NotEmpty().WithMessage("Mecburidi Qaqa");

            RuleFor(x => x.Sekli)
                .MaximumLength(1000).WithMessage("Sekli Maksimum 1000 Simvol Ola Biler")
                .MinimumLength(5).WithMessage("Ad Minimum 5 Simvol Ola Biler");

            RuleFor(x => x).Custom((x, context) =>
            {
                if (x.Esasdirmi)
                {
                    if (x.Sekli == null)
                    {
                        context.AddFailure("Sekil Mutleqdir");
                    }
                }
                else
                {
                    if (x.AidOlduguUstCategoryId == null)
                    {
                        context.AddFailure("AidOlduguUstCategoryId Mutleqdir");
                    }

                    if (x.Id == x.AidOlduguUstCategoryId)
                    {
                        context.AddFailure("Id AidOlduguUstCategoryId Beraber Ola Bilmez");
                    }
                }
            });
        }
    }
}
