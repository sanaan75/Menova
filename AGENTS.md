# Menova Code Style and Rules

This guide records conventions observed in the repository's handwritten source. Follow these patterns when adding or changing code, and preserve existing public API behavior unless the task calls for changing it. The solution targets .NET 10 with nullable reference types and implicit usings enabled.

## Project layout and dependencies

- `Entities/` contains domain entities, enums, validation attributes, and shared message/glossary text. Entity classes generally implement `IEntity` and expose public properties for EF Core.
- `Services/` is the use-case and query layer (there is no project named `Application`). It contains service interfaces, implementations, request contracts, cache abstractions, and small response models.
- `Persistence/` contains `AppDbContext` and EF Core entity configurations.
- `Web/` contains web-facing infrastructure such as actor/session services, API token and logging middleware, exception handling, and Minimal API endpoint mappings.
- `App/` is the ASP.NET Core host and contains MVC controllers, `Program.cs`, and appsettings files.
- `InitEmptyDatabase/` is a small database initialization executable.
- Keep code in the corresponding existing project and feature folder. Feature folders are nested by area, for example `Services/Menus/Categories/` and `Services/Users/Auth/`; namespaces follow those folders.
- Project references show the main dependency direction: `Services` and `Persistence` depend on `Entities`; `Web` depends on `Entities` and `Services`; `App` composes all four. Some actor-related service contracts live in `Services` while their implementations live in `Web`.
- Several shared types and registration helpers come from `Dlls/Common/*.dll`, which is referenced but not included in this repository. Use those APIs as existing call sites demonstrate; do not assume their unseen implementation details.

## Business model and API client context

### Restaurant menu domain

- A `User` is the account/owner for a restaurant or cafe in this product flow. `User.Type` has exactly two values: `Customer = 1` and `Admin = 2`. The flags-based `User.BusinessType` describes the business category. The user's `Slug` is the public lookup key for the public menu endpoint.
- The menu hierarchy is `User -> Menu -> Category -> SubCategory -> MenuItem`. A `Menu` belongs to a user through `UserId`; a category belongs to a menu; a subcategory belongs to a category; and a menu item belongs to a subcategory. `MenuItem` also stores `UserId`, so preserve and verify owner scoping when adding or changing queries.
- Categories and subcategories carry names, descriptions, display order, optional image URLs, and active state. Category and subcategory notifications are separate child records with title, description, order, and active state.
- A menu item carries its name, description, display order, active state, and existence state. Photos are separate `MenuItemImage` records. Price choices are separate `MenuItemProperty` records with a title and nullable price; use these for choices such as small/large sizes and their prices. The `MenuItem` entity itself has no base `Price` property in the current model.
- `Plan` and `UserPlan` model subscription periods and limits such as item and image counts. The visible item and image creation services do not enforce those plan limits, so check the intended business rule and current call paths before adding enforcement.
- `AddUser` creates the new user and its initial, blank-name `Menu` in the same unit of work. `AddMenu` checks for an existing menu when the user already has an ID; the user creation path passes the tracked `User` before its generated ID is available.
- Management services commonly use the current actor and ownership-aware searches/checks. Keep ownership validation in the service layer, and scope category, subcategory, item, image, and property operations through the owning user's menu hierarchy. Existing commands differ in how explicitly they validate every parent, so inspect the specific path before relying on an existing check.
- `Admin` can view and manage records across all customers. `Customer` can view and manage only records under their own user/menu hierarchy. Apply this rule in services for writes and in controller search projections; an admin creating a category can pass its target `MenuId`, while customer creation always resolves the customer's own menu.
- Add services generally validate required names, positive display order where applicable, and duplicate names within the parent collection. Category and subcategory creation also saves an uploaded image through `ISaveLocalFile`.
- Category removal is blocked while it has subcategories. Subcategory removal is blocked while it has menu items or notifications. Preserve these dependency checks when changing deletion behavior.

### Plans, settings, and account access

