using DriverSettlements.App.Resources.Localization;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Models;
using Driver = DriverSettlements.Common.Models.Driver;

namespace DriverSettlements.App.Controllers.Application
{
    public class ApplicationProfilesListController : BaseListController<ApplicationProfile>
    {
        private readonly ApplicationController _applicationController;
        public ApplicationProfilesListController(ApplicationController applicationController) : base()
        {
            _applicationController = applicationController;
        }

        public async Task<ApplicationProfile> GetApplicationProfile()
        {
            if (Objects == null)
                Objects = await GetDataFromService();
            var profile = Objects.FirstOrDefault(ap => ap.Default);
            if (profile == null)
                profile = Objects.FirstOrDefault();
            if (profile != null)
                foreach (var tempProfile in Objects)
                    tempProfile.Active = tempProfile.Equals(profile);
            return profile;
        }

        public void SetProfileAsActive(ApplicationProfile profile)
        {
            foreach(var tempProfile in Objects)
                tempProfile.Active = tempProfile.Equals(profile);
        }

        protected async override Task<List<ApplicationProfile>> GetDataFromService()
        {
            var applicationProfiles = new List<ApplicationProfile>();
            var applicationInstancesPath = Path.Combine(FileSystem.Current.AppDataDirectory, "ApplicationInstances");
            if (File.Exists(applicationInstancesPath))
            {
                var instances = await File.ReadAllLinesAsync(applicationInstancesPath);
                if (instances == null || instances.Length == 0)
                {
                    var applicationProfile = new ApplicationProfile()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = ApplicationStrings.ApplicationProfiles_DefaultName
                    };
                    await File.WriteAllTextAsync(applicationInstancesPath, applicationProfile.ToString());
                    applicationProfiles.Add(applicationProfile);
                }
                else
                    applicationProfiles.AddRange(instances.Select(i => new ApplicationProfile(i)));
            }
            else
            {
                var applicationProfile = new ApplicationProfile()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = ApplicationStrings.ApplicationProfiles_DefaultName
                };
                await File.WriteAllTextAsync(applicationInstancesPath, applicationProfile.ToString());
                applicationProfiles.Add(applicationProfile);
            }
            return applicationProfiles;
        }

        protected override async Task<ApplicationProfile> AddObjectToService(ApplicationProfile obj)
        {
            var profile = Objects.FirstOrDefault(ap => ap.Name.Equals(obj.Name, StringComparison.InvariantCultureIgnoreCase));
            if (profile != null)
                throw new Exception(ApplicationStrings.ApplicationProfiles_UniqueProfileNameMessage);
            obj.Id = Guid.NewGuid().ToString();
            Objects.Add(obj);
            await SaveApplicationProfiles();
            Objects.Remove(obj);
            return obj;
        }

        protected override async Task<ApplicationProfile> UpdateObjectInService(ApplicationProfile obj)
        {
            if  (obj.Default)
            {
                foreach (var profile in Objects)
                    profile.Default = profile.Equals(obj);
            }
            await SaveApplicationProfiles();
            return obj;
        }

        protected override async Task<ApplicationProfile> DeleteObjectInService(ApplicationProfile obj)
        {
            if (!Objects.Contains(obj))
                throw new Exception("aaa");
            Objects.Remove(obj);
            await SaveApplicationProfiles();
            Objects.Add(obj);
            return obj;
        }

        protected override List<ApplicationProfile> FilterData(string filerString)
        {
            return Objects;
        }

        private async Task SaveApplicationProfiles()
        {
            var applicationInstancesPath = Path.Combine(FileSystem.Current.AppDataDirectory, "ApplicationInstances");
            var lines = Objects.Select(ap => ap.ToString()).ToList();
            await File.WriteAllLinesAsync(applicationInstancesPath, lines);
        }
    }
}
