using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;

namespace QLVPP.Mappings
{
    public class ApprovalStepMappingProfile : Profile
    {
        public ApprovalStepMappingProfile()
        {
            CreateMap<ApprovalStepReq, ApprovalStep>();
            CreateMap<ApprovalStep, ApprovalStepRes>();
        }
    }
}