- `Plan` is a catalog entry with a name, item/image limits, price, and active state. `AddPlan` creates active plans and resets `IPlanCache` after the database save. `SearchPlans` can filter by ID, name, and active state.
- `UserPlan` assigns a plan to a user for a start/end period. The add service prevents a duplicate assignment for the same user and plan, and the search service filters by ID, user, and plan. It does not currently enforce date ordering, plan activity, or user/plan existence; do not assume those checks happen elsewhere.
- `Setting` is a singleton database row in the current application flow. `ISettingsCache.Get()` expects exactly one row, `IEditSettings` updates the settings and queues cache invalidation after save, and `ISetActionLogSettings` is a narrower update for action-log retention limits. Settings are database-backed runtime values, separate from host configuration in `appsettings.json`.
- Authorization uses only `UserType.Customer` and `UserType.Admin`; there are no user groups or per-action permission records. Admin-only platform operations include plan and settings administration. Customers are scoped to their own menu and account records.

### API and React integration

- The React application is an API client and its source is not part of this repository. Treat route names, request fields, response field names, and authentication behavior as frontend contracts; check existing controller mappings before changing them.
- Management endpoints are primarily MVC actions under `/api/[controller]` and use POST routes such as `List`, `Add`, `Edit`, `Info`, and `Remove`. The public menu endpoint is `GET /api/cafe/menu/{slug}`; it looks up the user by slug and returns menu item data with category/subcategory labels and ordering, item properties, and image URLs.
- Platform management controllers use the same POST action style. Plan operations include plan listing/creation and user-plan listing/assignment; settings expose cached settings and edits. Menu management routes include listing menus for the current customer and listing all menus for Admin. Keep route and request names stable once consumed by React.
- Login returns a bearer token. Protected controller actions are marked with `ApiAuthorize`, and `ApiTokenMiddleware` validates the bearer token and creates the current actor. The public menu endpoint uses the slug and does not require that controller metadata.
- The API commonly returns `ApiResponseModel` (`Message` and `Data`) or an endpoint-specific object with a `Data` property. Preserve the response shape expected by the current React client; there is no shared typed frontend DTO project in this solution.
- CORS currently has an `AllowFrontend` policy configured for local development. Keep the actual deployed frontend origin aligned with that policy when changing environments.

### Business assumptions and current implementation details

- `MenuController.Menu()` uses `Single()` for the current user's menu, expressing an expectation of one menu per user. `AddMenu` checks for an existing menu for persisted users, but the visible menu configuration does not show a unique index on `UserId`; the database does not independently enforce the one-menu rule.
- `AppDbContext` registers `Plan`, `UserPlan`, and `UserApiToken` explicitly as `DbSet`s because their services query them directly. No user-group or permission entities belong to the current model.
- The public menu query filters out items whose category or subcategory is inactive. It does not explicitly filter `MenuItem.IsActive` in the current query. Preserve current behavior when the task requires compatibility; confirm the intended publication rule before changing visibility logic.
- The endpoint and entity names use `MenuItem` for a dish/product and `MenuItemProperty` for its options. Prefer these established names in API mappings and code even when the business wording uses “dish,” “product,” or “size.”

## C# naming and formatting

- Use file-scoped namespaces, four-space indentation, braces on separate lines for types and multi-line control blocks, and one public type per file, matching nearby files.
- Use PascalCase for namespaces, types, interfaces, methods, and public properties. Prefix interfaces with `I`; implementation class names normally match the interface without `I` (for example, `ISearchPlans` / `SearchPlans`).
- Use camelCase for parameters and local variables and `_camelCase` for private fields. Public constants and enum members use PascalCase in the prevailing convention.
- Keep `using` directives at the top of the file. Their ordering is not fully consistent, so follow the nearest related files rather than reformatting unrelated imports.
- Primary constructors are common for small services and controllers. Use a conventional constructor with private fields when the instance must retain dependencies, subscribe to cache events, or otherwise perform setup.
- Nullable reference types are enabled. Mark optional fields nullable and follow the nullability of neighboring contracts; older request types are not fully annotated, so avoid unrelated nullability churn.
- Preserve established serialized names and public enum values unless a business change explicitly replaces them. `UserType` is restricted to `Customer = 1` and `Admin = 2`; older accounts stored with type value `32` are mapped to Admin when actors are created or restored.

