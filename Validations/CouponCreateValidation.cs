using FluentValidation;

namespace coupon;

public class CouponCreateValidation : AbstractValidator<CouponCreateDTO>
{
    public CouponCreateValidation() 
    {
        RuleFor(model => model.Name).NotEmpty();
        RuleFor(model => model.Percent).InclusiveBetween(1, 50);
    }
}