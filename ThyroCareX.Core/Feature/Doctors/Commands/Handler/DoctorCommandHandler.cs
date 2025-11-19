using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Feature.Doctors.Commands.Models;
using ThyroCareX.Core.Feature.Doctors.Queires.Result;
using ThyroCareX.Data.Models;
using ThyroCareX.Service.Abstarct;
using ThyroCareX.Service.Impelemanation;

namespace ThyroCareX.Core.Feature.Doctors.Commands.Handler
{
    public class DoctorCommandHandler:ResponseHandler,
                                                 IRequestHandler<EditDoctorCommand,Response<string>>,
                                                 IRequestHandler<DeleteDoctorCommand,Response<string>>,
                                                 IRequestHandler<AddDoctorCommand,Response<string>>
    {
        #region prop
        private readonly IDoctorService _doctorService;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        #endregion
        #region Constructor
        public DoctorCommandHandler(IDoctorService doctorService, IMapper mapper, IImageService imageService)
        {
            _doctorService = doctorService;
                        _mapper = mapper;
            _imageService = imageService;

        }

        #endregion
        #region Handle Functions
        public async Task<Response<string>> Handle(EditDoctorCommand request, CancellationToken cancellationToken)
        {
            //check if doctor exists 
        var doctor = await _doctorService.GetDoctorByIdAsync(request.Id);
            if(doctor == null)
            {
                return NotFound<string>("Doctor Is Not Found");
            }

            // 2️⃣ رفع الصورة الجديدة لو موجودة
            if (!string.IsNullOrEmpty(request.ImageFile))
            {
                // حفظ المسار القديم
                var oldImagePath = doctor.ImagePath;

                // تحويل Base64 string إلى Stream
                byte[] imageBytes = Convert.FromBase64String(request.ImageFile);
                using var stream = new MemoryStream(imageBytes);

                // رفع الصورة الجديدة عبر ImageService
                var newImagePath = await _imageService.UploadImageAsync(stream, $"doctor_{doctor.DoctorID}.webp");
                doctor.ImagePath = newImagePath;

                // حذف الصورة القديمة لو موجودة
                if (!string.IsNullOrEmpty(oldImagePath))
                    _imageService.DeleteImage(oldImagePath);
            }
            //mapping Between request and Doctor

            var mappedDoctor = _mapper.Map<Doctor>(request);
            //call srevice that make Edit 
            var result = await _doctorService.EditAsync(mappedDoctor);
            //return response
            if (result == "Success") return Success($"Updated Seccessfully {mappedDoctor.DoctorID}");
            else return BadRequest<string>();
        }

        public async Task<Response<string>> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
        {
            //check if the Id is Exist or Not
            var doctor= await _doctorService.GetDoctorByIdAsync(request.Id);
            //return NotFound
            if(doctor == null)
            {
                return NotFound<string>("Doctor Is Not Found");
            }
            //call service that make delete
            var result = await _doctorService.DeleteAsync(doctor);
            //return response
            if(result == "Success") return Deleted<string>($"Doctor {doctor.FullName} deleted successfully");
            else return BadRequest<string>();

        }

        public async Task<Response<string>> Handle(AddDoctorCommand request, CancellationToken cancellationToken)
        {
            var doctorMapped = _mapper.Map<Doctor>(request);
            var result = await _doctorService.AddAsync(doctorMapped);
            // return response
            if (result == "Success") return Created("");
            else return BadRequest<string>();
        }


        #endregion
    }
}