## Services and service contracts

- Put a public service interface and its implementation in the same feature folder and namespace when they belong to the same layer. Name the interface `IFoo` and implementation `Foo`.
- Service operations conventionally use `Respond(...)`. Keep the service contract focused on one operation or closely related query, and inject collaborators through the constructor rather than resolving them globally.
- Put operation input types inside the interface as a nested `Request` class when the operation needs a structured input. Callers construct it with an object initializer. Cache interfaces commonly define a nested `Model` for the projected cache shape.
- Existing return shapes include domain entities for adds, `void` for state changes, `IQueryable<TEntity>` for searches, and cache models for cache access. Match the established contract in the feature rather than introducing a new result framework.
- Search services generally start from `db.Query<TEntity>()`, return early for a null/default request using `request.IsNullOrDefault()`, apply shared filters (`FilterById`, `IgnoreById`, `Filter`) and feature-specific LINQ predicates, then return a deterministic ordering. Return `IQueryable` when callers need to compose or project the query; materialize only when the result must be enumerated or cached.
- Keep business validation and ownership checks in services. Use the shared `Check` helpers with a message factory, such as `Check.Given`, `Check.NotNull`, `Check.True`, `Check.False`, `Check.Positive`, and `Check.Defined`. Build user-facing text from `Glossary`, `ErrorMessagePersian` / `ErrorMessage`, or `CommonMessages` rather than scattering duplicate wording.
- Most write services update tracked entities through `db.Set<TEntity>()` and return the `.Entity` for adds. They usually leave saving to the calling boundary. In MVC actions, call `db.Save()` or `db.SaveAsync()` after a successful command. A few existing services that use a separate statistics context save internally; follow the specific service's established boundary.
- When a write changes cached data, queue invalidation with `db.AddPostSaveAction(cache.Reset)` so the cache resets after a successful save. Cache implementations marked `ISingleInstance` are registered as singletons; they use the container/cache abstractions and should not capture a scoped database context in a singleton.
- Most service methods let validation and data-access exceptions propagate to the web error handling. Catch exceptions only when the service has a clear recovery or logging responsibility. `AddUserLogin` is an existing exception: it catches a statistics write failure and logs it through `ILogger<AddUserLogin>`.
- Prefer real asynchronous work for async methods. A few existing methods wrap synchronous work in `Task` or are marked `async` without awaiting; treat that as legacy variation rather than a pattern to extend.

## Mapping and response models

- There is no visible object-mapping package or central mapper. Map query results explicitly with LINQ `Select` projections, usually to anonymous objects at the controller boundary or to a nested cache `Model` in cache fill logic.
- `Services/Models/ApiResponseModel` is the common API envelope with `Message` and `Data`; `IdModel` and `PaginationModel` are small shared input models. Existing endpoints also return anonymous `{ Data = ... }` objects or domain results, so preserve the response shape already used by the endpoint.
- Keep API projections deliberate: select only the fields the endpoint needs, and preserve existing property names such as `Items`, `TotalItems`, `ImageUrl`, or `TypeCaption` when changing a response.

## MVC controllers and Minimal APIs

- Put controllers under `App/Controllers/`, with namespaces that mirror subfolders (for example, `App.Controllers.Users`). Controllers use constructor injection, commonly primary constructors, and currently derive from `Controller`.
- Follow the existing attribute route style: `[ApiController]`, a controller-level `[Route("api/[controller]")]` (or the established `api/basics/[controller]` route), and action attributes such as `[HttpPost, Route("List")]`. Preserve `ApiAuthorize` on protected routes and use `[AllowAnonymous]` for explicitly public authentication actions.
- Plan creation/assignment and settings endpoints are restricted to Admin in addition to `ApiAuthorize`. Customers can view active plans and only their own `UserPlan` records. Customer menu ownership is checked in services; controller list queries use the current customer's ID, while Admin list queries can span all customers.
- Keep actions thin: pass a request to a service, use `IActorService` for the current actor where needed, save after commands, and return `Ok(...)` with the established envelope or projection. Put ownership and domain validation in the service layer rather than duplicating it in actions.
- For reads, project the service/query result into the endpoint's response shape. For writes, invoke the command service and persist through the injected `IDb`. Do not add a save to a read action.
- Minimal API mappings live in `Web/MinimalApis/`. Register them through `EndpointExtensions.MapAllEndpoints`, group related routes with `MapGroup` and tags, and keep endpoint handlers small with services, `CancellationToken`, and async EF calls injected as parameters where appropriate.

