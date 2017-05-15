using System;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using WorldofWords.Utils;
using WorldOfWords.API.Models;
using WorldOfWords.API.Models.IMappers;
using WorldOfWords.API.Models.Mappers;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services;
using WorldOfWords.Domain.Services.IServices;
using WorldOfWords.Domain.Services.Services;
using WorldOfWords.Validation;
using WorldOfWords.Validation.Classes;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using WorldofWords.Hubs;



namespace WorldofWords.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            //container.LoadConfiguration();
            // TODO: Register your types here
            container.RegisterType<IRoleMapper, RoleMapper>();
            container.RegisterType<IUserLoginMapper, UserLoginMapper>();
            container.RegisterType<IUserMapper, UserMapper>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<ITokenValidation, TokenValidation>();
            container.RegisterType<ICourseMapper, CourseMapper>();
            container.RegisterType<ICourseService, CourseService>();
            container.RegisterType<IWordSuiteMapper, WordSuiteMapper>();
            container.RegisterType<IGroupForListingMapper, GroupForListingMapper>();
            container.RegisterType<ITrainingWordSuiteMapper, TrainingWordSuiteMapper>();
            container.RegisterType<IWordSuiteService, WordSuiteService>();
            container.RegisterType<ITrainingWordTranslationMapper, TrainingWordTranslationMapper>();
            container.RegisterType<IQuizWordSuiteMapper, QuizWordSuiteMapper>();
            container.RegisterType<IQuizWordTranslationMapper, QuizWordTranslationMapper>();
            container.RegisterType<ILanguageService, LanguageService>();
            container.RegisterType<ILanguageMapper, LanguageMapper>();
            container.RegisterType<ICourseForGroupMapper, CourseForGroupMapper>();
            container.RegisterType<IGroupService, GroupService>();
            container.RegisterType<IGroupMapper, GroupMapper>();
            container.RegisterType<IWordMapper, WordMapper>();
            container.RegisterType<IWordService, WordService>();
            container.RegisterType<IPdfGenerator<WordSuite>, WordSuitePdfGenerator>();
            container.RegisterType<IWordTranslationService, WordTranslationService>();
            container.RegisterType<IWordTranslationMapper, WordTranslationMapper>();
            container.RegisterType<IWordProgressMapper, WordProgressMapper>();
            container.RegisterType<IWordProgressService, WordProgressService>();
            container.RegisterType<IIncomingUserMapper, IncomingUserMapper>();
            container.RegisterType<IIdentityMessageService, EmailService>();
            container.RegisterType<IEnrollmentMapper, EnrollmentMapper>();
            container.RegisterType<IEnrollmentService, EnrollmentService>();
            container.RegisterType<IUserToTokenMapper, UserToTokenMapper>();
            container.RegisterType<IUserForListingMapper, UserForListinigMapper>();
            container.RegisterType<IUserForListOfUsersMapper, UserForListOfUsersMapper>();
            container.RegisterType<IRequestIdentityService, RequestIdentityService>();
            container.RegisterType<ITagService, TagService>();
            container.RegisterType<ITagMapper, TagMapper>();
            container.RegisterType<IUnitOfWorkFactory, UnitOfWorkFactory>();
            container.RegisterType<ITicketService, TicketService>();
            container.RegisterType<ITicketMapper, TicketMapper>();
            container.RegisterType<IWordManagingService, WordManagingService>();
            container.RegisterType<IPictureMapper, PictureMapper>();
            container.RegisterType<IPictureService, PictureService>();
            container.RegisterType<IRecordMapper, RecordMapper>();
            container.RegisterType<IRecordService, RecordService>();
            container.RegisterType<IPartOfSpeechMapper, PartOfSpeechMapper>(); 
            container.RegisterType<IQuizMapper, QuizMapper>();
            container.RegisterType<ConnectedUsersContainer>(new InjectionFactory(context => ConnectedUsersContainer.Container));
            container.RegisterType<TicketNotificationHub>(new InjectionFactory(context => new TicketNotificationHub(container.Resolve<IUserService>(), container.Resolve<ICourseService>(), container.Resolve<ConnectedUsersContainer>())));
        }

    }
}
