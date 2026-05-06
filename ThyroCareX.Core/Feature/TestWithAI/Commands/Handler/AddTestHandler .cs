//using MediatR;
//using System;
//using System.Threading.Tasks;
//using ThyroCareX.Core.Bases;
//using ThyroCareX.Core.Feature.TestWithAI.Commands.Models;
//using ThyroCareX.Data.Models;
//using ThyroCareX.Service.Abstarct;
//using ThyroCareX.Core.Feature.TestWithAI.Dto;
//using ThyroCareX.Core.Dto.FnacAIResponse;
//using ThyroCareX.Data.Healpers.ClinicalAIResponse;
//using ThyroCareX.Core.Dto.ImageAIResponse;

//namespace ThyroCareX.Core.Feature.TestWithAI.Commands.Handler
//{
//    public class AddTestHandler( IDoctorService _doctorService,
//        IUserContextService _userContextService, ITestService _testService,
//        IMediator _mediator)
//        : ResponseHandler, IRequestHandler<AddTestCommand, Response<AddTestResponse>>
//    {
//        public async Task<Response<AddTestResponse>> Handle(AddTestCommand request, CancellationToken cancellationToken)
//        {
//            var userIdString = _userContextService.UserId;
//            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
//                return Unauthorized<AddTestResponse>("Unauthorized");

//            var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
//            //var imagePath = await _imageService.UploadFileAsync(request.Image);
//            //string? fnacImagePath = null;
//            //if (request.FnacImage != null)
//            //{
//            //    fnacImagePath = await _imageService.UploadFileAsync(request.FnacImage);
//            //}

//            var test = new Test
//            {
//                PatientId = request.PatientId,
//                DoctorId = doctor.DoctorID,
//                //ImagePath = imagePath,
//                //FnacImagePath = fnacImagePath,
//                TSH = request.TSH,
//                T3 = request.T3,
//                TT4 = request.TT4,
//                FTI = request.FTI,
//                T4U = request.T4U,
//                NodulePresent = request.NodulePresent,
//                OnThyroxine = request.OnThyroxine,
//                ThyroidSurgery = request.ThyroidSurgery,
//                QueryHyperthyroid = request.QueryHyperthyroid
//            };

//            await _testService.AddTestAsync(test);

//            // Process AI
//            //Response<ImageAIResponse>? imageResponse = null;
//            //try { imageResponse = await _mediator.Send(new PredictImageCommand { TestId = test.Id }); } catch { }

//            //Response<ClinicalAIResponse>? clinicalResponse = null;
//            //try { clinicalResponse = await _mediator.Send(new AssessClinicalCommand { TestId = test.Id }); } catch { }

//            //Response<FnacAIResponse>? fnacResponse = null;
//            //if (!string.IsNullOrEmpty(test.FnacImagePath))
//            //{
//            //    try { fnacResponse = await _mediator.Send(new PredictFnacCommand { TestId = test.Id }); } catch { }
//            //}

//            return Success(new AddTestResponse
//            {
//                TestId = test.Id,
//                Message = "Test added successfully.",
//                //ImageResult = imageResponse?.Data,
//                ClinicalResult = clinicalResponse?.Data,
//                //FnacResult = fnacResponse?.Data
//            });
//        }
//    }
//}
