using CFTBackOfficeAPI.Models;
using CFTBackOfficeAPI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using CFTBackOfficeAPI.DataAccess;
using System.Data.SqlClient;

namespace CFTBackOfficeAPI.Repository
{
    public class ContractRepository : IContract
    {
        private readonly IDataAccessProvider _db;
        private IConfiguration _cs;
        // private IWebHostEnvironment _environment;
        // private readonly IOptions<MyConfiguration> config;
        public ContractRepository(IDataAccessProvider db, IConfiguration cs)//, IWebHostEnvironment _env)
        {
            _cs = cs;
            _db = db;
            // _environment = _env;
        }
        public DataTable GetAllContract()
        {
            var erroMesg = "";
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "sp_CFTGetAllContractSummary", ref erroMesg);

        }

        public List<ContractDetail> GetContractById(Guid Id)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("Id");
            paramValue.Add(Id);
            DataTable data = _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "sp_CFTGetContractById", paramName, paramValue, ref erroMesg);
            return _db.ConvertDataTable<ContractDetail>(data);
        }

        public string Save(ContractDTO _contract, Guid CreatedBy)
        {
            string result = ValidateContractInput(_contract);
            if (result == "OK")
            {
                result = ValidateContract(_contract);
                if (result == "OK")
                {
                    var erroMesg = "";
                    Contract ct = _contract.Contract;
                    Guid sellerId = new Guid();
                    Guid BuyerId = new Guid();
                    string fileName = "";

                    if (_contract.Contract.Attachement != null)
                    {

                        fileName = new String(Path.GetFileNameWithoutExtension(_contract.Contract.Attachement.FileName).ToArray()).Replace(' ', '-');
                        fileName = fileName + DateTime.Now.ToString("yyMMddhhmmss") + Path.GetExtension(_contract.Contract.Attachement.FileName);

                        string filePath = Path.Combine(_cs["AttachementDirectory"], fileName);
                        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            _contract.Contract.Attachement.CopyTo(fileStream);
                        }
                    }
                    List<ContractTrader> trader = new List<ContractTrader>();
                    trader.Add(_contract.Buyer);
                    trader.Add(_contract.Seller);
                    if (trader.Count == 2)
                    {
                        SqlCommand cmdSeller = new SqlCommand();
                        SqlCommand cmdBuyer = new SqlCommand();


                        foreach (ContractTrader tr in trader)
                        {

                            if (tr.IsSeller)
                            {

                                sellerId = Guid.NewGuid();
                                SqlConnection conSell = new SqlConnection(_cs["ConnectionStrings:membership"]);
                                cmdSeller.CommandText = "dbo.sp_CFTTraderSave";
                                cmdSeller.CommandType = CommandType.StoredProcedure;
                                cmdSeller.Parameters.AddWithValue("@ClientId", tr.ClientId);
                                cmdSeller.Parameters.AddWithValue("@ClientName", tr.ClientName);
                                cmdSeller.Parameters.AddWithValue("@ID", sellerId);
                                cmdSeller.Parameters.AddWithValue("@HouseNo", tr.HouseNo);
                                cmdSeller.Parameters.AddWithValue("@IsSeller", tr.IsSeller);
                                cmdSeller.Parameters.AddWithValue("@MemberId", tr.MemberId);
                                cmdSeller.Parameters.AddWithValue("@MemberName", tr.MemberName);
                                cmdSeller.Parameters.AddWithValue("@Phone", tr.Phone);
                                cmdSeller.Parameters.AddWithValue("@Region", tr.Region);
                                cmdSeller.Parameters.AddWithValue("@TINNumber", tr.TINNumber);
                                cmdSeller.Parameters.AddWithValue("@VATNumber", tr.VATNumber);
                                cmdSeller.Parameters.AddWithValue("@Woreda", tr.Woreda);
                                cmdSeller.Parameters.AddWithValue("@Zone", tr.Zone);
                                cmdSeller.Parameters.AddWithValue("@CreatedBy", CreatedBy);

                                cmdSeller.Connection = conSell;
                            }
                            else
                            {

                                BuyerId = Guid.NewGuid();
                                SqlConnection conBuy = new SqlConnection(_cs["ConnectionStrings:membership"]);
                                cmdBuyer.CommandText = "dbo.sp_CFTTraderSave";
                                cmdBuyer.CommandType = CommandType.StoredProcedure;
                                cmdBuyer.Parameters.AddWithValue("@ClientId", tr.ClientId);
                                cmdBuyer.Parameters.AddWithValue("@ClientName", tr.ClientName);
                                cmdBuyer.Parameters.AddWithValue("@ID", BuyerId);
                                cmdBuyer.Parameters.AddWithValue("@HouseNo", tr.HouseNo);
                                cmdBuyer.Parameters.AddWithValue("@IsSeller", tr.IsSeller);
                                cmdBuyer.Parameters.AddWithValue("@MemberId", tr.MemberId);
                                cmdBuyer.Parameters.AddWithValue("@MemberName", tr.MemberName);
                                cmdBuyer.Parameters.AddWithValue("@Phone", tr.Phone);
                                cmdBuyer.Parameters.AddWithValue("@Region", tr.Region);
                                cmdBuyer.Parameters.AddWithValue("@TINNumber", tr.TINNumber);
                                cmdBuyer.Parameters.AddWithValue("@VATNumber", tr.VATNumber);
                                cmdBuyer.Parameters.AddWithValue("@Woreda", tr.Woreda);
                                cmdBuyer.Parameters.AddWithValue("@Zone", tr.Zone);
                                cmdBuyer.Parameters.AddWithValue("@CreatedBy", CreatedBy);

                                cmdBuyer.Connection = conBuy;
                            }
                        }
                        SqlCommand cmdContract = new SqlCommand();
                        SqlConnection con = new SqlConnection(_cs["ConnectionStrings:membership"]);
                        cmdContract.CommandText = "dbo.sp_CFTContractSave";
                        cmdContract.CommandType = CommandType.StoredProcedure;
                        cmdContract.Parameters.AddWithValue("@Attachement", fileName);
                        cmdContract.Parameters.AddWithValue("@CFTBuyId", BuyerId);
                        cmdContract.Parameters.AddWithValue("@CFTSellId", sellerId);
                        cmdContract.Parameters.AddWithValue("@CommodityId", ct.CommodityId);
                        cmdContract.Parameters.AddWithValue("@CommodityClassId", ct.CommodityClassId);
                        cmdContract.Parameters.AddWithValue("@ContractDate", ct.ContractDate);
                        cmdContract.Parameters.AddWithValue("@ECXWarehouseId", ct.ECXWarehouseId);
                        cmdContract.Parameters.AddWithValue("@MaturityDate", ct.MaturityDate);
                        cmdContract.Parameters.AddWithValue("@MakerId", CreatedBy);
                        cmdContract.Parameters.AddWithValue("@OptionId", ct.OptionId);
                        cmdContract.Parameters.AddWithValue("@ProductionYear", ct.ProductionYear);
                        cmdContract.Parameters.AddWithValue("@Price", ct.Price);
                        cmdContract.Parameters.AddWithValue("@QuantityInLot", ct.QuantityInLot);
                        cmdContract.Parameters.AddWithValue("@QuantityNetWeight", ct.QuantityNetWeight);
                        cmdContract.Parameters.AddWithValue("@Symbol", ct.Symbol);
                        cmdContract.Parameters.AddWithValue("@TraderWarehouse", ct.TraderWarehouse == null ? "" : ct.TraderWarehouse);

                        cmdContract.Connection = con;
                        result = _db.ExecuteSqlTransaction(cmdContract, cmdBuyer, cmdSeller);
                    }
                    /*   foreach (ContractTrader tr in trader)
                       {
                           Guid id = Guid.NewGuid();
                           ArrayList trparamName = new ArrayList();
                           ArrayList trparamValue = new ArrayList();

                           trparamName.Add("ClientId");
                           trparamName.Add("ClientName");
                           trparamName.Add("ID");
                           trparamName.Add("HouseNo");
                           trparamName.Add("IsSeller");
                           trparamName.Add("MemberId");
                           trparamName.Add("MemberName");
                           trparamName.Add("Phone");
                           trparamName.Add("Region");
                           trparamName.Add("TINNumber");
                           trparamName.Add("VATNumber");
                           trparamName.Add("Woreda");
                           trparamName.Add("Zone");
                           trparamName.Add("CreatedBy");
                           trparamName.Add("MarginId");

                           if (tr.IsSeller)
                               sellerId = id;
                           else
                               BuyerId = id;
                           trparamValue.Add(tr.ClientId);
                           trparamValue.Add(tr.ClientName);
                           trparamValue.Add(id);
                           trparamValue.Add(tr.HouseNo);
                           trparamValue.Add(tr.IsSeller);
                           trparamValue.Add(tr.MemberId);
                           trparamValue.Add(tr.MemberName);
                           trparamValue.Add(tr.Phone);
                           trparamValue.Add(tr.Region);
                           trparamValue.Add(tr.TINNumber);
                           trparamValue.Add(tr.VATNumber);
                           trparamValue.Add(tr.Woreda);
                           trparamValue.Add(tr.Zone);
                           trparamValue.Add(CreatedBy);
                           trparamValue.Add(tr.IsSeller ? ct.SellerMargin : ct.BuyerMargin);
                           _db.ExecuteNonQuery(_cs["ConnectionStrings:membership"], "dbo", "sp_CFTTraderSave", trparamName, trparamValue, ref erroMesg);
                       }
                       ArrayList paramName = new ArrayList();
                       paramName.Add("Attachement");
                       paramName.Add("CFTBuyId");
                       paramName.Add("CFTSellId");
                       paramName.Add("CommodityId");
                       paramName.Add("CommodityClassId");
                       paramName.Add("ContractDate");
                       paramName.Add("ECXWarehouseId");
                       paramName.Add("MakerId");
                       paramName.Add("MaturityDate");
                       paramName.Add("OptionId");
                       paramName.Add("Price");
                       paramName.Add("ProductionYear");
                       paramName.Add("QuantityInLot");
                       paramName.Add("QuantityNetWeight");
                       paramName.Add("StatusId");
                       paramName.Add("Symbol");
                       paramName.Add("TraderWarehouse");


                       ArrayList paramValue = new ArrayList();
                       paramValue.Add(fileName);
                       paramValue.Add(BuyerId);
                       paramValue.Add(sellerId);
                       paramValue.Add(ct.CommodityId);
                       paramValue.Add(ct.CommodityClassId);
                       paramValue.Add(ct.ContractDate);
                       paramValue.Add(ct.ECXWarehouseId);
                       paramValue.Add(CreatedBy);
                       paramValue.Add(ct.MaturityDate);
                       paramValue.Add(ct.OptionId);
                       paramValue.Add(ct.Price);
                       paramValue.Add(ct.ProductionYear);
                       paramValue.Add(ct.QuantityInLot);
                       paramValue.Add(ct.QuantityNetWeight);
                       paramValue.Add(ct.StatusId);
                       paramValue.Add(ct.Symbol);
                       paramValue.Add(ct.TraderWarehouse);

                       _db.ExecuteNonQuery(_cs["ConnectionStrings:membership"], "dbo", "sp_CFTContractSave", paramName, paramValue, ref erroMesg);
                       */
                }
            }
            return result;
        }

        public string Update(ContractDTO _contract, Guid updatedBy)
        {
            string result = ValidateContractInput(_contract);
            if (result == "OK")
            {
                result = ValidateContract(_contract);
                if ("OK" == "OK")
                {
                    Contract ct = _contract.Contract;
                    string fileName = "";

                    if (_contract.Contract.Attachement != null)
                    {

                        fileName = new String(Path.GetFileNameWithoutExtension(_contract.Contract.Attachement.FileName).ToArray()).Replace(' ', '-');
                        fileName = fileName + DateTime.Now.ToString("yyMMddhhmmss") + Path.GetExtension(_contract.Contract.Attachement.FileName);

                        string filePath = Path.Combine(_cs["AttachementDirectory"], fileName);
                        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            _contract.Contract.Attachement.CopyTo(fileStream);
                        }
                    }
                    List<ContractTrader> trader = new List<ContractTrader>();
                    trader.Add(_contract.Buyer);
                    trader.Add(_contract.Seller);

                    if (trader.Count == 2)
                    {
                        SqlCommand cmdSeller = new SqlCommand();
                        SqlCommand cmdBuyer = new SqlCommand();
                        foreach (ContractTrader tr in trader)
                        {

                            if (tr.IsSeller)
                            {

                                SqlConnection conSell = new SqlConnection(_cs["ConnectionStrings:membership"]);
                                cmdSeller.CommandText = "dbo.sp_CFTTraderEdit";
                                cmdSeller.CommandType = CommandType.StoredProcedure;
                                cmdSeller.Parameters.AddWithValue("@ClientId", tr.ClientId);
                                cmdSeller.Parameters.AddWithValue("@ClientName", tr.ClientName);
                                cmdSeller.Parameters.AddWithValue("@ID", tr.ID);
                                cmdSeller.Parameters.AddWithValue("@HouseNo", tr.HouseNo);
                                cmdSeller.Parameters.AddWithValue("@MemberId", tr.MemberId);
                                cmdSeller.Parameters.AddWithValue("@MemberName", tr.MemberName);
                                cmdSeller.Parameters.AddWithValue("@Phone", tr.Phone);
                                cmdSeller.Parameters.AddWithValue("@Region", tr.Region);
                                cmdSeller.Parameters.AddWithValue("@TINNumber", tr.TINNumber);
                                cmdSeller.Parameters.AddWithValue("@VATNumber", tr.VATNumber);
                                cmdSeller.Parameters.AddWithValue("@Woreda", tr.Woreda);
                                cmdSeller.Parameters.AddWithValue("@Zone", tr.Zone);
                                cmdSeller.Parameters.AddWithValue("@UpdatedBy", updatedBy);

                                cmdSeller.Connection = conSell;
                            }
                            else
                            {


                                SqlConnection conBuy = new SqlConnection(_cs["ConnectionStrings:membership"]);
                                cmdBuyer.CommandText = "dbo.sp_CFTTraderEdit";
                                cmdBuyer.CommandType = CommandType.StoredProcedure;
                                cmdBuyer.Parameters.AddWithValue("@ClientId", tr.ClientId);
                                cmdBuyer.Parameters.AddWithValue("@ClientName", tr.ClientName);
                                cmdBuyer.Parameters.AddWithValue("@ID", tr.ID);
                                cmdBuyer.Parameters.AddWithValue("@HouseNo", tr.HouseNo);
                                cmdBuyer.Parameters.AddWithValue("@MemberId", tr.MemberId);
                                cmdBuyer.Parameters.AddWithValue("@MemberName", tr.MemberName);
                                cmdBuyer.Parameters.AddWithValue("@Phone", tr.Phone);
                                cmdBuyer.Parameters.AddWithValue("@Region", tr.Region);
                                cmdBuyer.Parameters.AddWithValue("@TINNumber", tr.TINNumber);
                                cmdBuyer.Parameters.AddWithValue("@VATNumber", tr.VATNumber);
                                cmdBuyer.Parameters.AddWithValue("@Woreda", tr.Woreda);
                                cmdBuyer.Parameters.AddWithValue("@Zone", tr.Zone);
                                cmdBuyer.Parameters.AddWithValue("@UpdatedBy", updatedBy);

                                cmdBuyer.Connection = conBuy;
                            }
                        }

                        SqlCommand cmdContract = new SqlCommand();
                        SqlConnection con = new SqlConnection(_cs["ConnectionStrings:membership"]);
                        cmdContract.CommandText = "dbo.sp_CFTContractEdit";
                        cmdContract.CommandType = CommandType.StoredProcedure;
                        cmdContract.Parameters.AddWithValue("@ID", ct.ID);
                        cmdContract.Parameters.AddWithValue("@Attachement", ct.Attachement == null ? "" : ct.Attachement);
                        cmdContract.Parameters.AddWithValue("@CommodityId", ct.CommodityId);
                        cmdContract.Parameters.AddWithValue("@CommodityClassId", ct.CommodityClassId);
                        cmdContract.Parameters.AddWithValue("@ContractDate", ct.ContractDate);
                        cmdContract.Parameters.AddWithValue("@ECXWarehouseId", ct.ECXWarehouseId);
                        cmdContract.Parameters.AddWithValue("@MaturityDate", ct.MaturityDate);
                        cmdContract.Parameters.AddWithValue("@MakerId", updatedBy);
                        cmdContract.Parameters.AddWithValue("@OptionId", ct.OptionId);
                        cmdContract.Parameters.AddWithValue("@ProductionYear", ct.ProductionYear);
                        cmdContract.Parameters.AddWithValue("@Price", ct.Price);
                        cmdContract.Parameters.AddWithValue("@QuantityInLot", ct.QuantityInLot);
                        cmdContract.Parameters.AddWithValue("@QuantityNetWeight", ct.QuantityNetWeight);
                        cmdContract.Parameters.AddWithValue("@StatusId", ct.StatusId);
                        cmdContract.Parameters.AddWithValue("@Symbol", ct.Symbol);
                        cmdContract.Parameters.AddWithValue("@TraderWarehouse",  ct.TraderWarehouse == null ? "" : ct.TraderWarehouse);

                        cmdContract.Connection = con;
                        result = _db.ExecuteSqlTransaction(cmdContract, cmdBuyer, cmdSeller);
                    }
                }
            }
            return result;
        }
        public DataTable GetDetailContract(Guid Id)
        {
            var erroMesg = "";
            ArrayList srParamName = new ArrayList();
            ArrayList srParamValue = new ArrayList();
            srParamName.Add("Id");
            srParamValue.Add(Id);
            DataTable data = _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "sp_CFTGetDetailContractSummary", srParamName, srParamValue, ref erroMesg);
            /* if (data.Rows.Count > 0)
             {
                 data.Rows[0]["Attachement"] = String.Format(_cs["AttachementDirectory"] + "//" + data.Rows[0]["Attachement"]);
             }*/
            return data;
        }

        public DataTable GetContractForAmmendment(Guid Id)
        {
            var erroMesg = "";
            ArrayList srParamName = new ArrayList();
            ArrayList srParamValue = new ArrayList();
            srParamName.Add("Id");
            srParamValue.Add(Id);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "sp_CFTGetDetailContractForAmmend", srParamName, srParamValue, ref erroMesg);
        }

        public bool SaveAmmendment(ContractAmmendment _ammendment, Guid updatedBy)
        {
            string erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("Id");
            paramName.Add("CommodityId");
            paramName.Add("ClassId");
            paramName.Add("CommodityGrade");
            paramName.Add("QuantityInLot");
            paramName.Add("QuantityNetWeight");
            paramName.Add("Price");
            paramName.Add("AmmendedBy");
            paramValue.Add(_ammendment.Id);
            paramValue.Add(_ammendment.CommodityId);
            paramValue.Add(_ammendment.CommodityClassId);
            paramValue.Add(_ammendment.CommodityGrades);
            paramValue.Add(_ammendment.QuantityInLot);
            paramValue.Add(_ammendment.QuantityNetWeight);
            paramValue.Add(_ammendment.Price);
            paramValue.Add(updatedBy);
            return _db.ExecuteNonQuery(_cs["ConnectionStrings:membership"], "dbo", "sp_CFTAmmendContract", paramName, paramValue, ref erroMesg);
        }

        public string ValidateContractInput(ContractDTO _contract)
        {
            if (_contract == null || _contract.Buyer == null || _contract.Buyer == null || _contract.Contract == null)
            {
                return "Invalid Request";
            }
            #region Validate Seller

            if (_contract.Seller.ClientId == Guid.Empty)
            {
                return "Seller Client Id number can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Seller.ClientName))
            {
                return "Seller Client name can not be null or empty";
            }
            if (_contract.Seller.MemberId == Guid.Empty)
            {
                return "Seller Member Id number can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Seller.MemberName))
            {
                return "Seller Member name can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Seller.HouseNo))
            {
                return "Seller's House number can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Seller.Phone))
            {
                return "Seller's Phone number can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Seller.Region))
            {
                return "Seller's Region can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Seller.TINNumber))
            {
                return "Seller's TIN number can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Seller.VATNumber))
            {
                return "Seller's VAT number can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Seller.Woreda))
            {
                return "Seller's Woreda can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Seller.Zone))
            {
                return "Seller's Zone can not be null or empty";
            }
            #endregion

            #region Validate Buyer

            if (_contract.Buyer.ClientId == Guid.Empty)
            {
                return "Buyer Client Id number can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Buyer.ClientName))
            {
                return "Buyer Client name can not be null or empty";
            }
            if (_contract.Buyer.MemberId == Guid.Empty)
            {
                return "Buyer Member Id number can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Buyer.MemberName))
            {
                return "Buyer Member name can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Buyer.HouseNo))
            {
                return "Buyer's House number can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Buyer.Phone))
            {
                return "Buyer's Phone number can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Buyer.Region))
            {
                return "Buyer's Region can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Buyer.TINNumber))
            {
                return "Buyer's TIN number can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Buyer.VATNumber))
            {
                return "Buyer's VAT number can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Buyer.Woreda))
            {
                return "Buyer's Woreda can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Buyer.Zone))
            {
                return "Buyer's Zone can not be null or empty";
            }
            #endregion

            #region Validate Contract

            DateTime contractDate;
            if (!DateTime.TryParse(_contract.Contract.ContractDate.ToShortDateString(), out contractDate))
            {
                return "Invalid Contract date";
            }
            DateTime maturityDate;
            if (!DateTime.TryParse(_contract.Contract.MaturityDate.ToShortDateString(), out maturityDate))
            {
                return "Invalid Maturity date";
            }
            int option;
            if (!int.TryParse(_contract.Contract.OptionId.ToString(), out option))
            {
                return "Invalid option";
            }
            decimal quantityLot;
            if (!decimal.TryParse(_contract.Contract.QuantityInLot.ToString(), out quantityLot))
            {
                return "Invalid Quantity in Lot";
            }
            decimal quantityQuintal;
            if (!decimal.TryParse(_contract.Contract.QuantityNetWeight.ToString(), out quantityQuintal))
            {
                return "Invalid Net Weight quantity";
            }
            decimal productionYear;
            if (!decimal.TryParse(_contract.Contract.ProductionYear.ToString(), out productionYear))
            {
                return "Invalid Production Year";
            }
            /*  if (_contract.Contract.ContractNumber == null || _contract.Contract.ContractNumber))
              {
                  return "Contract Number can not be null or empty";
              }*/
            if (String.IsNullOrEmpty(_contract.Contract.Price))
            {
                return "Price can not be null or empty";
            }
            if (String.IsNullOrEmpty(_contract.Contract.Symbol))
            {
                return "Symbol can not be null or empty";
            }
            /* if (_contract.Contract.TraderWarehouse == null || _contract.Contract.TraderWarehouse))
             {
                 return "Trader warehouse can not be null or empty";
             }   */

            #endregion
            return "OK";
        }
        public string ValidateContract(ContractDTO _contract)
        {
            //Active
            if (CheckActive(_contract.Seller.MemberId).Rows.Count == 0)
                return "Seller Member is InActive";
            if (CheckActive(_contract.Seller.ClientId).Rows.Count == 0)
                return "Seller Client is InActive";
            if (CheckActive(_contract.Buyer.MemberId).Rows.Count == 0)
                return "Buyer Member is InActive";
            if (CheckActive(_contract.Buyer.ClientId).Rows.Count == 0)
                return "Buyer Client is InActive";
            //seller vs buyer difference
            if (_contract.Seller.MemberId == _contract.Buyer.MemberId)
                return "Seller and Buyer can not have the same Member";
            //Owner License

            if (_contract.Seller.MemberId == _contract.Seller.ClientId)
            {
                if (CheckMemberLicense(_contract.Seller.MemberId, _contract.Contract.CommodityId).Rows.Count == 0)
                    return "No Seller Member Business Liceness";
            }
            else
            {
                if (CheckClientLicense(_contract.Seller.ClientId, _contract.Contract.CommodityId).Rows.Count == 0)
                    return "No Seller Client Business Licesness";
            }
            if (_contract.Buyer.MemberId == _contract.Buyer.ClientId)
            {
                if (CheckMemberLicense(_contract.Buyer.MemberId, _contract.Contract.CommodityId).Rows.Count == 0)
                    return "No Buyer Member Business Liceness";
            }
            else
            {
                if (CheckClientLicense(_contract.Buyer.ClientId, _contract.Contract.CommodityId).Rows.Count == 0)
                    return "No Buyer Client Business Licesness";
            }

            //COC
            if (_contract.Contract.CommodityId.ToString() == "71604275-df23-4449-9dae-36501b14cc3b") //coffee
            {
                if (_contract.Seller.MemberId == _contract.Seller.ClientId)
                {
                    if (CheckMemberCOC(_contract.Seller.MemberId, _contract.Contract.CommodityId).Rows.Count == 0)
                        return "No Seller Member COC";
                }
                else
                {
                    if (CheckClientCOC(_contract.Seller.ClientId, _contract.Contract.CommodityId).Rows.Count == 0)
                        return "No Seller Client COC";
                }
                if (_contract.Buyer.MemberId == _contract.Buyer.ClientId)
                {
                    if (CheckMemberCOC(_contract.Buyer.MemberId, _contract.Contract.CommodityId).Rows.Count == 0)
                        return "No Buyer Member COC";
                }
                else
                {
                    if (CheckMemberCOC(_contract.Buyer.ClientId, _contract.Contract.CommodityId).Rows.Count == 0)
                        return "No Buyer Client COC";
                }
            }
            //Aggreement
            if (_contract.Seller.MemberId != _contract.Seller.ClientId)
            {
                if (CheckAggrement(_contract.Seller.ClientId, _contract.Seller.MemberId, _contract.Contract.CommodityId).Rows.Count == 0)
                    return "Seller doesn't have Aggrement with this Member";

            }
            if (_contract.Buyer.MemberId != _contract.Buyer.ClientId)
            {
                if (CheckAggrement(_contract.Buyer.ClientId, _contract.Buyer.MemberId, _contract.Contract.CommodityId).Rows.Count == 0)
                    return "Buyer doesn't have Aggrement with this Member";
            }
            //Maturity date
            double datediff = (_contract.Contract.MaturityDate - _contract.Contract.ContractDate).TotalDays;
            if (datediff < Convert.ToDouble(_cs["MinimumMaturityDate"]))
                return "Maturity Date is Below from the Minimum date";
            if (datediff > Convert.ToDouble(_cs["MaximumMaturityDate"]))
                return "Maturity Date is above from the Maximum date";
            return "OK";
        }
        private DataTable CheckActive(Guid id)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("OwnerId");
            paramValue.Add(id);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "spCFTMemberClientActive", paramName, paramValue, ref erroMesg);
        }
        private DataTable CheckMemberLicense(Guid _owner, Guid _commodity)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("ownerId");
            paramName.Add("commodityGuid");
            paramValue.Add(_owner);
            paramValue.Add(_commodity);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "spCFTValidateMemberLicense", paramName, paramValue, ref erroMesg);
        }
        private DataTable CheckClientLicense(Guid _owner, Guid _commodity)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("ownerId");
            paramName.Add("commodityGuid");
            paramValue.Add(_owner);
            paramValue.Add(_commodity);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "spCFTValidateClientLicense", paramName, paramValue, ref erroMesg);
        }
        private DataTable CheckMemberCOC(Guid _owner, Guid _commodity)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("ownerId");
            paramName.Add("commodityGuid");
            paramValue.Add(_owner);
            paramValue.Add(_commodity);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "spCFTValidateMemberCOC", paramName, paramValue, ref erroMesg);
        }
        private DataTable CheckClientCOC(Guid _owner, Guid _commodity)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("ownerId");
            paramName.Add("commodityGuid");
            paramValue.Add(_owner);
            paramValue.Add(_commodity);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "spCFTValidateClientCOC", paramName, paramValue, ref erroMesg);
        }
        private DataTable CheckAggrement(Guid _cliemtId, Guid _memberId, Guid _commodity)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("clientId");
            paramName.Add("memberId");
            paramName.Add("commodityId");
            paramValue.Add(_cliemtId);
            paramValue.Add(_memberId);
            paramValue.Add(_commodity);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "spCFTGetClientMCADetail", paramName, paramValue, ref erroMesg);
        }
        public int SaveMargin(Margin _margin, Guid createdBy)
        {

            string erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("CFTTraderId");
            paramName.Add("MarginId");
            paramName.Add("Amount");
            paramName.Add("QuantityInLot");
            paramName.Add("MakerId");
            paramName.Add("AccountID");
            paramName.Add("CFTID");
            paramValue.Add(_margin.CFTTraderId);
            paramValue.Add(_margin.MarginId);
            paramValue.Add(_margin.Amount);
            paramValue.Add(_margin.QuantityInLot);
            paramValue.Add(createdBy);
            paramValue.Add(_margin.AccountNumber);
            paramValue.Add(_margin.CFTId);
            return _db.ExecuteNonQuery(_cs["ConnectionStrings:membership"], "dbo", "spCFTSaveMargin", paramName, paramValue,"output",  ref erroMesg);
            
        }
        public bool ApproveReleaseMargin(Guid CFTTraderId, int status, Guid updatedBy)
        {
            string erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("CFTTraderId");
            paramName.Add("Status");
            paramName.Add("CheckerId");

            paramValue.Add(CFTTraderId);
            paramValue.Add(status);
            paramValue.Add(updatedBy);
            return _db.ExecuteNonQuery(_cs["ConnectionStrings:membership"], "dbo", "spCFTApproveReleaseMargin", paramName, paramValue, ref erroMesg);

        }
        public bool EditMargin(Margin _margin, Guid updatedBy)
        {
            string erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("CFTTraderId");
            paramName.Add("MarginId");
            paramName.Add("Amount");
            paramName.Add("QuantityInLot");
            paramName.Add("MakerId");

            paramValue.Add(_margin.CFTTraderId);
            paramValue.Add(_margin.MarginId);
            paramValue.Add(_margin.Amount);
            paramValue.Add(_margin.QuantityInLot);
            paramValue.Add(updatedBy);
            return _db.ExecuteNonQuery(_cs["ConnectionStrings:membership"], "dbo", "spCFTUpdateMargin", paramName, paramValue, ref erroMesg);

        }
        public DataTable Getmargin(Guid cftId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("CFTId");
            paramValue.Add(cftId);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "spCFTGetMarginByCFTId", paramName, paramValue, ref erroMesg);
        }

        public DataTable ContractHasMargin(Guid cftId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("CftId");
            paramValue.Add(cftId);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "spCFTGetCFTOption", paramName, paramValue, ref erroMesg);
        }
        public DataTable GetMargin(Guid cftId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("CftId");
            paramValue.Add(cftId);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "spCFTGetMargin", paramName, paramValue, ref erroMesg);
        }
        public DataTable GetMarginByTrader(Guid cftTraderId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("CftTrader");
            paramValue.Add(cftTraderId);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "spCFTGetMarginByTraderId", paramName, paramValue, ref erroMesg);
        }

        public DataTable GetContractByCFTID(Guid cFTID)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("CFTID");
            paramValue.Add(cFTID);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "spCFTGetContractByCFTID", paramName, paramValue, ref erroMesg);

        }
        public DataTable GetContractHistoryById(Guid cFTID)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("Id");
            paramValue.Add(cFTID);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "sp_CFTGetContractHistoryById", paramName, paramValue, ref erroMesg);
            
        }

        public DataTable GetColateralizedReport()
        {
            var erroMesg = "";
            return _db.ExecuteDataTable(_cs["ConnectionStrings:cns"], "dbo", "spCFTGetCollateralizedReport", ref erroMesg);
        }

        public bool ReleaseMargin(Guid cFTTraderId, Guid updatedBy)
        {
            string erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("cFTTraderId");
            paramName.Add("updatedBy");

            paramValue.Add(cFTTraderId);
            paramValue.Add(updatedBy);
            return _db.ExecuteNonQuery(_cs["ConnectionStrings:membership"], "dbo", "spCFTReleaseMargin", paramName, paramValue, ref erroMesg);
        }
    }
}
