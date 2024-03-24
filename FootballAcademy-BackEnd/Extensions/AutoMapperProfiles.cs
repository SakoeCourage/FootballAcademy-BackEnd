using AutoMapper;
using FootballAcademy_BackEnd.Contracts;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Features.User;
using static FootballAcademy_BackEnd.Features.Player.CreatePlayer;
using static FootballAcademy_BackEnd.Features.Player.UpdatePlayer;
using static FootballAcademy_BackEnd.Features.PlayerMentalAttribute.AddPlayerMentalAttribute;
using static FootballAcademy_BackEnd.Features.PlayerMentalAttribute.UpdatePlayerMentallAttribute;
using static FootballAcademy_BackEnd.Features.PlayerPhysicalAttribute.AddPlayerPhysicalAttribute;
using static FootballAcademy_BackEnd.Features.PlayerPhysicalAttribute.UpdatePlayerPhysicalAttribute;
using static FootballAcademy_BackEnd.Features.PlayerTacticallSkill.AddPlayerTacticalSkill;
using static FootballAcademy_BackEnd.Features.PlayerTacticallSkill.UpdatePlayerTacticallSkill;
using static FootballAcademy_BackEnd.Features.PlayerTechnicalSkill.AddPlayerTechnicalSkill;
using static FootballAcademy_BackEnd.Features.PlayerTechnicalSkill.UpdatePlayerTechnicalSkill;
using static FootballAcademy_BackEnd.Features.User.CreateUser;

namespace FootballAcademy_BackEnd.Extensions
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, LoginResponse>().ReverseMap();

            CreateMap<User, UserLoginResponse>().ReverseMap();

            CreateMap<User, UserWithoutPassword>().ReverseMap();

            CreateMap<User, GetUserResponse>().ReverseMap();

            CreateMap<UpdateUserRequest, UpdateUser.UpdateUserDTO>().ReverseMap();

            CreateMap<GetAuthUserResponse, User>().ReverseMap();

            CreateMap<StaffLoginResponseDTO, Staff>().ReverseMap();

            CreateMap<StaffRequestResponse, Staff>().ReverseMap();

            CreateMap<CreateUserDTO, Staff>().ReverseMap();

            CreateMap<CreatePlayerRequestData, CreatePlayerRequestData>().ReverseMap();

            CreateMap<CreatePlayerRequestData, Player>().ReverseMap();

            CreateMap<UpdatePlayerRequestData, Player>().ReverseMap();

            CreateMap<CreatePlayerRequestData, UpdatePlayerRequestData>().ReverseMap();

            CreateMap<AddPlayerPhysicalAttributeRequestData, PlayerHasPhysicalAttribute>().ReverseMap();

            CreateMap<AddPlayerPhysicalAttributeRequestData, AddPlayerPhysicalAttributeRequest>().ReverseMap();

            CreateMap<UpdatePlayerPhysicalAttributeRequestData, UpdatePlayerPhysicalAttributeRequest>().ReverseMap();

            CreateMap<AddPlayerMentalAttributeRequest, PlayerHasMentalAttribute>().ReverseMap();

            CreateMap<AddPlayerMentalAttributeRequest, AddPlayerMentalAttributeRequestData>().ReverseMap();

            CreateMap<PlayerHasMentalAttribute, AddPlayerMentalAttributeRequestData>().ReverseMap();

            CreateMap<UpdatePlayerMentalAttributeRequestData, UpdatePlayerMentalAttributeRequest>().ReverseMap();

            CreateMap<AddTacticalSkillRequestData, PlayerHasTacticalSkills>().ReverseMap();

            CreateMap<AddTacticalSkillRequestData, AddTacticalSkillRequest>().ReverseMap();

            CreateMap<UpdatePlayerTacticalSkillRequest, UpdatePlayerTacticalSkillRequestData>().ReverseMap();

            CreateMap<AddTechnicalSkillRequest, AddTechnicalSkillRequestData>().ReverseMap();

            CreateMap<PlayerHasTechnicalSkills, AddTechnicalSkillRequestData>().ReverseMap();

            CreateMap<UpdatePlayerTacticalSkillRequestData, UpdatePlayerTacticalSkillRequest>().ReverseMap();

            CreateMap<UpdatePlayerTechnicalSkillRequestData, PlayerHasTechnicalSkills>().ReverseMap();

            CreateMap<UpdatePlayerTechnicalSkillRequestData, UpdatePlayerTechnicalSkillRequest>().ReverseMap();








        }
    }
}
