# Backend Implementation: ThyroCareX (Comprehensive)

We will now explore the Backend Aspects of ThyroCareX. The project is based on a modern **Clean Architecture (Onion Architecture)** pattern, and the **.NET Core** framework was used to build the backend.

The system is decoupled into 5 main layers (projects) to ensure separation of concerns, testability, and scalability. These layers are:
1. **API Presentation Layer** (`ThyroCareX`)
2. **Application Core Layer** (`ThyroCareX.Core`)
3. **Data/Domain Layer** (`ThyroCareX.Data`)
4. **Infrastructure Layer** (`ThyroCareX.Infrastructure`)
5. **Business Service Layer** (`ThyroCareX.Service`)

When a new request reaches the server, it first goes to the **"Controllers"** in the API presentation layer. To handle these requests efficiently, the system utilizes the **CQRS (Command Query Responsibility Segregation)** pattern via the **MediatR** library. 

---

## 1. Controllers Structure (API Layer)

Controllers are located in the `ThyroCareX` main project (Presentation Layer). The namespace contains all the controllers used to expose the services as a REST API for the frontend and mobile clients. 

`AppControllerBase` is the base class that controllers inherit from, which provides the main functionality for handling requests, responses, and MediatR dispatching.

The API exposes the following main controllers:
- `AuthenticationController`: Manages user login, registration, and tokens.
- `DoctorProfileController` & `PatientController`: Manages profiles.
- `TestsWithAIController`: The core diagnostic controller (processes Images, FNAC, Clinical data).
- `PaymentController`: Integration endpoint for handling Stripe payments/webhooks.
- `CommunityController`: Social features (Posts, Comments, Likes).
- `PlanController`: Subscription plans management.

### Example Code for API Controllers: TestsWithAIController

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThyroCareX.Bases;
using ThyroCareX.Core.Feature.TestWithAI.Commands.Models;
using ThyroCareX.Data.Healpers.ClinicalAI;

namespace ThyroCareX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor")]
    public class TestsWithAIController : AppControllerBase
    {
        [HttpPost("ProcessImage")]
        public async Task<IActionResult> ProcessImage([FromForm] PredictImageCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("ProcessClinical")]
        public async Task<IActionResult> ProcessClinical([FromBody] ClinicalRequest request)
        {
            var response = await Mediator.Send(new AssessClinicalCommand(request));
            return Ok(response);
        }
    }
}
```

This acts as the gateway for the AI features. The controllers are kept extremely "thin"—they do no business logic, delegating everything to `Mediator.Send()`.

---

## 2. Core Application Layer (CQRS with MediatR) & Features Details

The `ThyroCareX.Core` project handles the application's use-cases. It groups logic by **Features** rather than strictly by technical type. Inside each feature, there are `Commands` (for modifying state) and `Queries` (for fetching state).

Here is a comprehensive breakdown of all features inside the application:

### Feature 1: Authentication (`Authentication`)
Handles everything related to user access control and registration.
- **Commands:** 
  - `SignInCommand`: Verifies credentials and generates a JWT Token for secure API access.
  - `RegisterDoctorCommand` & `RegisterPatientCommand`: Registers new user profiles with specific roles in the Identity system.

### Feature 2: AI Diagnostic Tests (`TestWithAI`)
The core value proposition of the system. It receives medical data, passes it to the AI services, and saves the results.
- **Commands:**
  - `PredictImageCommand`: Uploads an ultrasound image, sends it to the AI Image Service, and returns/saves the probability of malignancy.
  - `AssessClinicalCommand`: Receives clinical parameters (TSH, T3, etc.), predicts hyperthyroidism/hypothyroidism using the AI Service.
  - `PredictFnacCommand`: Processes FNAC (Fine Needle Aspiration Cytology) images via AI to provide cellular level predictions.
  - `AddTestCommand`: Aggregates the manual data and predictions into a finalized Test record.

### Feature 3: Community & Social (`Community`)
Allows doctors and patients to interact, ask questions, and share knowledge.
- **Commands:** `AddPostCommand`, `AddCommentCommand`, `LikePostCommand`.
- **Queries:** `GetPostsListQuery`, `GetPostDetailsQuery` (Includes pagination, filtering, and eager-loading comments/likes).

### Feature 4: Subscriptions & Plans (`Plans`)
Manages the tiered subscription levels for users/doctors.
- **Commands:** `AddPlanCommand`, `UpdatePlanCommand`, `DeletePlanCommand`.
- **Queries:** `GetPlanByIdQuery`, `GetAllPlansQuery`.

### Feature 5: Payments (`Payment`)
Integrates with the external payment gateway (Stripe) to process financial transactions for subscriptions.
- **Commands:** `CreateCheckoutSessionCommand` (Initiates payment), `HandleWebhookCommand` (Listens to Stripe events to update subscription status automatically).

### Feature 6: User Profiles (`Doctors` & `Patients` & `AplicationUser`)
Handles the management of profiles, medical history, and personal details.
- **Commands:** `UpdateDoctorProfileCommand`, `UpdatePatientProfileCommand`, `ChangePasswordCommand`.
- **Queries:** `GetDoctorProfileQuery`, `GetPatientMedicalHistoryQuery`.

### Feature 7: Communication (`Contact`)
Manages inquiries or support tickets sent by users.
- **Commands:** `AddContactMessageCommand`.

### Example Code for Core Handler: PredictImageCommand

*(Representational structure of a CQRS Command Model)*
```csharp
namespace ThyroCareX.Core.Feature.TestWithAI.Commands.Models
{
    public class PredictImageCommand : IRequest<Response<DiagnosisResult>>
    {
        public IFormFile UltrasoundImage { get; set; }
        public int PatientId { get; set; }
    }
}
```

---

## 3. Domain Models (Data Layer)

A Model in this architecture is the source of data in the system and defines the database schema using Entity Framework Core. They are located in `ThyroCareX.Data`.

Main Entities:
- `Doctor`, `Patient`: User roles holding specific medical and profile data.
- `Test`, `DiagnosisResult`: Handles the medical tests and AI predictions.
- `Post`, `Comment`, `PostLike`: Entities mapping the community forum aspect.
- `Plan`, `Payment`, `SubscriptionPlan`: Financial and access control entities.

### Model Example: Test Model

```csharp
using System;
using System.ComponentModel.DataAnnotations.Schema;
using ThyroCareX.Data.Enums;