## Web infrastructure, errors, and logging

- Keep middleware and web infrastructure in `Web/`. Existing middleware uses either the `RequestDelegate` constructor pattern or implements `IMiddleware`; `LoggingMiddleware` is registered with a scoped lifetime.
- The API token middleware checks endpoint metadata for `ApiAuthorize`, validates the bearer token, creates an actor, and continues the pipeline. Preserve that metadata-driven authorization flow when adding protected API endpoints.
- MVC API exceptions are handled by the globally registered `ApiExceptionFilter`. `AppExceptionHandler`, `ApiLogMiddleware`, `ErrorLogMiddleware`, and `SecurityHeadersMiddleware` exist in `Web/`, but are not registered in the current `Program.cs`; do not assume they are active without wiring them into the pipeline. `ApiTokenMiddleware` and `LoggingMiddleware` are registered.
- The current host adds the exception filter in `AddControllers`. Keep error translation centralized and use the shared `AppException`/`Check` conventions for expected application errors. Avoid returning raw unexpected exception details from new API responses.
- Use `ILogger<T>` for service logging when logging is required. The logging middleware records API request and error details in the statistics database, applies body-size limits, and sanitizes sensitive JSON fields; do not log passwords, tokens, or other secrets.

## Dependency injection, startup, and configuration

- The host is composed in `App/Program.cs` using `WebApplication.CreateBuilder(args)`. Register controllers, database contexts, shared services, and middleware before `builder.Build()`; map controllers and Minimal APIs after middleware setup.
- The host scans local and shared assemblies with `RegisterAssemblyPublicNonGenericClasses` and the dummy marker classes. Implementations are scoped by default; interfaces inheriting `ISingleInstance` are registered as singletons. Generic cache types, rate limiting, and a few infrastructure services are registered explicitly. Preserve these lifetime markers and scan behavior when adding services.
- The existing middleware order is deliberate: production exception handling/HSTS, HTTPS redirection, static files, routing, CORS, authentication, authorization, API token middleware, logging middleware, then controller and Minimal API mappings. Check endpoint metadata and dependencies before changing this order.
- Configuration is currently read with direct key lookups such as `builder.Configuration["ConnectionStrings:MainConnection"]` and `builder.Configuration["ConnectionStrings:StatisticConnection"]`. No typed `IOptions<T>` binding convention appears in the source. `Services/Settings/SettingsCache` loads runtime settings from the database, not from appsettings. Follow the existing configuration keys unless a task intentionally introduces a consistent options-binding pattern.
- Never copy connection-string credentials or other secrets into documentation or new source. The repository currently has SQL credentials in host/configuration files; do not repeat those values, and use environment variables or a secret store for new or deployed configuration.

## EF Core conventions

- Put `IEntityTypeConfiguration<TEntity>` classes in `Persistence/Configs/`, one per entity, and apply them through `ApplyConfigurationsFromAssembly` in `AppDbContext`.
- `AppDbContext.Query<TEntity>()` is no-tracking and ordered by descending `Id`; use `Set<TEntity>()` for tracked mutations. The context also centralizes delete behavior and supports post-save cache callbacks.
- Keep database-specific mapping and indexes in `Persistence`, not in controllers. Domain entities and relationships belong in `Entities`.
- No EF Core migration source is checked into this repository. Updating the model/configuration supports a newly created database, but an existing database still needs its schema updated through the deployment's database process.
- The current model no longer contains group, membership, or permission entities. Existing databases retain their old physical tables until the deployment database process removes them; accounts stored with the legacy type value `32` are treated as Admin at authentication time.

