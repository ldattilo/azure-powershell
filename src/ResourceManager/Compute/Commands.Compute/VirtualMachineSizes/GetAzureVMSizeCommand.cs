﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using AutoMapper;
using Microsoft.Azure.Commands.Compute.Common;
using Microsoft.Azure.Commands.Compute.Models;
using Microsoft.Azure.Management.Compute;
using Microsoft.Azure.Management.Compute.Models;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.Compute
{
    [Cmdlet(VerbsCommon.Get, ProfileNouns.VirtualMachineSize, DefaultParameterSetName = ListVirtualMachineSizeParamSet)]
    [OutputType(typeof(PSVirtualMachineSize))]
    public class GetAzureVMSizeCommand : VirtualMachineSizeBaseCmdlet
    {
        protected const string ListVirtualMachineSizeParamSet = "ListVirtualMachineSizeParamSet";
        protected const string ListAvailableSizesForAvailabilitySet = "ListAvailableSizesForAvailabilitySet";
        protected const string ListAvailableSizesForVirtualMachine = "ListAvailableSizesForVirtualMachine";

        [Parameter(
           Mandatory = true,
           Position = 0,
           ParameterSetName = ListVirtualMachineSizeParamSet,
           ValueFromPipelineByPropertyName = true,
           HelpMessage = "The location name.")]
        [ValidateNotNullOrEmpty]
        public string Location { get; set; }

        [Parameter(
           Mandatory = true,
           Position = 0,
           ParameterSetName = ListAvailableSizesForAvailabilitySet,
           ValueFromPipelineByPropertyName = true,
           HelpMessage = "The resource group name.")]
        [Parameter(
           Mandatory = true,
           Position = 0,
           ParameterSetName = ListAvailableSizesForVirtualMachine,
           ValueFromPipelineByPropertyName = true,
           HelpMessage = "The resource group name.")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1,
            ParameterSetName = ListAvailableSizesForAvailabilitySet,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The availability set name.")]
        [ValidateNotNullOrEmpty]
        public string AvailabilitySetName { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1,
            ParameterSetName = ListAvailableSizesForVirtualMachine,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The virtual machine name.")]
        [ValidateNotNullOrEmpty]
        public string VMName { get; set; }

        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();

            VirtualMachineSizeListResponse result = null;

            if (!string.IsNullOrEmpty(this.VMName))
            {
                result = this.VirtualMachineClient.ListAvailableSizes(
                    this.ResourceGroupName,
                    this.VMName);
            }
            else if (!string.IsNullOrEmpty(this.AvailabilitySetName))
            {
                result = this.AvailabilitySetClient.ListAvailableSizes(
                    this.ResourceGroupName,
                    this.AvailabilitySetName);
            }
            else
            {
                result = this.VirtualMachineSizeClient.List(this.Location.Canonicalize());
            }

            List<PSVirtualMachineSize> psResultList = new List<PSVirtualMachineSize>();
            foreach (var item in result.VirtualMachineSizes)
            {
                var psItem = Mapper.Map<VirtualMachineSize, PSVirtualMachineSize>(item);
                psItem = Mapper.Map<AzureOperationResponse, PSVirtualMachineSize>(result, psItem);
                psResultList.Add(psItem);
            }

            WriteObject(psResultList, true);
        }
    }
}
