using AutoMapper;
using FluentValidation;

using Microsoft.AspNetCore.Mvc;

using coupon;


var builder = WebApplication.CreateBuilder(args);

//add services to the container
builder.Services.AddEndpointsApiExplorer(); //dotnet add package Microsoft.AspNetCore.OpenApi
builder.Services.AddSwaggerGen(); //dotnet add package Swashbuckle.AspNetCore
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();      

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI();
}


app.MapGet("/api/coupons/", (ILogger<Program> _logger) =>
{
   _logger.Log(LogLevel.Information, "Getting all Coupons");
   return Results.Ok(CouponStore.couponList);
}).WithName("GetCoupons").Produces<IEnumerable<Coupon>>(200);

app.MapGet("/api/coupon/{id:int}", (int id) =>
{
   bool CouponMatchesId(Coupon c)
   {
      return c.Id == id;
   }
   return Results.Ok(CouponStore.couponList.FirstOrDefault(CouponMatchesId));
   //return Results.Ok(CouponStore.couponList.FirstOrDefault(x=>x.Id==id));
}).WithName("GetCoupon").Produces<Coupon>(200);

app.MapPost("/api/coupon",
         async (IMapper _mapper,
         IValidator<CouponCreateDTO> _validation,
         [FromBody] CouponCreateDTO coupon_createDTO) => 
{
   var validationResult = await _validation.ValidateAsync(coupon_createDTO);

   if (!validationResult.IsValid)
   {
      // return Results.BadRequest("Coupon name not provided");
       return Results.BadRequest(validationResult.Errors.FirstOrDefault().ToString());
   }

   Coupon coupon = _mapper.Map<Coupon>(coupon_createDTO);
   coupon.Id = CouponStore.couponList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
   CouponStore.couponList.Add(coupon);

   CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);
   //return Results.Ok(coupon);
   //return Results.Created($"/api/coupon/{coupon.Id}", coupon);
   return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, couponDTO);

}).WithName("CreateCoupon").Accepts<Coupon>("application/json").Produces<Coupon>(201).Produces(400);

app.MapPut("/api/coupon", () =>
{

});

app.MapDelete("/api/coupon/{id:int}", (int id) =>
{

});

app.Run();
