## Minimal template for clean web api with vertical sliced architecture based on [AutoMapper](https://automapper.org/), [MediatR](https://github.com/jbogard/MediatR) and [FluentValidation](https://fluentvalidation.net/)
#### Points of interest
* 1:1 Mapping is setup over a `IMapFrom<,>` interface, for more complex cased implement a `public void Mapping(Profile profile)` method to override to default implementation of the interface
* Implement a `AbstractValidator<,>` to setup validation
* `Controllers` should only contain a reference to `IMediator` and nothing else
* MediatR handles all request, commands and notifications
    * `Commands` = creates and updates
    * `Queries` = queries 
    * `RequestValidationBehavior` applies validations from FluentValidation to data that is passed in based on their respective `AbstractValidator<,>`
* `CustomExceptionHandlerMiddleware` is a middleware that captures exceptions and display them in a better unified format

#### Use library based architecture for larger projects

![alt text](https://i.imgur.com/RhcxJ82.png "EU Pronet")
