using DriverSettlements.App.Controllers;
using DriverSettlements.App.Controllers.MasterAdmin;
using DriverSettlements.App.Resources.Localization;
using DriverSettlements.App.Services;
using DriverSettlements.Common;
using DriverSettlements.Common.DataContracts;
using DriverSettlements.Common.DataContracts.ApplicationInstance;
using DriverSettlements.Common.DataContracts.MasterAdmin;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.App
{
    public class MasterAdminController
    {
        public readonly TenantsListController TenantsListController;
        private readonly IMasterAdminAsync _masterAdminService;
        private readonly ApplicationController _applicationController;

        public MasterAdminController(ApplicationController applicationController)
        {
            TenantsListController = new TenantsListController(applicationController);
            _applicationController = applicationController; 
            _masterAdminService = ServiceHelper.GetService<IMasterAdminAsync>();
        }

        public async Task<bool> AddDefaultReportDefinitions()
        {
            var response = await _masterAdminService.AddDefaultReportDefinitions(new AddDefaultReportDefinitionsRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return true;
        }

        public async Task<bool> AddSettlementTypes()
        {
            var response = await _masterAdminService.AddSettlementTypes(new AddSettlementTypesRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return true;
        }

        public void ClearControllers()
        {
            TenantsListController.ClearController();
        }
    }
}
