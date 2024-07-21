using FluentValidation;

namespace coupon;

public class CouponCreateValidation : AbstractValidator<CouponCreateDTO>
{
    public CouponCreateValidation() 
    {
        RuleFor(model => model.Name).NotEmpty();
        RuleFor(model => model.Name).Must(BeUniqueName).WithMessage("Coupon name already exists");
        
        RuleFor(model => model.Percent).InclusiveBetween(1, 50);
    }

    private bool BeUniqueName(string name)
    {
        // Assuming CouponStore is accessible here and contains the list of coupons
        return CouponStore.couponList.FirstOrDefault(x => x.Name.ToLower() == name.ToLower()) == null;
    }

}