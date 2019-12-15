using FluentValidation;

namespace OpenApi.Dtos
{
    public class CursorPagedMetaDtoValidator : AbstractValidator<CursorPagedMetaDto>
    {
        public CursorPagedMetaDtoValidator()
        {
            RuleFor(dto => dto.Before)
                .Length(32);

            RuleFor(dto => dto.After)
                .Length(32);

            RuleFor(dto => dto.Limit)
                .NotEmpty();

            RuleFor(dto => dto.Count)
                .NotEmpty();
        }
    }
}
