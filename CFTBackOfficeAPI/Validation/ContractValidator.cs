using CFTBackOfficeAPI.Models;
using CFTBackOfficeAPI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using ECXSecurity;

namespace CFTBackOfficeAPI.Validation
{
    public class ContractValidator : IContractValidator
    {
        private IContract _ic;
        private readonly IECXSecurityService _securityService;

        public ContractValidator(IContract c, IECXSecurityService securityService)
        {
            _ic = c;
            _securityService = securityService;
        }
        string ValidateOptionHasMargin(Guid cftId)
        {
            DataTable hasMargin = _ic.ContractHasMargin(cftId);
            if (hasMargin.Rows.Count > 0)
            {
                return "Valid";
            }
            return "InValid";
        }

        public async Task<string> SaveMargin(Margin margin, Guid createdBy)
        {
            ECXSecurityAccessSoapClient client = _securityService.GetService();

            HasRightResponse response = new();

            if (margin != null)
            {
                if (margin.Side.Equals("2"))//Seller margin
                {//SellMarginMaker
                    response = await client.HasRightAsync(createdBy.ToString(), "SellMarginMaker", "");
                    if (response.Body.HasRightResult == 2)
                    {
                        return "User has no right to save margin data";
                    }
                }
                else if (margin.Side.Equals("1"))//Buyer Marign 
                {//BuyMarginMaker
                    response = await client.HasRightAsync(createdBy.ToString(), "BuyMarginMaker", "");
                    if (response.Body.HasRightResult == 2)
                    {
                        return "User has no right to save margin data";
                    }
                    if (margin.MarginId != 1)
                    {
                        return @"Buyer margin can't be in Commodity";
                    }
                }
                if (margin.MarginId == 0)
                {
                    return @"Margin should be either in comodity or cash";
                }
                if (margin.MarginId == 1)
                {
                    if (margin.AccountNumber == Guid.Empty)
                    {
                        return @"Account number can not be empty";
                    }
                }
                else if (margin.MarginId == 2)
                {
                    if (margin.QuantityInLot == 0)
                    {
                        return @"Quantity can not be Zero";
                    }
                }
                if (margin.Amount == 0)
                {
                    return @"Amount can not be Zero";
                }

                bool traderFound = false;
                DataTable dtTable = _ic.GetContractByCFTID(margin.CFTId);
                if (dtTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtTable.Rows)
                    {
                        if (new Guid(dr["CFTBuyId"].ToString()) == margin.CFTTraderId ||
                            new Guid(dr["CFTSellId"].ToString()) == margin.CFTTraderId)
                        {
                            traderFound = true;
                        }
                    }
                    if (!traderFound)
                        return "No contract data is registered to be used for margin ";

                    bool allowSave = false;
                    if (ValidateOptionHasMargin(margin.CFTId) == "Valid")
                    {
                        DataTable dt = _ic.GetMargin(margin.CFTId);
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (new Guid(Convert.ToString(dr["CFTTraderId"])) == margin.CFTTraderId)
                                {
                                    return "Margin has been registered for the selected contract";
                                }
                                if (Convert.ToBoolean(dr["IsSeller"]) && margin.CFTTraderId != new Guid(Convert.ToString(dr["CFTTraderId"])))
                                {
                                    allowSave = true;
                                }
                            }
                        }
                        else
                        {//Check if logged in user is from seller side 
                            if (margin.Side == "2")//is Seller
                            {
                                allowSave = true;
                            }
                            else
                            {
                                return "Seller side marign should be registered first";
                            }
                        }
                        if (allowSave)
                        {
                            int result = _ic.SaveMargin(margin, createdBy);
                            if (result == 1)
                            {
                                return "Ok";
                            }
                            else if (result == 2 || result == 6)
                            {
                                return "Margin exists by the selected contract trader";
                            }
                            else if (result == 3)
                            {
                                return "wrong margin type";
                            }
                            else if (result == 4)
                            {
                                return "The bank account doesn't have enough balance";
                            }
                            else if (result == 5)
                            {
                                return "Active margin exists with another account number";
                            }
                            else
                            {
                                return "Error while saving margin data for contract";
                            }
                        }
                    }
                    else
                        return "Contract has no option for margin registration";
                }
            }
            return "margin data(model) is null";
        }
        public async Task<string> ApproveMargin(Guid cftId, Guid cftTraderId, Guid approvedBy)
        {
            DataTable traderMargin = _ic.GetMarginByTrader(cftTraderId);
            if (traderMargin.Rows.Count > 0)
            {
                if (new Guid(traderMargin.Rows[0]["MakerId"].ToString()) == approvedBy)
                {
                    return "Maker and Checker Should not be the Same";
                }
                if (Convert.ToInt16(traderMargin.Rows[0]["Status"]) == 2)
                {
                    return "Margin is already approved";
                }
                if (Convert.ToInt16(traderMargin.Rows[0]["Status"]) == 3)
                {
                    return "Released margin can not be approved";
                }
                ECXSecurityAccessSoapClient client = _securityService.GetService();
                HasRightResponse response = new();

                if (Convert.ToBoolean(traderMargin.Rows[0]["IsSeller"]))
                {
                    //check approver has cd role from security manager
                    response = await client.HasRightAsync(approvedBy.ToString(), "SellMarginChecker", "");
                    if (response.Body.HasRightResult == 2)
                    {
                        return "You don't have Access to Approve the Margin";
                    }
                }
                else
                {
                    response = await client.HasRightAsync(approvedBy.ToString(), "BuyMarginChecker", "");
                    if (response.Body.HasRightResult == 2)
                    {
                        return "You don't have Access to Approve the Margin";
                    }
                }
                DataTable margin = _ic.GetMargin(cftId);
                if (margin.Rows.Count == 2)
                {
                    if (Math.Round(Convert.ToDecimal(margin.Rows[0]["Amount"]), 2) == Math.Round(Convert.ToDecimal(margin.Rows[1]["Amount"]), 2))
                    {
                        if (_ic.ApproveReleaseMargin(new Guid(traderMargin.Rows[0]["ID"].ToString()), 2, approvedBy))
                        {
                            return "Ok";
                        }
                        else
                            return "Error while Approving the Margin";
                    }
                    else
                    {
                        return "Counter party Margin Amount should be equal";
                    }
                }
                else
                {
                    return "Counter party Margin should be Register";
                }

            }
            return "Margin Not Exist";
        }
    }
}