namespace ThyroCareX.Data.Models
{
    public class Test
    {
        public int Id { get; set; }

        // 🧪 Lab Data
        public double? TSH { get; set; }
        public double? T3 { get; set; }
        public double? TT4 { get; set; }
        public double? FTI { get; set; }
        public double? T4U { get; set; }

        public bool NodulePresent { get; set; }
        public int OnThyroxine { get; set; }
        
        // 🖼️ Image
        public string? ImagePath { get; set; }
        public string? FnacImagePath { get; set; }

        // 📊 Status
        public TestStatus Status { get; set; } = TestStatus.Queued;
        
        // 🔗 Navigation
        public DiagnosisResult DiagnosisResult { get; set; }
        
        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public Doctor Doctor { get; set; }
        
        public int PatientId { get;  set; }
        [ForeignKey("PatientId")]
        public Patient? Patient { get;  set; }
    }
}
```

---

## 4. Infrastructure Layer (Repositories & DB Context)

The `ThyroCareX.Infrastructure` contains the `ApplicationDbContext` (Entity Framework mapping) and implements the **Repository Pattern**.

It uses generic repositories (`InfrastructureBases`) and specific ones (`DoctorRepository`, `PatientRepository`, `TestRepo`, `PostRepository`, etc.). This layer also holds the EF Core Migrations and third-party Infrastructure configurations (like **Stripe** for payments).

### Example Code for Generic Repository Abstraction

```csharp
namespace ThyroCareX.Infrastructure.InfrastructureBases
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
```

---

## 5. Services and External Integrations (Service Layer)

The `ThyroCareX.Service` project contains the core business logic and external integrations that are injected into the Handlers.

Key Services:
- **`IAIService`**: Responsible for integrating with the external Python/Flask AI models to process clinical data, ultrasound images, and FNAC images.
- **`IImageService`**: Handles processing, saving, and SkiaSharp compression of user and medical images (converting to WebP).
- **`IPaymentService`**: Manages transactions, subscriptions, and webhooks via the Stripe API.
- **`IAuthentcationService`**: Handles JWT token generation and validation.
- **`IPostService`, `ICommentService`**: Contains logic for the community platform.

### Service Example: Image Service (WebP Compression)

```csharp
using SkiaSharp;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Service.Impelemanation
{
    public class ImageService : IImageService
    {
        private readonly string _baseRootPath;
        
        public ImageService(IWebHostEnvironment env)
        {
            _baseRootPath = Path.Combine(env.WebRootPath ?? "wwwroot", "uploads");
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string originalFileName, string subFolder = "doctors")
        {
            var targetFolder = Path.Combine(_baseRootPath, subFolder);
            var fileName = $"{Guid.NewGuid():N}.webp";
            var filePath = Path.Combine(targetFolder, fileName);

            // Using SkiaSharp to convert images directly to WebP format for optimization
            using var skBitmap = SKBitmap.Decode(fileStream);
            using var image = SKImage.FromBitmap(skBitmap);
            using var data = image.Encode(SKEncodedImageFormat.Webp, 80); // 80% quality compression

            using var output = File.OpenWrite(filePath);
            data.SaveTo(output);

            return Path.Combine("uploads", subFolder, fileName).Replace("\\", "/");
        }
    }
}
```

---

## Conclusion

The ThyroCareX backend uses a highly decoupled **Clean Architecture** combined with **CQRS/MediatR**. By delegating logic across the 5 specific layers:
1. The **API** remains thin and handles routing.
2. The **Core** manages MediatR commands and queries and encapsulates all 7 major features of the application.
3. The **Services** orchestrate complex logic and AI/Stripe connections.
4. The **Infrastructure** abstracts database operations using Repositories.
5. The **Data Models** form a robust foundation for EF Core.

This ensures that the system is scalable, easily maintainable, and well-structured, providing a solid foundation for the AI-powered medical diagnostic features.
